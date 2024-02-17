using AuthWebApp.Data.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyApp.Namespace
{

    public class UserProfileModel : PageModel
    {

        [BindProperty]
        public UserProfileViewModel UserProfile { get; set; } = new UserProfileViewModel();

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

            UserProfile.Email = user?.Email ?? string.Empty;
            UserProfile.Department = claims.FirstOrDefault(c => c.Type == "Department")?.Value ?? string.Empty;
            UserProfile.Position = claims.FirstOrDefault(c => c.Type == "Position")?.Value ?? string.Empty;

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
