namespace NexSphere.DataAccessAPI
{
    using Fanex.Data;
    using Fanex.Data.MySql;
    using Fanex.Logging;
    using Fanex.Logging.Sentry.RavenClient;
    using Microsoft.Extensions.Configuration;
    using NexSphere.Core;
    using NexSphere.DataAccessAPI.Middlewares;
    using NexSphere.DataAccessAPI.Services;
    using SharpRaven;
    
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
            var appSettings = new AppSettings(this.Configuration);

            services.AddSingleton<IAppSettings>(appSettings);
            services.AddSingleton<ILogger, Logger>();

            services.AddControllers();
            services.AddEndpointsApiExplorer();

            #region Swagger Configuration
            services.AddSwaggerGen();
            #endregion

            services.AddCors(options =>
            {
                options.AddPolicy("AllowOnlyGateway",
                    policy =>
                    {
                        policy.WithOrigins("http://localhost:5000")
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
            });

            services.AddHttpClient();
            services.AddHttpContextAccessor();

            services.RegisterService();

            FanexDataConfig(appSettings);
            RegisterLogging(appSettings);

            LogService.Info(string.Format("DataAccess API started at {0}", DateTimeOffset.Now));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NexSphere.DataAccessAPI v1");
                });

            }

            app.UseRouting();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseMiddleware<JWTValidationMiddleware>();
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseCors("AllowOnlyGateway");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }

        private void FanexDataConfig(IAppSettings appSettings)
        {
            var connections = Configuration
                                  .GetSection("Connections")
                                  .GetChildren()
                                  .AsEnumerable()
                                  .ToDictionary(
                                           x => x.GetValue<string>("Name"),
                                           y => new ConnectionConfiguration(y.GetValue<string>("Name"), y.GetValue<string>("ConnectionString"), y.GetValue<string>("ProviderName"))
                                   );

            DbSettingProviderManager
              .StartNewSession()
              .Use(connections)
              .WithMySql()
              .UseDefaultDbSettingProvider(resourcePath: appSettings.SPName)
              .Run();
        }

        private void RegisterLogging(IAppSettings appSettings)
        {
            LogManager
              .SetDefaultLogCategory("NexSphere.DataAccessAPI")
              .Use(new List<ILogEngine>
              {
                        new SentryLogging(new SentryEngineOptions
                        {
                            Dsn = new Dsn(appSettings.SentryDsn)
                        })
              });

        }
    }
}
