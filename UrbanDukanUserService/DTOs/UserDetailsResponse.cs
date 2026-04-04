using System;

namespace UrbanDukanUserService.DTOs
{
    public class UserDetailsResponse
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string[] Roles { get; set; } = Array.Empty<string>();
    }
}