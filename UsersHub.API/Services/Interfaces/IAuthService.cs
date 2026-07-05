using UsersHub.API.DTOs;
using UsersHub.API.DTOs.Auth;

namespace UsersHub.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse> RegisterAsync(RegisterRequest request);

        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}