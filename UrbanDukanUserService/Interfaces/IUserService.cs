using System.Threading.Tasks;
using UrbanDukanUserService.DTOs;

namespace UrbanDukanUserService.Interfaces
{
    public interface IUserService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}