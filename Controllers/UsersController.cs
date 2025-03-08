using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using WebApiProject.Interface;
using WebApiProject.Models;
using WebApiProject.services;
using WebApiProject.Services;


namespace WebApiProject.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private IUserFinderService userFinderSrvice;
    public UsersController(IUserFinderService userFinderSrvice)
    {
        this.userFinderSrvice = userFinderSrvice;
    }

    [HttpGet("GetAllUsers")]
    [Authorize(Policy = "SuperAdmin")]
    public IActionResult GetAllUsers()
    {
        var users = userFinderSrvice.GetAllUsers();
        Console.WriteLine(users);
        if (users != null)
            return Ok(users);
        return StatusCode(500, "Data reading failed while retrieving users.");
    }

    [HttpGet]
    public IActionResult Get()
    {
        ObjectResult passObj = (ObjectResult)GeneralService.GetUserPasswordFromToken(HttpContext);
        var password = passObj.Value as string;
        var currentUser = userFinderSrvice.Get(password);
        if (currentUser == null)
            return StatusCode(500, "Data reading failed while retrieving users.");
        return Ok(currentUser);
    }

    [HttpPost]
    [Authorize(Policy = "SuperAdmin")]
    public IActionResult Post([FromBody] User newUser)
    {
        return userFinderSrvice.post(newUser);
    }

    [HttpPut("{password}")]
    public IActionResult Put(User userToUpdate, string password)
    {
        ObjectResult typeObj = (ObjectResult)GeneralService.GetUserTypeFromToken(HttpContext);
        var type = typeObj.Value as string;
        ObjectResult passObj = (ObjectResult)GeneralService.GetUserPasswordFromToken(HttpContext);
        var currentPassword = passObj.Value as string;
        if (type.Equals("SuperAdmin") || currentPassword.Equals(password))
        {
            return userFinderSrvice.Put(userToUpdate, password);
        }
        return Forbid("You do not have permission to modify this user.");
    }


    [HttpDelete("{password}")]
    public IActionResult Delete(string password)
    {
        ObjectResult typeObj = (ObjectResult)GeneralService.GetUserTypeFromToken(HttpContext);
        var type = typeObj.Value as string;
        ObjectResult passObj = (ObjectResult)GeneralService.GetUserPasswordFromToken(HttpContext);
        var uPassword = passObj.Value as string;
        if (type.Equals("SuperAdmin") || uPassword.Equals(password))
        {
            return userFinderSrvice.Delete(password);
        }
        return Forbid("You do not have permission to modify this job.");
    }
}