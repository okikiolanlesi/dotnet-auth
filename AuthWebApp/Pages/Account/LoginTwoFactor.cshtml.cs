using System.ComponentModel.DataAnnotations;
using AuthWebApp.Data.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthWebApp.Pages.Account
{
    public class LoginTwoFactorModel : PageModel
    {
        private readonly SignInManager<User> signInManager;

        public LoginTwoFactorModel(SignInManager<User> signInManager)
        {
            this.signInManager = signInManager;
        }
        [BindProperty]
        public LoginTwoFactorViewModel LoginTwoFactorViewModel { get; set; } = new LoginTwoFactorViewModel();

        public void OnGet(bool RememberMe = false)
        {

            LoginTwoFactorViewModel.RememberMe = RememberMe;
            LoginTwoFactorViewModel.SecurityCode = string.Empty;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            var result = await signInManager.TwoFactorAuthenticatorSignInAsync(LoginTwoFactorViewModel.SecurityCode, LoginTwoFactorViewModel.RememberMe, false);

            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            else if (result.IsLockedOut)
            {
                ModelState.AddModelError("Login", "Your account is locked out");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid security code");
            }
            return Page();

        }
    }

    public class LoginTwoFactorViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string SecurityCode { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
}
