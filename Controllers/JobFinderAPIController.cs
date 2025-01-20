using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using WebApiProject.Models;
using WebApiProject.Controllers;
using System.Security.Cryptography.X509Certificates;
using WebApiProject.services;
using WebApiProject.Interface;


namespace WebApiProject.Controllers;

[ApiController]
[Route("[controller]")]
public class JobFinderAPIController : ControllerBase

{
    private IJobFinderService JobFinderService;
    public JobFinderAPIController(IJobFinderService JobFinderService)
    {
        this.JobFinderService = JobFinderService;
    }


    [HttpGet]
    public IEnumerable<JobFinderAPI> GetAll()
    {
        return JobFinderService.GetAll();

    }

    [HttpGet("{id}")]
    public ActionResult<JobFinderAPI> Get(int id)
    {
        var specJob = JobFinderService.Get(id);
        if (specJob == null)
        {
            return BadRequest("Oooops,Invalid Id!!!");
        }
        return specJob;
    }

    [HttpPost]
    public ActionResult Post(JobFinderAPI newJob)
    {
        JobFinderService.Post(newJob);
        return CreatedAtAction(nameof(Post), new { newJobId = newJob.JobId }, newJob);
    }

    [HttpPut("{id}")]
    public ActionResult Put(int id, JobFinderAPI newJob)
    {
        if (id != newJob.JobId)
            return BadRequest("Not Valid Id!!!");
        var jobToUpdate = JobFinderService.Get(id);
        if (jobToUpdate == null)
            return BadRequest("Oooops,Invalid Id!!!");
        JobFinderService.Put(jobToUpdate, newJob);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var jobToDelete = JobFinderService.Get(id);
        if (jobToDelete == null)
            return BadRequest("Oooops, Invalid Id!!!");

        JobFinderService.Delete(jobToDelete);
        return Ok(jobToDelete);


    }


}

