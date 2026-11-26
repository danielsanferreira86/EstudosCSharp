using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EstudosCSharp.Pages {
    [Authorize]
    public class IndexModel : PageModel {
        public string NomeUsuario { get; private set; }
            = string.Empty;

        public string LoginUsuario { get; private set; }
            = string.Empty;

        public string PerfilUsuario { get; private set; }
            = string.Empty;

        public void OnGet() {
            NomeUsuario =
                User.Identity?.Name ?? string.Empty;

            LoginUsuario =
                User.FindFirst("Login")?.Value
                ?? string.Empty;

            PerfilUsuario =
                User.FindFirst(ClaimTypes.Role)?.Value
                ?? string.Empty;
        }
    }
}