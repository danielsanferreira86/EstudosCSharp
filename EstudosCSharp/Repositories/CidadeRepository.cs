using System.Data;
using Microsoft.Data.SqlClient;
using EstudosCSharp.Data;
using EstudosCSharp.Models;

namespace EstudosCSharp.Repositories {
    public class CidadeRepository {
        private readonly Conexao _conexao;

        public CidadeRepository(Conexao conexao) {
            _conexao = conexao;
        }

        // Consulta base reutilizada pelos métodos.
        private const string SelectBase = """
            SELECT
                C.ID,
                C.Nome,
                C.Codigo_IBGE,
                C.ID_Estado,
                E.Nome AS Estado_Nome,
                E.UF AS Estado_UF,
                E.Codigo_IBGE AS Estado_IBGE
            FROM Cidade C
            INNER JOIN Estado E ON E.ID = C.ID_Estado
            """;

        // LISTAR TODAS AS CIDADES
        public async Task<List<Cidade>> ListarAsync() {
            List<Cidade> cidades = new();

            string sql = SelectBase +
                "\nORDER BY C.Nome";

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync()) {
                cidades.Add(MapearCidade(reader));
            }

            return cidades;
        }

        // BUSCAR CIDADE PELO ID
        public async Task<Cidade?> BuscarPorIdAsync(int id) {
            string sql = SelectBase +
                "\nWHERE C.ID = @ID";

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();

            if (await reader.ReadAsync()) {
                return MapearCidade(reader);
            }

            return null;
        }

        // LISTAR CIDADES DE UM ESTADO
        public async Task<List<Cidade>> ListarPorEstadoAsync(
            int idEstado) {
            List<Cidade> cidades = new();

            string sql = SelectBase + """

                WHERE C.ID_Estado = @ID_Estado
                ORDER BY C.Nome
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            cmd.Parameters.Add("@ID_Estado", SqlDbType.Int)
                .Value = idEstado;

            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync()) {
                cidades.Add(MapearCidade(reader));
            }

            return cidades;
        }

        // INSERIR CIDADE
        public async Task<int> InserirAsync(Cidade cidade) {
            const string sql = """
                INSERT INTO Cidade
                    (Nome, Codigo_IBGE, ID_Estado)
                OUTPUT INSERTED.ID
                VALUES
                    (@Nome, @Codigo_IBGE, @ID_Estado)
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            AdicionarParametros(cmd, cidade);

            return Convert.ToInt32(
                await cmd.ExecuteScalarAsync());
        }

        // ATUALIZAR CIDADE
        public async Task<bool> AtualizarAsync(Cidade cidade) {
            const string sql = """
                UPDATE Cidade
                SET Nome = @Nome,
                    Codigo_IBGE = @Codigo_IBGE,
                    ID_Estado = @ID_Estado
                WHERE ID = @ID
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            AdicionarParametros(cmd, cidade);

            cmd.Parameters.Add("@ID", SqlDbType.Int)
                .Value = cidade.ID;

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        // EXCLUIR CIDADE
        public async Task<bool> ExcluirAsync(int id) {
            const string sql = """
                DELETE FROM Cidade
                WHERE ID = @ID
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            cmd.Parameters.Add("@ID", SqlDbType.Int).Value = id;

            return await cmd.ExecuteNonQueryAsync() > 0;
        }

        // MAPEAR CIDADE E ESTADO
        private static Cidade MapearCidade(SqlDataReader reader) {
            return new Cidade {
                ID = reader.GetInt32(
                    reader.GetOrdinal("ID")),

                Nome = reader.GetString(
                    reader.GetOrdinal("Nome")),

                Codigo_IBGE = reader.GetString(
                    reader.GetOrdinal("Codigo_IBGE")),

                ID_Estado = reader.GetInt32(
                    reader.GetOrdinal("ID_Estado")),

                Estado = new Estado {
                    ID = reader.GetInt32(
                        reader.GetOrdinal("ID_Estado")),

                    Nome = reader.GetString(
                        reader.GetOrdinal("Estado_Nome")),

                    UF = reader.GetString(
                        reader.GetOrdinal("Estado_UF")),

                    Codigo_IBGE = reader.GetString(
                        reader.GetOrdinal("Estado_IBGE"))
                }
            };
        }

        // PARÂMETROS DE INCLUSÃO E ALTERAÇÃO
        private static void AdicionarParametros(
            SqlCommand cmd, Cidade cidade) {
            cmd.Parameters.Add("@Nome", SqlDbType.NVarChar, 100)
                .Value = cidade.Nome;

            cmd.Parameters.Add("@Codigo_IBGE", SqlDbType.Char, 7)
                .Value = cidade.Codigo_IBGE;

            cmd.Parameters.Add("@ID_Estado", SqlDbType.Int)
                .Value = cidade.ID_Estado;
        }
    }
}