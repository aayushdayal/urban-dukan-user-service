using System.ComponentModel.DataAnnotations;

namespace UrbanDukanUserService.DTOs
{
    public class RegisterRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Role { get; set; }
    }
}