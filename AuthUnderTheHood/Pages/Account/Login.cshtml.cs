using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthUnderTheHood.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Credential Credential { get; set; } = new Credential();
        public bool RememberMe { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            if (Credential.Username == "admin" && Credential.Password == "password")
            {
                // Creating the security context
                var claims = new List<Claim>{
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@admin.com"),
                    new Claim("Department", "HR"),
                    new Claim("EmploymentDate", "2024-01-01")
                };

                var identity = new ClaimsIdentity(claims, "Cookie");
                // var identityTest = new ClaimsIdentity(claims, "Bearer");
                ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(identity);
                // ClaimsPrincipal claimsPrincipalTest = new ClaimsPrincipal(identityTest);

                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = Credential.RememberMe
                };

                await HttpContext.SignInAsync("Cookie", claimsPrincipal, authProperties);
                // await HttpContext.SignInAsync("Bearer", claimsPrincipalTest);

                return RedirectToPage("/Index");
            }

            return Page();
        }
    }

    public class Credential
    {
        [Required]
        [Display(Name = "User Name")]
        public string Username { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

    }
}
