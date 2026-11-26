using Microsoft.Data.SqlClient;

namespace EstudosCSharp.Data {
    public class Conexao {
        // Armazena a string de conexão com o banco.
        private readonly string _connectionString;

        // Construtor da classe.
        public Conexao(IConfiguration configuration) {
            // Recupera a configuração do appsettings.json.
            _connectionString = configuration
                .GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "String de conexão não encontrada.");
        }

        // Cria uma nova instância de conexão.
        public SqlConnection CriarConexao() {
            return new SqlConnection(_connectionString);
        }
    }
}