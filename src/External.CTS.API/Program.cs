using External.CTS.API.Services;
using External.CTS.Core.Utility;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOnlyGateway",
        policy =>
        {
            policy.WithOrigins("https://localhost:5000")
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

SD.GatewayAPIBase = builder.Configuration["ServiceUrls:GatewayAPI"];

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

app.UseCors("AllowOnlyGateway");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
