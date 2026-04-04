using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrbanDukanUserService.Data;
using UrbanDukanUserService.Interfaces;
using UrbanDukanUserService.Models;

namespace UrbanDukanUserService.Repositories
{
    public class EfUserRepository : IUserRepository
    {
        private readonly UserDbContext _db;

        public EfUserRepository(UserDbContext db) => _db = db;

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _db.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _db.Users.Include(u => u.Roles).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task CreateAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task<Role?> GetRoleByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            var normalized = name.Trim().ToLowerInvariant();
            return await _db.Roles.FirstOrDefaultAsync(r => r.Name.ToLower() == normalized);
        }

        public async Task UpdateAsync(User user)
        {
            // User is normally tracked when retrieved via GetByIdAsync; Update ensures changes are persisted.
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }
    }
}
