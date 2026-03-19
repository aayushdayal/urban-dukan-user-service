using Microsoft.EntityFrameworkCore;
using UrbanDukanUserService.Models;

namespace UrbanDukanUserService.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users table
            var user = modelBuilder.Entity<User>();
            user.ToTable("Users");
            user.HasKey(u => u.Id);
            user.HasIndex(u => u.Email).IsUnique();
            user.Property(u => u.Email).IsRequired();
            user.Property(u => u.PasswordHash).IsRequired();
            user.Property(u => u.FirstName);
            user.Property(u => u.LastName);
            user.Property(u => u.IsActive).HasDefaultValue(true);
            user.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            user.Property(u => u.UpdatedAt);
            user.Property(u => u.Address);
            user.Property(u => u.Phone);

            // Roles table
            var role = modelBuilder.Entity<Role>();
            role.ToTable("Roles");
            role.HasKey(r => r.Id);
            role.HasIndex(r => r.Name).IsUnique();
            role.Property(r => r.Name).IsRequired();

            // Many-to-many join table will be created by EF Core with default name and columns
            modelBuilder.Entity<User>()
                        .HasMany(u => u.Roles)
                        .WithMany(r => r.Users)
                        .UsingEntity(j => j.ToTable("UserRoles"));
        }
    }
}
