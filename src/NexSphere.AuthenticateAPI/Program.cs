using Fanex.Logging;
using NexSphere.Core.Services;
using NexSphere.Core;
using Fanex.Data;
using Fanex.Logging.Sentry.RavenClient;
using SharpRaven;
using NexSphere.AuthenticateAPI.Middlewares;
using ILogger = Fanex.Logging.ILogger;
using Fanex.Data.MySql;
using NexSphere.AuthenticateAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var appSettings = new AppSettings(builder.Configuration);

builder.Services.AddSingleton<IAppSettings>(appSettings);
builder.Services.AddSingleton<ILogger, Logger>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IIdentityService, IdentityService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token in the text input below.\n\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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

var secretKey = builder.Configuration["JwtSettings:SecretKey"];
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "yourIssuer",
        ValidAudience = "yourAudience",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"))
    .AddPolicy("RequireUserRole", policy => policy.RequireRole("User"));

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
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "NexSphere API");
    });
}

app.UseRouting();

app.UseHttpsRedirection();

//app.UseMiddleware<JWTValidationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

LogService.Info(string.Format("NexSphere Authenticate API started at {0}", DateTimeOffset.Now));

app.Run();
