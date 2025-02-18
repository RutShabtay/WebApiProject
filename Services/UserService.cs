using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApiProject.Interface;
using WebApiProject.Models;
using WebApiProject.Services;

namespace WebApiProject.services
{

    public class UserService : IUserFinderService
    {
        private readonly string jsonFilePath = "./JsonFile/Users.json";

        public IEnumerable<User> GetAllUsers()
        {
            List<Object> objectList = GeneralService.ReadFromJsonFile(jsonFilePath, "user");
            return objectList.Cast<User>();
        }

        public User Get(string userId)
        {
            var users = GeneralService.ReadFromJsonFile(jsonFilePath, "user").Cast<User>().ToList();
            var currentUser = users.Find(c => c.userId == userId);
            return currentUser;
        }


        public ActionResult post(User newUser)
        {
            var claims = new List<Claim>
            {
                new Claim("role", "GeneralUser"),
                new Claim("permission", newUser.permission.ToString()),
                new Claim("userId",newUser.userId.ToString())
            };

            var token = JobFinderTokenService.GetToken(claims);
            GeneralService.WriteToJsonFile(jsonFilePath, newUser);
            return new OkObjectResult(JobFinderTokenService.WriteToken(token));
        }

        public ActionResult Put(User userToUpdate)
        {
            var deleteResult = Delete(userToUpdate.userId);
            if (deleteResult is ObjectResult objectResult && objectResult.StatusCode == 400)
            {
                return new ObjectResult("User ID not found.") { StatusCode = 400 };
            }

            return GeneralService.WriteToJsonFile(jsonFilePath, userToUpdate);
        }

        public IActionResult Delete(string userId)
        {
            var users = GeneralService.ReadFromJsonFile(jsonFilePath, "user").Cast<User>().ToList();
            var userToDelete = users.Find(u => u.userId == userId);
            if (userToDelete == null)
                return new ObjectResult("User ID not found in token") { StatusCode = 400 };
            users.Remove(userToDelete);
            var userJson = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(jsonFilePath, userJson);
            return new ObjectResult("User added successfully") { StatusCode = 201 };
        }

    }
}