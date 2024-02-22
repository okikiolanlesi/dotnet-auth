using System.Security.Claims;
using AuthWebApp.Data.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AuthWebApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{

  private readonly SignInManager<User> _signInManager;
  private readonly UserManager<User> _userManager;
  public AccountController(SignInManager<User> signInManager, UserManager<User> userManager)
  {
    this._signInManager = signInManager;
    this._userManager = userManager;
  }

  // This will be called when the user clicks on the Google button and redirected to the Google login page and then redirected back to the ExternalLoginCallback action method
  public async Task<IActionResult> ExternalLoginCallback()
  {

    //loginInfo will contain the email and other information of the user which is returned from Google and identity framework will use this information to create a user in the database if the user does not exist in the database and then sign in the user and redirect to the home page of the application

    // The auth middleware will get the user info from google and add it to the HttpContext.User.Identity claims collection
    var loginInfo = await _signInManager.GetExternalLoginInfoAsync();

    if (loginInfo != null)
    {
      var emailClaim = loginInfo.Principal.FindFirstValue(ClaimTypes.Email);
      var userClaim = loginInfo.Principal.FindFirstValue(ClaimTypes.Name);

      if (emailClaim != null && userClaim != null)
      {
        var user = await _userManager.FindByEmailAsync(emailClaim);
        if (user == null)
        {
          user = new User
          {
            UserName = emailClaim,
            Email = emailClaim
          };
          var result = await _userManager.CreateAsync(user);
          if (result.Succeeded)
          {
            result = await _userManager.AddLoginAsync(user, loginInfo);
            if (result.Succeeded)
            {
              await _signInManager.SignInAsync(user, isPersistent: false);
              return Redirect("/");
            }
          }
        }
        else
        {
          await _signInManager.SignInAsync(user, isPersistent: false);
          return Redirect("/");
        }

      }
    }
    return RedirectToPage("/Index");

  }
}
