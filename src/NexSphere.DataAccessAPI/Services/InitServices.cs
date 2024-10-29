using Fanex.Data.Repository;
using NexSphere.Core.Services;
using NexSphere.Data.Features.UserAgent;

namespace NexSphere.DataAccessAPI.Services
{
    public static class InitServices
    {
        public static void RegisterService(this IServiceCollection services)
        {
            services.AddSingleton<IDynamicRepository, DynamicRepository>();
            services.AddSingleton<IJwtTokenService, JwtTokenService>();

            RegisterUserAgentService(services);    
        }

        private static void RegisterUserAgentService(this IServiceCollection services)
        {
            services.AddSingleton<IUserAgentDataAccess, UserAgentDataAccess>();
            services.AddSingleton<IUserAgentServices, UserAgentServices>();
        }

    }
}
