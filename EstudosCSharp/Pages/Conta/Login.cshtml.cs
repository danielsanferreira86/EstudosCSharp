using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EstudosCSharp.Services;
using EstudosCSharp.ViewModels;

namespace EstudosCSharp.Pages.Conta {
    [AllowAnonymous]
    public class LoginModel : PageModel {
        private readonly AutenticacaoService _autenticacaoService;

        [BindProperty]
        public LoginViewModel Credenciais { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public LoginModel(
            AutenticacaoService autenticacaoService) {
            _autenticacaoService = autenticacaoService;
        }

        // ACESSAR A PÁGINA DE LOGIN
        public IActionResult OnGet() {
            if (User.Identity?.IsAuthenticated == true) {
                return RedirecionarUsuario();
            }

            return Page();
        }

        // PROCESSAR O FORMULÁRIO DE LOGIN
        public async Task<IActionResult> OnPostAsync() {
            if (User.Identity?.IsAuthenticated == true) {
                return RedirecionarUsuario();
            }

            // Validação dos campos.
            if (!ModelState.IsValid) {
                Credenciais.Senha = string.Empty;

                return Page();
            }

            // Validação das credenciais.
            ResultadoAutenticacao resultado =
                await _autenticacaoService.AutenticarAsync(
                    Credenciais.Login,
                    Credenciais.Senha);

            // Limpa a senha após a utilização.
            Credenciais.Senha = string.Empty;

            ModelState.Remove("Credenciais.Senha");

            if (!resultado.Sucesso) {
                ModelState.AddModelError(
                    string.Empty,
                    resultado.Mensagem);

                return Page();
            }

            // Criação das Claims.
            List<Claim> claims = new()
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    resultado.ID_Usuario.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    resultado.Nome),

                new Claim(
                    ClaimTypes.Role,
                    resultado.Tipo_Usuario),

                new Claim(
                    "Login",
                    resultado.Login)
            };

            // Criação da identidade.
            ClaimsIdentity identidade = new(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            // Criação do usuário autenticado.
            ClaimsPrincipal usuario = new(identidade);

            // Propriedades da autenticação.
            AuthenticationProperties propriedades = new() {
                IsPersistent = false,

                AllowRefresh = true
            };

            // Geração do cookie de autenticação.
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                usuario,
                propriedades);

            return RedirecionarUsuario();
        }

        // REDIRECIONAMENTO APÓS AUTENTICAÇÃO
        private IActionResult RedirecionarUsuario() {
            if (!string.IsNullOrWhiteSpace(ReturnUrl) &&
                Url.IsLocalUrl(ReturnUrl)) {
                return LocalRedirect(ReturnUrl);
            }

            return RedirectToPage("/Index");
        }
    }
}