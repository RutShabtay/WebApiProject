using Microsoft.AspNetCore.Mvc;
using WebApiProject.Controllers;
using WebApiProject.Models;

namespace WebApiProject.Interface
{

    public interface IUserFinderService
    {
        public IEnumerable<User> GetAllUsers();
        User Get(string userId);
        ActionResult post(User newUser);
        ActionResult Put(User userToUpdate,string password);
        IActionResult Delete(string userId);
        object GetAll();
    }
}



