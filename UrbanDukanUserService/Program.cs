using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UrbanDukanUserService.Extensions;
using UrbanDukanUserService.Settings;
using UrbanDukanUserService.Data;
using UrbanDukanUserService.Models;

var builder = WebApplication.CreateBuilder(args);

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

var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
});
var logger = loggerFactory.CreateLogger("Startup");
logger.LogWarning("JwtSettings:Secret is empty. Configure a strong secret in production.");

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

        //if (applyMigrations)
        //{
        //    logger1.LogInformation("Applying EF Core migrations at startup...");
        //    db.Database.Migrate();
        //    logger1.LogInformation("Migrations applied.");
        //}
        //else
        //{
        //    logger1.LogInformation("ApplyMigrationsAtStartup is false; skipping migrations at startup.");
        //}

        //if (seedRoles)
        //{
        //    logger1.LogInformation("Seeding role data (SeedRolesAtStartup=true)...");
        //    var roles = new[] { "Admin", "Seller", "Buyer" };
        //    foreach (var roleName in roles)
        //    {
        //        if (!db.Roles.Any(r => r.Name == roleName))
        //        {
        //            db.Roles.Add(new Role { Name = roleName });
        //        }
        //    }
        //    db.SaveChanges();
        //    logger1.LogInformation("Role seeding complete.");
        //}
    
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