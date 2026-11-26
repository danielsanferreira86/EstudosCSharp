using System.Security.Claims;
using EstudosCSharp.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace EstudosCSharp.Security {
    public class ValidacaoCookie : CookieAuthenticationEvents {
        private readonly UsuarioRepository _usuarioRepository;

        public ValidacaoCookie(
            UsuarioRepository usuarioRepository) {
            _usuarioRepository = usuarioRepository;
        }

        public override async Task ValidatePrincipal(
            CookieValidatePrincipalContext context) {
            // Recupera o ID armazenado no cookie.
            string? idUsuario = context.Principal?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Valida o ID.
            if (!int.TryParse(idUsuario, out int id)) {
                await InvalidarCookieAsync(context);

                return;
            }

            // Consulta o usuário no banco de dados.
            var usuario = await _usuarioRepository
                .BuscarPorIdAsync(id);

            // Verifica se o usuário existe e está ativo.
            if (usuario == null || !usuario.Ativo) {
                await InvalidarCookieAsync(context);

                return;
            }

            // Verifica se o perfil é válido.
            if (usuario.Tipo_Usuario != Perfis.Administrador &&
                usuario.Tipo_Usuario != Perfis.Padrao) {
                await InvalidarCookieAsync(context);

                return;
            }

            // Recupera os dados armazenados no cookie.
            string? perfilCookie = context.Principal?
                .FindFirst(ClaimTypes.Role)?.Value;

            string? nomeCookie = context.Principal?
                .FindFirst(ClaimTypes.Name)?.Value;

            string? loginCookie = context.Principal?
                .FindFirst("Login")?.Value;

            // Verifica se os dados foram alterados.
            if (perfilCookie != usuario.Tipo_Usuario ||
                nomeCookie != usuario.Nome ||
                loginCookie != usuario.Login) {
                await InvalidarCookieAsync(context);
            }
        }

        private static async Task InvalidarCookieAsync(
            CookieValidatePrincipalContext context) {
            // Rejeita a identidade autenticada.
            context.RejectPrincipal();

            // Remove o cookie de autenticação.
            await context.HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}