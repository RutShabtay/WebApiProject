using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApiProject.Interface;
using WebApiProject.Models;
using WebApiProject.Services;

namespace WebApiProject.services
{

    public class UserService : IUserFinderService
    {
        private readonly string jsonFilePath = "./JsonFiles/Users.json";
        public IEnumerable<User> GetAllUsers()
        {
            List<Object> objectList = GeneralService.ReadFromJsonFile(jsonFilePath, "user");
            return objectList.Cast<User>();
        }

        public User Get(string password)
        {
            var users = GeneralService.ReadFromJsonFile(jsonFilePath, "user").Cast<User>().ToList();
            var currentUser = users.FirstOrDefault(c => c.password == password);
            return currentUser;
        }

        public ActionResult post(User newUser)
        {
            GeneralService.WriteToJsonFile(jsonFilePath, newUser, "user");
            return new OkObjectResult("User added successfully");
        }

        public ActionResult Put(User userToUpdate, string password)
        {
            var deleteResult = Delete(password);
            if (deleteResult is ObjectResult objectResult && objectResult.StatusCode == 400)
            {
                return new ObjectResult("User Password not found.") { StatusCode = 400 };
            }

            return GeneralService.WriteToJsonFile(jsonFilePath, userToUpdate, "user");
        }

        public IActionResult Delete(string password)
        {

            var users = GeneralService.ReadFromJsonFile(jsonFilePath, "user").Cast<User>().ToList();
            var userToDelete = users.Find(u => u.password == password);
            if (userToDelete == null)
                return new ObjectResult("User ID not found in token") { StatusCode = 400 };
            users.Remove(userToDelete);
            var userJson = JsonConvert.SerializeObject(users, Formatting.Indented);
            File.WriteAllText(jsonFilePath, userJson);
            return new ObjectResult("User Deleted successfully") { StatusCode = 201 };
        }

        public object GetAll()
        {
            throw new NotImplementedException();
        }
    }

    public static class UsersServiceHelper
    {
        public static void AddUserService(this IServiceCollection services)
        {
            services.AddSingleton<IUserFinderService, UserService>();
        }
    }

}