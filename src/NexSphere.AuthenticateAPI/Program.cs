using Fanex.Logging;
using NexSphere.Core.Services;
using NexSphere.Core;
using Fanex.Data;
using Fanex.Logging.Sentry.RavenClient;
using SharpRaven;
using NexSphere.AuthenticateAPI.Middlewares;
using ILogger = Fanex.Logging.ILogger;
using Fanex.Data.MySql;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var appSettings = new AppSettings(builder.Configuration);

builder.Services.AddSingleton<IAppSettings>(appSettings);
builder.Services.AddSingleton<ILogger, Logger>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOnlyGateway",
        policy =>
        {
            policy.WithOrigins("http://localhost:5000")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

#region FanexDataConfig
var connections = builder.Configuration
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
#endregion


#region RegisterLogging
LogManager
              .SetDefaultLogCategory("NexSphere.AuthenticateAPI")
              .Use(new List<ILogEngine>
              {
                        new SentryLogging(new SentryEngineOptions
                        {
                            Dsn = new Dsn(appSettings.SentryDsn)
                        })
              });
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NexSphere.AuthenticateAPI v1");
    });
}

app.UseRouting();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<JWTValidationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

LogService.Info(string.Format("NexSphere Authenticate API started at {0}", DateTimeOffset.Now));

app.Run();
