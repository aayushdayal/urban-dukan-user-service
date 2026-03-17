using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UrbanDukanUserService.Interfaces;
using UrbanDukanUserService.Models;

namespace UrbanDukanUserService.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<string, User> _byEmail = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<Guid, User> _byId = new();

        public Task<User?> GetByEmailAsync(string email)
        {
            _byEmail.TryGetValue(email, out var user);
            return Task.FromResult(user);
        }

        public Task<User?> GetByIdAsync(Guid id)
        {
            _byId.TryGetValue(id, out var user);
            return Task.FromResult(user);
        }

        public Task CreateAsync(User user)
        {
            if (!_byEmail.TryAdd(user.Email, user))
                throw new InvalidOperationException("User with this email already exists.");

            _byId[user.Id] = user;
            return Task.CompletedTask;
        }
    }
}