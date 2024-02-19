using System.ComponentModel.DataAnnotations;
using AuthWebApp.Data.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;

namespace AuthWebApp.Pages.Account
{
    [Authorize]
    public class AuthenticatorWithMFASetupModel : PageModel
    {
        private readonly UserManager<User> userManager;

        [BindProperty]
        public SetupMFAViewModel SetupMFAViewModel { get; set; } = new SetupMFAViewModel();
        [BindProperty]
        public bool Succeeded { get; set; }

        public AuthenticatorWithMFASetupModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {

            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                var key = await userManager.GetAuthenticatorKeyAsync(user);
                if (string.IsNullOrEmpty(key))
                {
                    await userManager.ResetAuthenticatorKeyAsync(user);
                    key = await userManager.GetAuthenticatorKeyAsync(user);
                }
                SetupMFAViewModel.Key = key ?? string.Empty;
                SetupMFAViewModel.QRCodeBytes = GenerateQRCodeBytes("Test Auth App", SetupMFAViewModel.Key, user.Email ?? string.Empty);

                System.Console.WriteLine(SetupMFAViewModel.QRCodeBytes.Length + " bytes========================>");
            }
            return Page();
        }
        public async Task<IActionResult> OnPostResetKey()
        {

            var user = await userManager.GetUserAsync(User);

            if (user != null)
            {

                await userManager.ResetAuthenticatorKeyAsync(user);
                var key = await userManager.GetAuthenticatorKeyAsync(user);

                SetupMFAViewModel.Key = key ?? string.Empty;
                SetupMFAViewModel.QRCodeBytes = GenerateQRCodeBytes("Test Auth App", SetupMFAViewModel.Key, user.Email ?? string.Empty);

                System.Console.WriteLine(SetupMFAViewModel.QRCodeBytes.Length + " bytes========================>");
            }
            return RedirectToPage("/Account/AuthenticatorWithMFASetup");
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            var user = await userManager.GetUserAsync(User);
            if (user != null && await userManager.VerifyTwoFactorTokenAsync(user, userManager.Options.Tokens.AuthenticatorTokenProvider, SetupMFAViewModel.SecurityCode))
            {
                await userManager.SetTwoFactorEnabledAsync(user, true);
                this.Succeeded = true;
            }
            else
            {
                ModelState.AddModelError("SetupMFAViewModel.SecurityCode", "Invalid security code");
            }
            return Page();
        }

        private Byte[] GenerateQRCodeBytes(string provider, string key, string email)
        {
            var qrCodeGenerator = new QRCodeGenerator();

            var qrCodeData = qrCodeGenerator.CreateQrCode($"otpauth://totp/{provider}:{email}?secret={key}&issuer={provider}", QRCodeGenerator.ECCLevel.Q);

            var qrCode = new PngByteQRCode(qrCodeData);

            return qrCode.GetGraphic(20);

        }
    }

    public class SetupMFAViewModel
    {
        public string Key { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Code")]
        public string SecurityCode { get; set; } = string.Empty;

        public Byte[]? QRCodeBytes { get; set; }
    }
}
