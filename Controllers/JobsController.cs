using Microsoft.AspNetCore.Mvc;
using WebApiProject.Models;
using WebApiProject.Interface;
using Microsoft.AspNetCore.Authorization;
using WebApiProject.Services;

namespace WebApiProject.Controllers
{


    [ApiController]
    [Route("[controller]")]
    public class JobsController : ControllerBase

    {
        private IJobFinderService JobFinderService;
        public JobsController(IJobFinderService JobFinderService)
        {
            this.JobFinderService = JobFinderService;
        }

        //Get all Jobs.
        [HttpGet]
        public IActionResult GetAll()
        {
            var jobs = JobFinderService.GetAll();
            if (jobs != null)
                return Ok(jobs);
            return StatusCode(500, "Data reading failed while retrieving users.");
        }

        //Get specific Job from all Jobs.
        [HttpGet("{id}")]
        public ActionResult<Job> Get(int id)
        {
            var specJob = JobFinderService.Get(id);
            if (specJob == null)
            {
                return BadRequest("Oooops,Invalid Id---");
            }
            return specJob;
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public ActionResult Post(Job newJob)
        {
            ObjectResult passwordObj = (ObjectResult)GeneralService.GetUserPasswordFromToken(HttpContext);
            var CreatedBy = passwordObj.Value as string;
            JobFinderService.Post(newJob, CreatedBy);
            return CreatedAtAction(nameof(Post), new { newJobId = newJob.JobId }, newJob);
        }


        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult Put(int id, Job newJob)
        {
            ObjectResult type1 = (ObjectResult)GeneralService.GetUserTypeFromToken(HttpContext);
            var type = type1.Value as string;
            ObjectResult password1 = (ObjectResult)GeneralService.GetUserPasswordFromToken(HttpContext);
            var password = password1.Value as String;
            Console.WriteLine(type);
            if (type.Equals("SuperAdmin") || (type.Equals("Admin") && JobFinderService.Get(id).CreatedBy.Equals(password)))
            {
                if (newJob == null || id != newJob.JobId)
                    return BadRequest("Not Valid Id---");
                var jobToUpdate = JobFinderService.Get(id);
                if (jobToUpdate == null)
                    return BadRequest("Oooops,Invalid JobId---");
                return JobFinderService.Put(newJob);
            }
            return Forbid("You do not have permission to modify this job.");
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult Delete(int id)
        {
            ObjectResult role = (ObjectResult)GeneralService.GetUserTypeFromToken(HttpContext);
            var type = role.Value as string;
            ObjectResult passObj = (ObjectResult)GeneralService.GetUserPasswordFromToken(HttpContext);
            var password = passObj.Value as string;

            if (type.Equals("SuperAdmin") || (type.Equals("Admin") && JobFinderService.Get(id).CreatedBy.Equals(password)))
            {
                var jobToDelete = JobFinderService.Get(id);
                if (jobToDelete == null)
                    return BadRequest("Oooops, Invalid jobId---");

                return JobFinderService.Delete(jobToDelete);
            }
            return Forbid("You do not have permission to delete this job.");
        }
    }

}
