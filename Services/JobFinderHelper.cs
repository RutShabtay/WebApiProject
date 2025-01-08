
using WebApiProject.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace WebApiProject.services

{
    public static class jobFinderHelper
    {

        public static void AddJobFinderServices(this IServiceCollection services)
        {
            services.AddSingleton<IJobFinderService, JobFinderService>();

        }
    }
}

