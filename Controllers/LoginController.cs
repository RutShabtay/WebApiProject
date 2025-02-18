using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApiProject.Models;
using WebApiProject.Services;

namespace WebApiProject.Controllers
{

    public class LoginController : ControllerBase
    {
        [HttpPost]
        [Route("[action]")]
        public ActionResult<String> Login([FromBody] User User)
        {
            var dt = DateTime.Now;

            if (User.userName != "Wray"
            || User.userId != $"W{dt.Year}#{dt.Day}!")
            {
                return Unauthorized();
            }

            var claims = new List<Claim>
            {
                new Claim("role", "superAdmin"),
                new Claim("userId",$"W{dt.Year}#{dt.Day}!")
            };

            var token = JobFinderTokenService.GetToken(claims);

            return new OkObjectResult(JobFinderTokenService.WriteToken(token));
        }
    }
}