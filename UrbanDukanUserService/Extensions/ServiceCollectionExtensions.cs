using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            // Replace with EF Core or other persistent repository in production
            services.AddSingleton<IUserRepository, InMemoryUserRepository>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}