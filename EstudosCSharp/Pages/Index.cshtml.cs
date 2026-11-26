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

        public string TipoUsuario { get; private set; }
            = string.Empty;

        public string ID_Usuario { get; private set; }
            = string.Empty;

        public void OnGet() {
            NomeUsuario =
                User.Identity?.Name ?? string.Empty;

            LoginUsuario =
                User.FindFirstValue("Login") ?? string.Empty;

            TipoUsuario =
                User.FindFirstValue(ClaimTypes.Role)
                ?? string.Empty;

            ID_Usuario =
                User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? string.Empty;
        }
    }
}