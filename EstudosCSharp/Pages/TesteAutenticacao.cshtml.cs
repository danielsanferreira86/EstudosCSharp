using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EstudosCSharp.Services;
using EstudosCSharp.ViewModels;

namespace EstudosCSharp.Pages {
    public class TesteAutenticacaoModel : PageModel {
        private readonly AutenticacaoService _autenticacaoService;
        private readonly IWebHostEnvironment _environment;

        [BindProperty]
        public LoginViewModel Credenciais { get; set; } = new();

        public ResultadoAutenticacao? Resultado {
            get;
            private set;
        }

        public TesteAutenticacaoModel(
            AutenticacaoService autenticacaoService,
            IWebHostEnvironment environment) {
            _autenticacaoService = autenticacaoService;
            _environment = environment;
        }

        // Executado ao acessar a página.
        public IActionResult OnGet() {
            if (!_environment.IsDevelopment()) {
                return NotFound();
            }

            return Page();
        }

        // Executado ao enviar o formulário.
        public async Task<IActionResult> OnPostAsync() {
            if (!_environment.IsDevelopment()) {
                return NotFound();
            }

            // Valida as Data Annotations.
            if (!ModelState.IsValid) {
                Credenciais.Senha = string.Empty;

                return Page();
            }

            // Executa o serviço de autenticação.
            Resultado =
                await _autenticacaoService.AutenticarAsync(
                    Credenciais.Login,
                    Credenciais.Senha);

            // Limpa a senha após a validação.
            Credenciais.Senha = string.Empty;

            ModelState.Remove("Credenciais.Senha");

            // Retorna para a mesma página.
            return Page();
        }
    }
}