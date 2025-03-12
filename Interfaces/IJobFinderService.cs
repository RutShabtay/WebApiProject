using Microsoft.AspNetCore.Mvc;
using WebApiProject.Models;

namespace WebApiProject.Interface
{

    public interface IJobFinderService
    {
        IEnumerable<Job> GetAll();
        Job? Get(int id);
        IActionResult Post(Job newJob, string CreatedBy);
        IActionResult Put(Job jobToUpdate);
        IActionResult Delete(Job jobToDelete);
    }
}
