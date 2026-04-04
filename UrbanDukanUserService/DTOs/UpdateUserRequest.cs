using System.ComponentModel.DataAnnotations;

namespace UrbanDukanUserService.DTOs
{
    public class UpdateUserRequest
    {
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [Phone]
        public string? Phone { get; set; }
    }
}