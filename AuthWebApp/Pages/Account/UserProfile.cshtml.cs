using System.Security.Claims;
using AuthWebApp.Data.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthWebApp.Pages.Account
{

    public class UserProfileModel : PageModel
    {

        [BindProperty]
        public UserProfileViewModel UserProfileViewModel { get; set; } = new UserProfileViewModel();

        [BindProperty]
        public string? SuccessMessage { get; set; }

        private UserManager<User> userManager;

        public UserProfileModel(UserManager<User> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);

            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var claims = await userManager.GetClaimsAsync(user);

            UserProfileViewModel.Email = user?.Email ?? string.Empty;
            UserProfileViewModel.Department = claims.FirstOrDefault(c => c.Type == "Department")?.Value ?? string.Empty;
            UserProfileViewModel.Position = claims.FirstOrDefault(c => c.Type == "Position")?.Value ?? string.Empty;

            return Page();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await userManager.FindByNameAsync(User.Identity?.Name ?? string.Empty);

            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var claims = await userManager.GetClaimsAsync(user);

            var departmentClaim = claims.FirstOrDefault(c => c.Type == "Department");
            var positionClaim = claims.FirstOrDefault(c => c.Type == "Position");

            if (departmentClaim != null)
            {
                await userManager.RemoveClaimAsync(user, departmentClaim);
            }

            if (positionClaim != null)
            {
                await userManager.RemoveClaimAsync(user, positionClaim);
            }

            await userManager.AddClaimAsync(user, new Claim("Department", UserProfileViewModel.Department));
            await userManager.AddClaimAsync(user, new Claim("Position", UserProfileViewModel.Position));

            this.SuccessMessage = "User profile updated successfully";

            return Page();
        }
    }

    public class UserProfileViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
    }
}
