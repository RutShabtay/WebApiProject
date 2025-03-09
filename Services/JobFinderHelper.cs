
using WebApiProject.Interface;
using Microsoft.Extensions.DependencyInjection;
using WebApiProject.Services;

namespace WebApiProject.services

{
    public static class jobFinderHelper
    {

        public static void AddJobFinderServices(this IServiceCollection services)
        {
            services.AddSingleton<IJobFinderService, JobsService>();
            services.AddSingleton<IUserFinderService, UserService>();
            services.AddSingleton<ITokenService, TokenService>();
        }
    }
}

