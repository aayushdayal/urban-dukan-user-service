using System;
using System.Collections.Generic;

namespace UrbanDukanUserService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        // New profile columns
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public string? Address { get; set; }
        public string? Phone { get; set; }

        // Navigation - many-to-many with Role
        public ICollection<Role> Roles { get; set; } = new List<Role>();
    }
}