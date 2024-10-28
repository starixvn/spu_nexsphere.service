namespace NexSphere.Service.API
{
    using Fanex.Logging;
    using NexSphere.Core.Middlewares;
    using NexSphere.Core.Services;

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            services.AddSingleton<IJwtTokenService, JwtTokenService>();
            services.AddSingleton<ILogger, Logger>();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            

            app.UseMiddleware<JWTValidationMiddleware>();

            
        }

        
    }
}
