using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using EstudosCSharp.Data;

namespace EstudosCSharp.Pages {
    public class TesteConexaoModel : PageModel {
        private readonly Conexao _conexao;

        public string Mensagem { get; private set; } = "";
        public bool Sucesso { get; private set; }

        // Injeção de dependência pelo construtor.
        public TesteConexaoModel(Conexao conexao) {
            _conexao = conexao;
        }

        // Executado ao acessar a página.
        public async Task OnGetAsync() {
            try {
                // Cria a conexão com o banco.
                using SqlConnection conn =
                    _conexao.CriarConexao();

                // Abre a conexão.
                await conn.OpenAsync();

                Sucesso = true;

                Mensagem =
                    $"Conexão realizada com sucesso! " +
                    $"Banco: {conn.Database}";
            } catch (SqlException ex) {
                Sucesso = false;

                Mensagem =
                    $"Erro ao conectar: {ex.Message}";
            }
        }
    }
}