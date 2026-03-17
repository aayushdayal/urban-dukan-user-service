using System;
using System.Threading.Tasks;
using UrbanDukanUserService.Models;

namespace UrbanDukanUserService.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task CreateAsync(User user);
    }
}