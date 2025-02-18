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
        [Authorize(Policy = "Management")]
        public ActionResult Post(Job newJob)
        {
            //The Id of the adding user
            var CreatedBy = GeneralService.GetUserIdFromToken(HttpContext).ToString();
            JobFinderService.Post(newJob, CreatedBy);
            return CreatedAtAction(nameof(Post), new { newJobId = newJob.JobId }, newJob);
        }

        [HttpPut]
        [Authorize(Policy = "Management")]

        public IActionResult Put(int jobId, Job newJob)
        {
            var role = GeneralService.GetUserRoleFromToken(HttpContext).ToString;
            if (role.Equals("superAdmin") || (role.Equals("Admin") && JobFinderService.Get(jobId).CreatedBy.Equals(GeneralService.GetUserIdFromToken(HttpContext).ToString())))
            {
                if (jobId != newJob.JobId)
                    return BadRequest("Not Valid Id---");
                var jobToUpdate = JobFinderService.Get(jobId);
                if (jobToUpdate == null)
                    return BadRequest("Oooops,Invalid JobId---");
                return JobFinderService.Put(newJob);
            }
            return Forbid("You do not have permission to modify this job.");
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Management")]
        public IActionResult Delete(int jobId)
        {
            var role = GeneralService.GetUserRoleFromToken(HttpContext).ToString();
            if (role.Equals("superAdmin") || (role.Equals("Admin") && JobFinderService.Get(jobId).CreatedBy.Equals(GeneralService.GetUserIdFromToken(HttpContext).ToString())))
            {
                var jobToDelete = JobFinderService.Get(jobId);
                if (jobToDelete == null)
                    return BadRequest("Oooops, Invalid jobId---");

                return JobFinderService.Delete(jobToDelete);
            }
            return Forbid("You do not have permission to delete this job.");
        }
    }

}
