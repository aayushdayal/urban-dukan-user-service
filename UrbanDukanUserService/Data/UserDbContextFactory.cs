using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UrbanDukanUserService.Data
{
    public class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
    {
        public UserDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<UserDbContext>();

            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Connection string not found for design-time DbContext creation.");
            }

            optionsBuilder.UseSqlServer(connectionString);

            return new UserDbContext(optionsBuilder.Options);
        }
    }
}