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
        if (users != null)
            return Ok(users);
        return StatusCode(500, "Data reading failed while retrieving users.");
    }

    [HttpGet]
    public IActionResult Get()
    {
        var userId = GeneralService.GetUserIdFromToken(HttpContext).ToString();
        var currentUser = userFinderSrvice.Get(userId);
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

    [HttpPut]
    public IActionResult Put(User userToUpdate)
    {
        var role = GeneralService.GetUserRoleFromToken(HttpContext).ToString();
        if (role.Equals("superAdmin") || GeneralService.GetUserIdFromToken(HttpContext).ToString().Equals(userToUpdate.userId))
        {
            return userFinderSrvice.Put(userToUpdate);
        }
        return Forbid("You do not have permission to modify this job.");
    }


    [HttpDelete]
    public IActionResult Delete(string userId)
    {
        var role = GeneralService.GetUserRoleFromToken(HttpContext).ToString();
        if (role.Equals("superAdmin") || GeneralService.GetUserIdFromToken(HttpContext).ToString().Equals(userId))
        {
            return userFinderSrvice.Delete(userId);
        }
        return Forbid("You do not have permission to modify this job.");
    }

}