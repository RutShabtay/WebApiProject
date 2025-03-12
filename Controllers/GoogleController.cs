using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace WebApi.Controllers;

[Route("Google")]
[ApiController]
public class GoogleController : ControllerBase
{
    [HttpGet]
    [HttpGet("Login")]
    public IActionResult Login()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = "/Html/Users.html"
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet]
    [Route("GoogleResponse")]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded)
        {
            var failureMessage = result.Failure?.Message ?? "Unknown error";
            Console.WriteLine($"Authentication failed: {failureMessage}");
            return Unauthorized(new { error = "Authentication failed", details = failureMessage });
        }

        var claims = result.Principal.Identities
            .FirstOrDefault()?.Claims.Select(c => new { c.Type, c.Value });

        foreach (var claim in claims)
        {
            Console.WriteLine($"Claim Type: {claim.Type}, Value: {claim.Value}");
        }

        var googleAccessToken = result.Properties.GetTokenValue("access_token");

        var googleClaims = result.Principal.Identities
            .FirstOrDefault()?
            .Claims.ToDictionary(c => c.Type, c => c.Value);

        var password = googleClaims.ContainsKey(ClaimTypes.NameIdentifier) ? googleClaims[ClaimTypes.NameIdentifier] : string.Empty;
        var name = googleClaims.ContainsKey(ClaimTypes.Name) ? googleClaims[ClaimTypes.Name] : string.Empty;

        var userTypes = new[] { "Admin", "user", "SuperAdmin" };
        var random = new Random();
        var selectedType = userTypes[random.Next(userTypes.Length)]; // בוחר אקראית אחד מהערכים

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("YourSuperSecretKey");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            new[]
            {
            new Claim("password", password),
            new Claim("userName", name),
            new Claim("GoogleAccessToken", googleAccessToken ?? ""),
            new Claim("type", selectedType),
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);
        return Ok(new { Token = jwtToken, GoogleAccessToken = googleAccessToken, Claims = claims });
    }

    [HttpGet]
    [Route("[action]")]
    [Authorize]
    public IActionResult GetUserInfo()
    {
        return Ok(GetUserClaims());
    }

    [HttpGet]
    [Route("[action]")]
    public IActionResult Logout()
    {
        return SignOut(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    private object GetUserClaims()
    {
        return new
        {
            name = User.Identity?.Name,
            email = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value
        };
    }
}
