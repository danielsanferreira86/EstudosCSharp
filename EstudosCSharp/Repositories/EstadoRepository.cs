using System.Data;
using Microsoft.Data.SqlClient;
using EstudosCSharp.Data;
using EstudosCSharp.Models;

namespace EstudosCSharp.Repositories {
    public class EstadoRepository {
        private readonly Conexao _conexao;

        // Injeção de dependência.
        public EstadoRepository(Conexao conexao) {
            _conexao = conexao;
        }

        // LISTAR TODOS OS ESTADOS
        public async Task<List<Estado>> ListarAsync() {
            List<Estado> estados = new();

            const string sql = """
                SELECT ID, Nome, UF, Codigo_IBGE
                FROM Estado
                ORDER BY Nome
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync()) {
                estados.Add(MapearEstado(reader));
            }

            return estados;
        }

        // BUSCAR ESTADO PELO ID
        public async Task<Estado?> BuscarPorIdAsync(int id) {
            const string sql = """
                SELECT ID, Nome, UF, Codigo_IBGE
                FROM Estado
                WHERE ID = @ID
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync()) {
                return MapearEstado(reader);
            }

            return null;
        }

        // INSERIR ESTADO
        public async Task<int> InserirAsync(Estado estado) {
            const string sql = """
                INSERT INTO Estado (Nome, UF, Codigo_IBGE)
                OUTPUT INSERTED.ID
                VALUES (@Nome, @UF, @Codigo_IBGE)
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            AdicionarParametros(cmd, estado);

            return Convert.ToInt32(
                await cmd.ExecuteScalarAsync());
        }

        // ATUALIZAR ESTADO
        public async Task<bool> AtualizarAsync(Estado estado) {
            const string sql = """
                UPDATE Estado
                SET Nome = @Nome,
                    UF = @UF,
                    Codigo_IBGE = @Codigo_IBGE
                WHERE ID = @ID
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            AdicionarParametros(cmd, estado);

            cmd.Parameters.Add("@ID", SqlDbType.Int)
                .Value = estado.ID;

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        // EXCLUIR ESTADO
        public async Task<bool> ExcluirAsync(int id) {
            const string sql = """
                DELETE FROM Estado
                WHERE ID = @ID
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        // MÉTODO AUXILIAR PARA PREENCHER A MODEL
        private static Estado MapearEstado(SqlDataReader reader) {
            return new Estado {
                ID = reader.GetInt32(
                    reader.GetOrdinal("ID")),

                Nome = reader.GetString(
                    reader.GetOrdinal("Nome")),

                UF = reader.GetString(
                    reader.GetOrdinal("UF")),

                Codigo_IBGE = reader.GetString(
                    reader.GetOrdinal("Codigo_IBGE"))
            };
        }

        // MÉTODO AUXILIAR PARA OS PARÂMETROS
        private static void AdicionarParametros(
            SqlCommand cmd, Estado estado) {
            cmd.Parameters.Add("@Nome", SqlDbType.NVarChar, 50)
                .Value = estado.Nome;

            cmd.Parameters.Add("@UF", SqlDbType.Char, 2)
                .Value = estado.UF;

            cmd.Parameters.Add("@Codigo_IBGE", SqlDbType.Char, 2)
                .Value = estado.Codigo_IBGE;
        }
    }
}