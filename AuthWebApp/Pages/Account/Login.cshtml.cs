using AuthWebApp.Data.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthWebApp.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public CredentialViewModel Credential { get; set; } = new CredentialViewModel();

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public LoginModel(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Credential.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, Credential.Password, Credential.RememberMe, false);
                    if (result.Succeeded)
                    {
                        return RedirectToPage("/Index");
                    }
                    else if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("/Account/LoginTwoFactor", new { Credential.RememberMe, ReturnUrl = "/Index" });
                    }
                    else if (result.IsLockedOut)
                    {
                        ModelState.AddModelError("Login", "Your account is locked out");
                    }

                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return Page();
        }

        public class CredentialViewModel
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
            public bool RememberMe { get; set; }
        }
    }
}
