using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UrbanDukanUserService.Extensions;
using UrbanDukanUserService.Settings;

var builder = WebApplication.CreateBuilder(args);

// Configuration and services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Bind JwtSettings and register user-service components
builder.Services.AddUrbanDukanUserService(builder.Configuration);

// Read JwtSettings for authentication configuration
var jwtSection = builder.Configuration.GetSection("JwtSettings");
var jwt = jwtSection.Get<JwtSettings>() ?? new JwtSettings();

// Replace this line:
// builder.Logging.CreateLogger("Startup").LogWarning("JwtSettings:Secret is empty. Configure a strong secret in production.");

// With the following block:
var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.AddConsole();
    // Optionally add other providers as needed
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

// Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();