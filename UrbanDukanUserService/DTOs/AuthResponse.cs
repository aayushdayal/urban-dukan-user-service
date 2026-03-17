using System;

namespace UrbanDukanUserService.DTOs
{
    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;
    }
}