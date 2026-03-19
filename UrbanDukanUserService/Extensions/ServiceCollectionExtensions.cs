using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UrbanDukanUserService.Data;
using UrbanDukanUserService.Interfaces;
using UrbanDukanUserService.Repositories;
using UrbanDukanUserService.Services;
using UrbanDukanUserService.Settings;

namespace UrbanDukanUserService.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUrbanDukanUserService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // Register DbContext. Use connection string DefaultConnection from configuration; fallback to in-memory SQLite for quick testing.
            var conn = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrWhiteSpace(conn))
            {
                throw new Exception("Connection string not configured!");
                // services.AddDbContext<UserDbContext>(opt => opt.UseSqlServer(conn));
            }
            else
            {
                // In-memory SQLite helps with dev/testing without extra setup. Schema can be migrated later.
                services.AddDbContext<UserDbContext>(opt => opt.UseSqlite("Data Source=UserDb.sqlite"));
            }

            // Replace with EF Core repository implementation
            services.AddScoped<IUserRepository, EfUserRepository>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}