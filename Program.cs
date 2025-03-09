using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Serilog;
using WebApiProject.Interface;
using WebApiProject.Services;
using myLoggerMiddleWare;
using myMiddleWareExceptions;
using WebApiProject.services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(3000); // HTTP
    options.ListenLocalhost(3001, listenOptions => { listenOptions.UseHttps(); });
});

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
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
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
        new string[] {} }
    });
    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/auth"),
                TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
                Scopes = new Dictionary<string, string>
                {
                    { "openid", "OpenID Connect" },
                    { "profile", "Access profile information" },
                    { "email", "Access email address" }
                }
            }
        }
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "oauth2"
                }
            },
            new List<string>() { "openid", "profile", "email" }
        }
    });
});

builder.Services.AddJobFinderServices();
builder.Services.AddTokenService();

// Authentication and Authorization setup
var provider = builder.Services.BuildServiceProvider();
var tokenService = provider.GetRequiredService<ITokenService>();

// Unified authentication setup (Google + JWT)
builder.Services.AddAuthentication(options =>
{
    // options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme; // Default scheme is Cookie
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
//  .AddCookie(options =>
//     {
//         options.LoginPath = "/Google/Login";  // הנתיב להפניה במקרה שצריך להתחבר
//         options.LogoutPath = "/Google/Logout"; // נתיב להתנתקות
//         options.AccessDeniedPath = "/Home/AccessDenied"; // נתיב במקרה של גישה לא מורשית
//     })
// Cookie authentication setup
// .AddGoogle(options =>
// {
//     var googleAuthConfig = builder.Configuration.GetSection("Authentication:Google");
//     options.ClientId = googleAuthConfig["ClientId"];
//     options.ClientSecret = googleAuthConfig["ClientSecret"];
//     options.CallbackPath = "/signin-google";
//     options.BackchannelTimeout = TimeSpan.FromMinutes(2); // Timeout extension
//     options.Scope.Add("openid");
//     options.Scope.Add("profile");
//     options.Scope.Add("email");
// })
.AddJwtBearer(cfg =>
{
    var googleAuthConfig = builder.Configuration.GetSection("Authentication:Google");
    // var clientId = googleAuthConfig["ClientId"];
    cfg.RequireHttpsMetadata = true;
    cfg.TokenValidationParameters = tokenService.GetTokenValidationParameters();
    cfg.TokenValidationParameters.RoleClaimType = "role";
    cfg.Authority = "https://accounts.google.com";
    cfg.MetadataAddress = "https://accounts.google.com/.well-known/openid-configuration";
    // cfg.Audience = clientId;
    cfg.SaveToken = true;
});


// Authorization policies
builder.Services.AddAuthorization(cfg =>
{
    cfg.AddPolicy("SuperAdmin", policy => policy.RequireClaim("type", "SuperAdmin"));
    cfg.AddPolicy("Admin", policy => policy.RequireClaim("type", "Admin", "SuperAdmin"));
    cfg.AddPolicy("User", policy => policy.RequireClaim("type", "User", "Admin", "SuperAdmin"));
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()  // Change this to a more restrictive policy in production
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});


// Set up logging with Serilog
builder.Host.UseSerilog((context, config) =>
{
    config
        .WriteTo.Console()
        .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
        .MinimumLevel.Debug();
});

// Middleware for logging and exceptions
var app = builder.Build();
app.UseCors("AllowAll");
app.UseMyLogMiddleWare();
app.useMyMiddleWareExceptions();

// HTTP request pipeline configuration
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

app.MapControllers();
app.MapFallbackToFile("Html/Jobs.html");

app.Run();
