using myMiddleWareExceptions;
using WebApiProject.Interface;
using WebApiProject.services;
using Serilog;
using myLoggerMiddleWare;
using WebApiProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddJobFinderServices();
builder.Host.UseSerilog((context, config) =>
{
    config
        .WriteTo.Console()
        .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day) // כתיבה לקובץ, מתחדש כל יום
        .MinimumLevel.Debug();
});

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = false;
        cfg.TokenValidationParameters = JobFinderTokenService.GetTokenValidationParameters();
    });

builder.Services.AddAuthorization(cfg =>
    {
        cfg.AddPolicy("superAdmin", policy => policy.RequireClaim("role", "superAdmin"));
        cfg.AddPolicy("generalUser", policy => policy.RequireClaim("role", "superAdmin", "generalUser"));
        cfg.AddPolicy("user", policy => policy.RequireClaim("ClearanceLevel", "user", "Admin"));
        cfg.AddPolicy("Management", policy => policy.RequireClaim("permision", "Admin"));
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddJobFinderServices();
var app = builder.Build();


app.UseMyLogMiddleWare();
app.useMyMiddleWareExceptions();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();
