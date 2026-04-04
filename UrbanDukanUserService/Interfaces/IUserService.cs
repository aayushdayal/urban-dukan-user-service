using System.Threading.Tasks;
using UrbanDukanUserService.DTOs;

namespace UrbanDukanUserService.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);

        // Get details for a specific user id (returns null if not found)
        Task<UserDetailsResponse?> GetUserDetailsAsync(int userId);

        // Update profile fields for a user (returns null if user not found)
        Task<UserDetailsResponse?> UpdateUserAsync(int userId, UpdateUserRequest request);
    }
}