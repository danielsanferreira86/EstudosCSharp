using EstudosCSharp.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EstudosCSharp.Pages.Testes {
    [Authorize(Policy = Permissoes.GerenciarUsuarios)]
    public class AdministradorModel : PageModel {
        public void OnGet() {
        }
    }
}