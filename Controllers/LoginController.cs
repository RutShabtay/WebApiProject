using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApiProject.Interface;
using WebApiProject.Models;
using WebApiProject.Services;

namespace WebApiProject.Controllers
{
    public class LoginController : ControllerBase
    {

        private ITokenService tokenService;
        public LoginController(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        private readonly string jsonFilePath = "./JsonFiles/Users.json";

        [HttpPost]
        [Route("[action]")]

        public ActionResult<String> Login([FromBody] User User)
        {
            var claims = new List<Claim>();
            var users = GeneralService.ReadFromJsonFile(jsonFilePath, "user").Cast<User>().ToList();
            var currentUser = users.Find(c => c.password == User.password);
            if (currentUser == null || !currentUser.userName.Equals(User.userName))
            {
                return Unauthorized();
            }
            else
            {
                claims.Add(new Claim("type", currentUser.permission));

            }

            claims.Add(new Claim("password", User.password));
            claims.Add(new Claim("userName", User.userName));
            var token = tokenService.GetToken(claims);
            return new OkObjectResult(tokenService.WriteToken(token));
        }
    }
}