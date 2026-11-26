using System.Data;
using Microsoft.Data.SqlClient;
using EstudosCSharp.Data;
using EstudosCSharp.Models;

namespace EstudosCSharp.Repositories {
    public class UsuariosRepository {
        private readonly Conexao _conexao;

        public UsuariosRepository(Conexao conexao) {
            _conexao = conexao;
        }

        private const string SelectBase = """
            SELECT ID, Nome, CPF, Telefone, Email,
                   Login, Tipo_Usuario, Ativo
            FROM Usuarios
            """;

        // LISTAR USUÁRIOS
        public async Task<List<Usuarios>> ListarAsync() {
            List<Usuarios> usuarios = new();

            string sql = SelectBase + "\nORDER BY Nome";

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync()) {
                usuarios.Add(MapearUsuario(reader));
            }

            return usuarios;
        }

        // BUSCAR USUÁRIO PELO ID
        public async Task<Usuarios?> BuscarPorIdAsync(int id) {
            string sql = SelectBase + "\nWHERE ID = @ID";

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync()) {
                return MapearUsuario(reader);
            }

            return null;
        }

        // BUSCAR USUÁRIO PELO LOGIN
        // Utilizado exclusivamente para autenticação.
        public async Task<Usuarios?> BuscarPorLoginAsync(
            string login) {
            const string sql = """
                SELECT ID, Nome, CPF, Telefone, Email,
                       Login, SenhaHash, Tipo_Usuario, Ativo
                FROM Usuarios
                WHERE Login = @Login
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            cmd.Parameters.Add("@Login", SqlDbType.VarChar, 50)
                .Value = login;

            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync()) {
                Usuarios usuario = MapearUsuario(reader);

                usuario.SenhaHash = reader.GetString(
                    reader.GetOrdinal("SenhaHash"));

                return usuario;
            }

            return null;
        }

        // INSERIR USUÁRIO
        public async Task<int> InserirAsync(Usuarios usuario) {
            const string sql = """
                INSERT INTO Usuarios
                (
                    Nome, CPF, Telefone, Email,
                    Login, SenhaHash, Tipo_Usuario, Ativo
                )
                OUTPUT INSERTED.ID
                VALUES
                (
                    @Nome, @CPF, @Telefone, @Email,
                    @Login, @SenhaHash, @Tipo_Usuario, @Ativo
                )
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            AdicionarParametros(cmd, usuario);

            cmd.Parameters.Add("@SenhaHash", SqlDbType.VarChar, 255)
                .Value = usuario.SenhaHash;

            return Convert.ToInt32(
                await cmd.ExecuteScalarAsync());
        }

        // ATUALIZAR DADOS DO USUÁRIO
        // Não altera a senha.
        public async Task<bool> AtualizarAsync(Usuarios usuario) {
            const string sql = """
                UPDATE Usuarios
                SET Nome = @Nome,
                    CPF = @CPF,
                    Telefone = @Telefone,
                    Email = @Email,
                    Login = @Login,
                    Tipo_Usuario = @Tipo_Usuario,
                    Ativo = @Ativo
                WHERE ID = @ID
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            AdicionarParametros(cmd, usuario);

            cmd.Parameters.Add("@ID", SqlDbType.Int)
                .Value = usuario.ID;

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        // ALTERAR O HASH DA SENHA
        public async Task<bool> AlterarSenhaAsync(
            int id, string senhaHash) {
            const string sql = """
                UPDATE Usuarios
                SET SenhaHash = @SenhaHash
                WHERE ID = @ID
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

            cmd.Parameters.Add("@SenhaHash", SqlDbType.VarChar, 255)
                .Value = senhaHash;

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        // INATIVAR USUÁRIO
        public async Task<bool> InativarAsync(int id) {
            const string sql = """
                UPDATE Usuarios
                SET Ativo = 0
                WHERE ID = @ID
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        // MAPEAR OS DADOS DO USUÁRIO
        private static Usuarios MapearUsuario(SqlDataReader reader) {
            return new Usuarios {
                ID = reader.GetInt32(
                    reader.GetOrdinal("ID")),

                Nome = reader.GetString(
                    reader.GetOrdinal("Nome")),

                CPF = reader.GetString(
                    reader.GetOrdinal("CPF")),

                Telefone = reader.IsDBNull(
                    reader.GetOrdinal("Telefone"))
                    ? null
                    : reader.GetString(
                        reader.GetOrdinal("Telefone")),

                Email = reader.GetString(
                    reader.GetOrdinal("Email")),

                Login = reader.GetString(
                    reader.GetOrdinal("Login")),

                Tipo_Usuario = reader.GetString(
                    reader.GetOrdinal("Tipo_Usuario")),

                Ativo = reader.GetBoolean(
                    reader.GetOrdinal("Ativo"))
            };
        }

        // PARÂMETROS DE INCLUSÃO E ALTERAÇÃO
        private static void AdicionarParametros(
            SqlCommand cmd, Usuarios usuario) {
            cmd.Parameters.Add("@Nome", SqlDbType.VarChar, 150)
                .Value = usuario.Nome;

            cmd.Parameters.Add("@CPF", SqlDbType.Char, 11)
                .Value = usuario.CPF;

            cmd.Parameters.Add("@Telefone", SqlDbType.VarChar, 15)
                .Value = (object?)usuario.Telefone ?? DBNull.Value;

            cmd.Parameters.Add("@Email", SqlDbType.VarChar, 150)
                .Value = usuario.Email;

            cmd.Parameters.Add("@Login", SqlDbType.VarChar, 50)
                .Value = usuario.Login;

            cmd.Parameters.Add("@Tipo_Usuario", SqlDbType.VarChar, 15)
                .Value = usuario.Tipo_Usuario;

            cmd.Parameters.Add("@Ativo", SqlDbType.Bit)
                .Value = usuario.Ativo;
        }
    }
}