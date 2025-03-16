using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.OpenApi.Models;
using Serilog;
using WebApiProject.Interface;
using WebApiProject.Services;
using myLoggerMiddleWare;
using myMiddleWareExceptions;
using WebApiProject.services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(3000); // HTTP
    options.ListenLocalhost(3001, listenOptions => { listenOptions.UseHttps(); });
});

// הוספת שירותי בקרות
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// הגדרת Swagger עם תמיכה באימות OAuth2
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

// הוספת שירותים מותאמים אישית
builder.Services.AddJobService();
builder.Services.AddTokenService();
builder.Services.AddUserService();

// הגדרת אימות עם תמיכה ב-JWT, Google ו-Cookies
var provider = builder.Services.BuildServiceProvider();
var tokenService = provider.GetRequiredService<ITokenService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // הגדרת ברירת מחדל ל-JWT
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme; // Google כברירת מחדל לאתגר כניסה
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme; // כדי לתמוך גם ב-Cookies
})
.AddCookie() // הוספת תמיכה בקוקי
.AddGoogle(options =>
{
    var googleAuthConfig = builder.Configuration.GetSection("Authentication:Google");
    options.ClientId = googleAuthConfig["ClientId"];
    options.ClientSecret = googleAuthConfig["ClientSecret"];
    options.CallbackPath = "/signin-google";
    options.BackchannelTimeout = TimeSpan.FromMinutes(2);
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");
})
.AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = true;
    cfg.TokenValidationParameters = tokenService.GetTokenValidationParameters();
    cfg.TokenValidationParameters.RoleClaimType = "role";
    cfg.Authority = "https://accounts.google.com";
    cfg.MetadataAddress = "https://accounts.google.com/.well-known/openid-configuration";
    cfg.SaveToken = true;
});

// הגדרת הרשאות
builder.Services.AddAuthorization(cfg =>
{
    cfg.AddPolicy("SuperAdmin", policy => policy.RequireClaim("type", "SuperAdmin"));
    cfg.AddPolicy("Admin", policy => policy.RequireClaim("type", "Admin", "SuperAdmin"));
    cfg.AddPolicy("User", policy => policy.RequireClaim("type", "User", "Admin", "SuperAdmin"));
});

//הגדרת CORS לאפשר חיבור מה-Frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.WithOrigins("https://localhost:3001")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials()); // תמיכה ב-Cookies
});

// הגדרת Serilog ללוגים
builder.Host.UseSerilog((context, config) =>
{
    config
        .WriteTo.Console()
        .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day)
        .MinimumLevel.Debug();
});

// הגדרת Middleware
var app = builder.Build();
app.UseCors("AllowAll");
app.UseMyLogMiddleWare();
app.useMyMiddleWareExceptions();

// קביעת הגדרות HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthentication(); // הפעלת אימות
app.UseAuthorization();  // הפעלת הרשאות

app.MapControllers();
app.MapFallbackToFile("Html/Jobs.html");

app.Run();
