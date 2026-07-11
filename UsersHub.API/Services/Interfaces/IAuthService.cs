using UsersHub.API.DTOs;
using UsersHub.API.DTOs.Auth;
using UsersHub.API.DTOs.User;

namespace UsersHub.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse> RegisterAsync(RegisterRequest request);

        Task<LoginResponse> LoginAsync(LoginRequest request);

        Task<UserProfileResponse?> GetProfileAsync(string userId);

        Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
    }
}