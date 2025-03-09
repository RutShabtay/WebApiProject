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

[Route("[controller]")]
[ApiController]
public class GoogleController : ControllerBase
{
    /// <summary>
    /// התחברות דרך Google
    /// </summary>
    [HttpGet]
    [HttpGet("Login")]
    public IActionResult Login()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = "/Html/Jobs.html" // תוודא שכתובת ה- URL הזו נכונה
        };
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet]
    [Route("[action]")]
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

        // קבלי את טוקן הגישה של גוגל
        var googleAccessToken = result.Properties.GetTokenValue("access_token");

        // צור טוקן משלך
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("YourSuperSecretKey");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
            new Claim(ClaimTypes.NameIdentifier, result.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? ""),
            new Claim(ClaimTypes.Name, result.Principal.Identity?.Name ?? ""),
            new Claim("GoogleAccessToken", googleAccessToken ?? ""), // שמור את ה-Token שלך כאן

            new Claim("type", "Admin"),
            new Claim("type", "User"),
            new Claim("type", "SuperAdmin"),
        ]),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = tokenHandler.WriteToken(token);

        return Ok(new { Token = jwtToken, GoogleAccessToken = googleAccessToken, Claims = claims });
    }

    /// <summary>
    /// החזרת פרטי המשתמש המחובר
    /// </summary>
    [HttpGet]
    [Route("[action]")]
    [Authorize]
    public IActionResult GetUserInfo()
    {
        return Ok(GetUserClaims());
    }

    /// <summary>
    /// התנתקות מהמערכת
    /// </summary>
    [HttpGet]
    [Route("[action]")]
    public IActionResult Logout()
    {
        return SignOut(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    /// <summary>
    /// פונקציה עוזרת לשליפת פרטי המשתמש
    /// </summary>
    private object GetUserClaims()
    {
        return new
        {
            name = User.Identity?.Name,
            email = User.Claims.FirstOrDefault(c => c.Type == "email")?.Value
        };
    }
}
