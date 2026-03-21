using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.AzureAppServices;
using UrbanDukanUserService.Extensions;
using UrbanDukanUserService.Settings;
using UrbanDukanUserService.Data;
using UrbanDukanUserService.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure logging (console + debug + Azure App Service diagnostics)
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddAzureWebAppDiagnostics();

// Configuration and services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT (Bearer) support so you can use the Authorize button in UI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UrbanDukan User Service",
        Version = "v1",
        Description = "API for UrbanDukan user management (register, login, token testing)"
    });

    var bearerScheme = new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", bearerScheme);

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { bearerScheme, Array.Empty<string>() }
    });
});

// Bind JwtSettings and register user-service components
builder.Services.AddUrbanDukanUserService(builder.Configuration);

// Read JwtSettings for authentication configuration
var jwtSection = builder.Configuration.GetSection("JwtSettings");
var jwt = jwtSection.Get<JwtSettings>() ?? new JwtSettings();

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret ?? string.Empty)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

var app = builder.Build();

// log Jwt configuration after DI/logging is available
var logger = app.Services.GetRequiredService<ILogger<Program>>();
if (string.IsNullOrWhiteSpace(jwt.Secret) || jwt.Secret == "replace-with-a-strong-secret")
{
    logger.LogWarning("JwtSettings:Secret is empty or using the placeholder. Configure a strong secret in production.");
}
else
{
    logger.LogInformation("JwtSettings loaded.");
}

// Controlled migration + seeding (disabled by default).
// Configure via appsettings or environment variables:
//   ApplyMigrationsAtStartup = true|false
//   SeedRolesAtStartup = true|false
//var config = app.Services.GetRequiredService<IConfiguration>();
//var applyMigrations = config.GetValue<bool>("ApplyMigrationsAtStartup", false);
//var seedRoles = config.GetValue<bool>("SeedRolesAtStartup", false);

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UserDbContext>();

    // migration / seeding code (commented)...
}

// Pipeline
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "UrbanDukan User Service v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();