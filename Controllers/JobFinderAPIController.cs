using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using WebApiProject.Models;
using WebApiProject.Controllers;
using System.Security.Cryptography.X509Certificates;


namespace WebApiProject.Controllers;

[ApiController]
[Route("[controller]")]
public class JobFinderAPIController : ControllerBase

{
    private static List<JobFinderAPI> JobList;

    static JobFinderAPIController()
    {
        JobList = new List<JobFinderAPI>
        {

            new JobFinderAPI{JobId=111,Location="Jerusalem",JobFieldCategory="Programming",Sallery=19000,JobDescription="",PostedDate=DateTime.Now},
            new JobFinderAPI{JobId=222,Location="Modi'n",JobFieldCategory="Programming",Sallery=20000,JobDescription="",PostedDate=DateTime.Now}

        };
    }

    [HttpGet]
    public IEnumerable<JobFinderAPI> Get()
    {
        return JobList;
    }

    [HttpGet("{id}")]
    public ActionResult<JobFinderAPI> Get(int id)
    {
        var specJob = JobList.FirstOrDefault(s => s.JobId == id);
        if (specJob == null)
            return BadRequest("Oooops,Invalid Id!!!");
        return specJob;
    }

    [HttpPost]
    public ActionResult Post(JobFinderAPI newJob)
    {
        var nextId = JobList.Max(n => n.JobId);
        newJob.JobId = nextId + 1;
        JobList.Add(newJob);
        return CreatedAtAction(nameof(Post), new { newJobId = newJob.JobId }, newJob);
    }

    [HttpPut("{id}")]
    public ActionResult Put(int id, JobFinderAPI newJob)
    {
        var jobToUpdate = JobList.FirstOrDefault(j => j.JobId == id);
        if (jobToUpdate == null)
            return BadRequest("Oooops,Invalid Id!!!");
        jobToUpdate.JobId = newJob.JobId;
        jobToUpdate.Location = newJob.Location;
        jobToUpdate.JobFieldCategory = newJob.JobFieldCategory;
        jobToUpdate.Sallery = newJob.Sallery;
        jobToUpdate.JobDescription = newJob.JobDescription;
        jobToUpdate.PostedDate = newJob.PostedDate;
        return NoContent();
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(int id)
    {
        var jobToDelete = JobList.FirstOrDefault(j => j.JobId == id);
        if (jobToDelete == null)
            return BadRequest("Oooops, Invalid Id!!!");

        JobList.Remove(jobToDelete);
        return Ok(jobToDelete);
    }


}

