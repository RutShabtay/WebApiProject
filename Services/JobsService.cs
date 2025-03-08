using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using WebApiProject.Interface;
using WebApiProject.Models;
using System.Linq;
using WebApiProject.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;


namespace WebApiProject.services
{

    public class JobsService : IJobFinderService
    {

        private readonly string jsonFilePath = "./JsonFiles/Jobs.json";

        public IEnumerable<Job> GetAll()
        {
            var objectList = GeneralService.ReadFromJsonFile(jsonFilePath, "job");
            return objectList.Cast<Job>();
        }
        public Job? Get(int id)
        {
            var jobs = GeneralService.ReadFromJsonFile(jsonFilePath, "job").Cast<Job>().ToList();
            var specJob = jobs.FirstOrDefault(s => s.JobId == id);
            return specJob;
        }

        public IActionResult Post(Job newJob, string CreatedBy)
        {

            newJob.CreatedBy = CreatedBy;
            var jobs = GeneralService.ReadFromJsonFile(jsonFilePath, "job").Cast<Job>().ToList();
            var nextId = jobs.Max(n => n.JobId);
            newJob.JobId = nextId + 1;
            var WriteToJsonFileResult = GeneralService.WriteToJsonFile(jsonFilePath, newJob, "job");

            if (WriteToJsonFileResult is ObjectResult objectResult && objectResult.StatusCode == 404)
            {

                return new ObjectResult("File was not found.") { StatusCode = 404 };
            }

            return new OkObjectResult("The job was added successfully.");
        }

        public IActionResult Put(Job jobToUpdate)
        {
            var deleteResult = Delete(jobToUpdate);
            if (deleteResult is ObjectResult objectResult && objectResult.StatusCode == 400)
            {
                return new ObjectResult("Job ID not found.") { StatusCode = 400 };
            }
            return GeneralService.WriteToJsonFile(jsonFilePath, jobToUpdate, "job");
        }

        public IActionResult Delete(Job jobToDelete)
        {
            var jobs = GeneralService.ReadFromJsonFile(jsonFilePath, "job").Cast<Job>().ToList();
            bool isRemoved = jobs.Remove(jobs.FirstOrDefault(j => j.JobId == jobToDelete.JobId));
            if (!isRemoved)
            {
                return new ObjectResult("Job ID not found.") { StatusCode = 400 };
            }
            var jobsJson = JsonConvert.SerializeObject(jobs, Formatting.Indented);
            File.WriteAllText(jsonFilePath, jobsJson);
            return new ObjectResult("Jobs deleted successfully.") { StatusCode = 201 };
        }

    }

    public static class JobsServiceHelper
    {
        public static void AddJobService(this IServiceCollection services)
        {
            services.AddSingleton<IJobFinderService, JobsService>();
        }
    }

}