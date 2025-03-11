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
    /// <summary>
    /// התחברות דרך Google
    /// </summary>
    [HttpGet]
    [HttpGet("Login")]
    public IActionResult Login()
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = "/Html/Users.html" // תוודא שכתובת ה- URL הזו נכונה
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

        // קבלי את טוקן הגישה של גוגל
        var googleAccessToken = result.Properties.GetTokenValue("access_token");

        // כאן מבוצע מיפוי השדות
        var googleClaims = result.Principal.Identities
            .FirstOrDefault()?
            .Claims.ToDictionary(c => c.Type, c => c.Value);

        // מיפוי של השדות מגוגל לשדות הנדרשים בטוקן שלך
        var password = googleClaims.ContainsKey(ClaimTypes.NameIdentifier) ? googleClaims[ClaimTypes.NameIdentifier] : string.Empty;
        var name = googleClaims.ContainsKey(ClaimTypes.Name) ? googleClaims[ClaimTypes.Name] : string.Empty;

        // אקראי: הגרלת סוג משתמש מתוך שלושה אפשרויות
        var userTypes = new[] { "Admin", "user", "SuperAdmin" };
        var random = new Random();
        var selectedType = userTypes[random.Next(userTypes.Length)]; // בוחר אקראית אחד מהערכים

        // צור טוקן משלך
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes("YourSuperSecretKey");
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            new[]
            {
            new Claim("password", password),
            new Claim("userName", name), // כאן אנחנו מכניסים את ה-email ממיפוי השדות
            new Claim("GoogleAccessToken", googleAccessToken ?? ""), // שמור את ה-Token שלך כאן
            new Claim("type", selectedType) // סוג המשתמש שנבחר אקראית
            }),
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
