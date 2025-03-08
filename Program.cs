using myMiddleWareExceptions;
using WebApiProject.Interface;
using WebApiProject.services;
using Serilog;
using myLoggerMiddleWare;
using WebApiProject.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddJobFinderServices();
builder.Services.AddTokenService();

builder.Host.UseSerilog((context, config) =>
{
    config
        .WriteTo.Console()
        .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day) // כתיבה לקובץ, מתחדש כל יום
        .MinimumLevel.Debug();
});

var provider = builder.Services.BuildServiceProvider();
var tokenService = provider.GetRequiredService<ITokenService>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(cfg =>
    {
        cfg.RequireHttpsMetadata = true;
        cfg.TokenValidationParameters = tokenService.GetTokenValidationParameters();
        cfg.TokenValidationParameters.RoleClaimType = "role";  // מוודא שהמערכת מזהה את ה-Role כמו שהוא בטוקן
    });



builder.Services.AddAuthorization(cfg =>
    {
        cfg.AddPolicy("SuperAdmin", policy => policy.RequireClaim("type", "SuperAdmin"));
        cfg.AddPolicy("Admin", policy => policy.RequireClaim("type", "Admin", "SuperAdmin"));
        cfg.AddPolicy("User", policy => policy.RequireClaim("type", "User", "Admin", "SuperAdmin"));
    });


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "WebAPIProject",
        Version = "v1",
        Description = "API for Job Finder"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter JWT with Bearer into field",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                { new OpenApiSecurityScheme
                        {
                         Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer"}
                        },
                    new string[] {}
                }
                });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});


builder.Services.AddJobFinderServices();
builder.Services.AddTokenService();
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
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");


app.MapControllers();
app.MapFallbackToFile("Html/index.html");
app.Run();
