using Microsoft.AspNetCore.Identity;
using UsersHub.API.DTOs;
using UsersHub.API.DTOs.Auth;
using UsersHub.API.DTOs.User;
using UsersHub.API.Models;
using UsersHub.API.Repositories.Interfaces;
using UsersHub.API.Services.Interfaces;

namespace UsersHub.API.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ITokenService _jwtService;

        public AuthService(IAuthRepository authRepository,ITokenService jwtService )
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
        }

        public async Task<ApiResponse> RegisterAsync(RegisterRequest request)
        {

            if (await _authRepository.UserExistsAsync(request.Email))
            {
                return new ApiResponse
                {
                    Success = false,
                    Message = "Email already exists."
                };
            }

            var user = new ApplicationUser
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                UserName = request.Email
            };

            IdentityResult result = await _authRepository.RegisterUserAsync(user, request.Password);

            if (result.Succeeded)
            {
                await _authRepository.AddUserToRoleAsync(user, "User");

                return new ApiResponse
                {
                    Success = true,
                    Message = "User registered successfully."
                };
            }

            return new ApiResponse
            {
                Success = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            };
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Step 1: Find the user by email
            var user = await _authRepository.GetUserByEmailAsync(request.Email);

            // Step 2: Check if the user exists
            if (user == null)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }

            // Step 3: Verify the password
            var isPasswordValid = await _authRepository.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
            {
                return new LoginResponse
                {
                    Success = false,
                    Message = "Invalid email or password."
                };
            }
            var roles = await _authRepository.GetUserRolesAsync(user);
            // Step 4: Generate JWT token
            var token = _jwtService.GenerateToken(user, roles);
            var refreshToken = _jwtService.GenerateRefreshToken(user);
            await _authRepository.SaveRefreshTokenAsync(refreshToken);
            // Step 5: Return success response
            return new LoginResponse
            {
                Success = true,
                Message = "Login successful.",
                AccessToken = token,
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<UserProfileResponse?> GetProfileAsync(string userId)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return null;
            }

            return new UserProfileResponse
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!
            };
        }

        public async Task<RefreshTokenResponse> RefreshTokenAsync(
    RefreshTokenRequest request)
        {
            var refreshToken = await _authRepository
    .GetRefreshTokenAsync(request.RefreshToken);

            if (refreshToken == null)
            {
                return new RefreshTokenResponse
                {
                    Success = false,
                    Message = "Invalid refresh token."
                };
            }
            if (refreshToken == null)
            {
                return new RefreshTokenResponse
                {
                    Success = false,
                    Message = "Invalid refresh token."
                };
            }
            if (refreshToken.IsRevoked)
            {
                return new RefreshTokenResponse
                {
                    Success = false,
                    Message = "Refresh token has been revoked."
                };
            }
            var roles = await _authRepository.GetUserRolesAsync(refreshToken.User);
            var accessToken = _jwtService.GenerateToken(refreshToken.User, roles);

            return new RefreshTokenResponse
            {
                Success = true,
                Message = "Access token generated successfully.",
                AccessToken = accessToken
            };
        }
    }
}