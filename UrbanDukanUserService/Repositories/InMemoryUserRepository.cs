using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using UrbanDukanUserService.Interfaces;
using UrbanDukanUserService.Models;

namespace UrbanDukanUserService.Repositories
{
    // Keep the old in-memory repository for tests/dev but mark as obsolete - registration now uses EF repository
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<string, User> _byEmail = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<int, User> _byId = new();
        private readonly ConcurrentDictionary<string, Role> _roles = new(StringComparer.OrdinalIgnoreCase);

        public InMemoryUserRepository()
        {
            // seed some roles so tests/dev have them available
            var admin = new Role { Id = 1, Name = "Admin" };
            var seller = new Role { Id = 2, Name = "Seller" };
            var buyer = new Role { Id = 3, Name = "Buyer" };
            _roles[admin.Name] = admin;
            _roles[seller.Name] = seller;
            _roles[buyer.Name] = buyer;
        }

        public Task<User?> GetByEmailAsync(string email)
        {
            _byEmail.TryGetValue(email, out var user);
            return Task.FromResult(user);
        }

        public Task<User?> GetByIdAsync(int id)
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

        public Task<Role?> GetRoleByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Task.FromResult<Role?>(null);

            _roles.TryGetValue(name.Trim(), out var role);
            return Task.FromResult(role);
        }
    }
}