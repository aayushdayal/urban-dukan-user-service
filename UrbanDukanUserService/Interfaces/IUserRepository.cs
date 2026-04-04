using System;
using System.Threading.Tasks;
using UrbanDukanUserService.Models;

namespace UrbanDukanUserService.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task CreateAsync(User user);

        // New: resolve a Role by name (case-insensitive)
        Task<Role?> GetRoleByNameAsync(string name);

        // Persist updates to an existing user
        Task UpdateAsync(User user);
    }
}