using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EstudosCSharp.Pages.Conta {
    [Authorize]
    public class LogoutModel : PageModel {
        public IActionResult OnGet() {
            return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostAsync() {
            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToPage("/Conta/Login");
        }
    }
}