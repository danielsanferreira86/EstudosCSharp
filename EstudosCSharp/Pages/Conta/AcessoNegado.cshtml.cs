using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EstudosCSharp.Pages.Conta {
    [Authorize]
    public class AcessoNegadoModel : PageModel {
        public void OnGet() {
            Response.StatusCode =
                StatusCodes.Status403Forbidden;
        }
    }
}