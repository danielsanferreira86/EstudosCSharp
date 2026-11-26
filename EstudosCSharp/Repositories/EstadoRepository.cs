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

        // PESQUISAR ESTADOS COM PAGINAÇÃO
        public async Task<List<Estado>> PesquisarAsync(
            string? pesquisa,
            int pagina = 1,
            int tamanhoPagina = 10) {
            if (pagina < 1)
                throw new ArgumentOutOfRangeException(nameof(pagina));

            if (tamanhoPagina < 1 || tamanhoPagina > 100)
                throw new ArgumentOutOfRangeException(
                    nameof(tamanhoPagina));

            List<Estado> estados = new();

            int offset = checked((pagina - 1) * tamanhoPagina);

            const string sql = """
                SELECT ID, Nome, UF, Codigo_IBGE
                FROM Estado
                WHERE Nome LIKE @Pesquisa
                   OR UF LIKE @Pesquisa
                   OR Codigo_IBGE LIKE @Pesquisa
                ORDER BY Nome, ID
                OFFSET @Offset ROWS
                FETCH NEXT @TamanhoPagina ROWS ONLY
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            cmd.Parameters.Add("@Pesquisa", SqlDbType.NVarChar, -1)
                .Value = $"%{pesquisa?.Trim()}%";

            cmd.Parameters.Add("@Offset", SqlDbType.Int)
                .Value = offset;

            cmd.Parameters.Add("@TamanhoPagina", SqlDbType.Int)
                .Value = tamanhoPagina;

            using SqlDataReader reader =
                await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync()) {
                estados.Add(MapearEstado(reader));
            }

            return estados;
        }

        // CONTAR REGISTROS DA PESQUISA
        public async Task<int> ContarAsync(
            string? pesquisa = null) {
            const string sql = """
                SELECT COUNT(*)
                FROM Estado
                WHERE Nome LIKE @Pesquisa
                   OR UF LIKE @Pesquisa
                   OR Codigo_IBGE LIKE @Pesquisa
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            cmd.Parameters.Add("@Pesquisa", SqlDbType.NVarChar, -1)
                .Value = $"%{pesquisa?.Trim()}%";

            return Convert.ToInt32(
                await cmd.ExecuteScalarAsync());
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

            int linhasAfetadas = await cmd.ExecuteNonQueryAsync();

            return linhasAfetadas == 1;
        }

        // VERIFICAR DUPLICIDADE DE UF
        public async Task<bool> ExisteUFAsync(
            string uf,
            int? idIgnorar = null) {
            const string sql = """
                SELECT CAST(
                    CASE WHEN EXISTS
                    (
                        SELECT 1
                        FROM Estado
                        WHERE UF = @UF
                          AND (@ID IS NULL OR ID <> @ID)
                    )
                    THEN 1 ELSE 0 END
                AS BIT)
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            cmd.Parameters.Add("@UF", SqlDbType.Char, 2)
                .Value = uf.Trim().ToUpperInvariant();

            cmd.Parameters.Add("@ID", SqlDbType.Int)
                .Value = (object?)idIgnorar ?? DBNull.Value;

            return Convert.ToBoolean(
                await cmd.ExecuteScalarAsync());
        }

        // VERIFICAR DUPLICIDADE DO CÓDIGO IBGE
        public async Task<bool> ExisteCodigoIBGEAsync(
            string codigoIBGE,
            int? idIgnorar = null) {
            const string sql = """
                SELECT CAST(
                    CASE WHEN EXISTS
                    (
                        SELECT 1
                        FROM Estado
                        WHERE Codigo_IBGE = @Codigo_IBGE
                          AND (@ID IS NULL OR ID <> @ID)
                    )
                    THEN 1 ELSE 0 END
                AS BIT)
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            cmd.Parameters.Add("@Codigo_IBGE", SqlDbType.Char, 2)
                .Value = codigoIBGE.Trim();

            cmd.Parameters.Add("@ID", SqlDbType.Int)
                .Value = (object?)idIgnorar ?? DBNull.Value;

            return Convert.ToBoolean(
                await cmd.ExecuteScalarAsync());
        }

        // VERIFICAR SE O ESTADO POSSUI CIDADES
        public async Task<bool> PossuiCidadesAsync(int idEstado) {
            const string sql = """
                SELECT CAST(
                    CASE WHEN EXISTS
                    (
                        SELECT 1
                        FROM Cidade
                        WHERE ID_Estado = @ID_Estado
                    )
                    THEN 1 ELSE 0 END
                AS BIT)
                """;

            using SqlConnection conn = _conexao.CriarConexao();

            await conn.OpenAsync();

            using SqlCommand cmd = new(sql, conn);

            cmd.Parameters.Add("@ID_Estado", SqlDbType.Int)
                .Value = idEstado;

            return Convert.ToBoolean(
                await cmd.ExecuteScalarAsync());
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
                .Value = estado.Nome.Trim();

            cmd.Parameters.Add("@UF", SqlDbType.Char, 2)
                .Value = estado.UF.Trim().ToUpperInvariant();

            cmd.Parameters.Add("@Codigo_IBGE", SqlDbType.Char, 2)
                .Value = estado.Codigo_IBGE.Trim();
        }
    }
}