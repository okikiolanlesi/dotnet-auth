using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using AuthWebApp.Data.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthWebApp.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public RegisterModel(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [BindProperty]
        public RegisterViewModel RegisterViewModel { get; set; } = new RegisterViewModel();
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Validate Email Address(optional because we already configured this in program.cs)

            //Create the user
            var user = new User
            {
                UserName = RegisterViewModel.Email,
                Email = RegisterViewModel.Email,
                Department = RegisterViewModel.Department,
                Position = RegisterViewModel.Position
            };

            // Add the department and position as claims to the user just for the sake of this example. In a real-world scenario, you would add claims that are relevant to your application
            var claims = new Claim[]{
                    new Claim("Department", RegisterViewModel.Department),
                    new Claim("Position", RegisterViewModel.Position)
                };

            await userManager.AddClaimsAsync(user, claims);

            var result = await userManager.CreateAsync(user, RegisterViewModel.Password);

            if (result.Succeeded)
            {
                var confirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(user);
                string? url = Url.PageLink(pageName: "/Account/ConfirmEmail", values: new { email = RegisterViewModel.Email, token = confirmationToken ?? "", userId = user.Id });

                // Ideally, you would send an email to the user with the confirmation link but for this example, we will just redirect to the confirmation page with the token and user id in the query string 
                return Redirect(url ?? "");
            }
            else
            {
                result.Errors.ToList().ForEach(error => ModelState.AddModelError("Register", error.Description));

                return Page();
            }

        }
    }


    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Department { get; set; } = string.Empty;
        [Required]
        public string Position { get; set; } = string.Empty;
    }
}

