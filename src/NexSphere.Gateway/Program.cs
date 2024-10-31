using Fanex.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NexSphere.Core;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;
using NexSphere.Gateway.Middlewares;
using Fanex.Logging.Sentry.RavenClient;
using SharpRaven;
using ILogger = Fanex.Logging.ILogger;
using NexSphere.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

var appSettings = new AppSettings(builder.Configuration);

builder.Services.AddSingleton<IAppSettings>(appSettings);
builder.Services.AddSingleton<ILogger, Logger>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

builder.Services.AddSwaggerGen();

#region Authentication
builder.Services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = appSettings.ApiIssuer,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.DataAccessApiSecretKey))
            };
        });
#endregion

LogManager
          .SetDefaultLogCategory("NexSphere.Gateway")
          .Use(new List<ILogEngine>
          {
                    new SentryLogging(new SentryEngineOptions
                    {
                        Dsn = new Dsn(appSettings.SentryDsn)
                    })
          });


var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NexSphere.Gateway v1");
    c.RoutePrefix = string.Empty;
});

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<JWTValidationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

await app.UseOcelot();

LogService.Info(string.Format("NexSphere Gateway has been started at {0}", DateTimeOffset.Now));

app.Run();
