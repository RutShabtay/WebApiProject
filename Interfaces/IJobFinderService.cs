using Microsoft.AspNetCore.Mvc;
using WebApiProject.Models;

namespace WebApiProject.Interface
{

    public interface IJobFinderService
    {
        IEnumerable<JobFinderAPI> GetAll();

        JobFinderAPI? Get(int id);

        void Post(JobFinderAPI newJob);

        void Delete(JobFinderAPI jobToDelete);
        void Put(JobFinderAPI jobToUpdate, JobFinderAPI newJob);

    }
}
