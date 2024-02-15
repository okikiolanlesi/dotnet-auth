using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AuthWebApi.Controllers;




[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration configuration;

    public AuthController(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    [HttpPost]
    public IActionResult Authenticate([FromBody] Credential credential)
    {
        if (credential.Username == "admin" && credential.Password == "password")
        {
            // Creating the security context
            var claims = new List<Claim>{
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin@admin.com"),
                    new Claim("Department", "HR"),
                    new Claim("EmploymentDate", "2024-01-01")
                };

            var expiryTime = DateTime.Now.AddMinutes(10);

            var payload = new
            {
                access_token = CreateToken(claims, expiryTime),
                expires_at = expiryTime
            };

            return Ok(payload);

        }

        ModelState.AddModelError("Unauthorized", "Invalid username or password");

        return Unauthorized(ModelState);


    }

    private string CreateToken(IEnumerable<Claim> claims, DateTime expireAt)
    {
        var token = new JwtSecurityToken(

            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expireAt,
            signingCredentials: new Microsoft.IdentityModel.Tokens.SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("SecretKey") ?? "")), SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);

    }
}

public class Credential
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

