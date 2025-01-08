using Microsoft.AspNetCore.Mvc;
using WebApiProject.Interface;
using WebApiProject.Models;


namespace WebApiProject.services
{

    public class JobFinderService : IJobFinderService
    {

       

        private List<JobFinderAPI> JobList;
        public JobFinderService()
        {
            JobList = new List<JobFinderAPI>
        {

            new JobFinderAPI{JobId=111,Location="Jerusalem",JobFieldCategory="Programming",Sallery=19000,JobDescription="Amazing Job.",PostedDate=DateTime.Now},
            new JobFinderAPI{JobId=222,Location="Modi'n",JobFieldCategory="Programming",Sallery=20000,JobDescription="High level technologic",PostedDate=DateTime.Now}

        };
        }

        public IEnumerable<JobFinderAPI> GetAll() => JobList;
        public JobFinderAPI? Get(int id)
        {
            var specJob = JobList.FirstOrDefault(s => s.JobId == id);
            return specJob;
        }

        public void Post(JobFinderAPI newJob)
        {
            var nextId = JobList.Max(n => n.JobId);
            newJob.JobId = nextId + 1;
            JobList.Add(newJob);
        }

        public void Put(JobFinderAPI jobToUpdate, JobFinderAPI newJob)
        {
            jobToUpdate.JobId = newJob.JobId;
            jobToUpdate.Location = newJob.Location;
            jobToUpdate.JobFieldCategory = newJob.JobFieldCategory;
            jobToUpdate.Sallery = newJob.Sallery;
            jobToUpdate.JobDescription = newJob.JobDescription;
            jobToUpdate.PostedDate = newJob.PostedDate;
        }

        [HttpDelete("{id}")]
        public void Delete(JobFinderAPI jobToDelete)
        {
            JobList.Remove(jobToDelete);
        }


    }


}