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

        public AuthService(IAuthRepository authRepository, ITokenService jwtService)
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

        public async Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            // Step 1: Get refresh token from database
            var refreshToken = await _authRepository.GetRefreshTokenAsync(request.RefreshToken);

            if (refreshToken == null)
            {
                return new RefreshTokenResponse
                {
                    Success = false,
                    Message = "Invalid refresh token."
                };
            }

            // Step 2: Check if token is already revoked
            if (refreshToken.IsRevoked)
            {
                return new RefreshTokenResponse
                {
                    Success = false,
                    Message = "Refresh token has already been revoked."
                };
            }

            // Step 3: Check if token is expired
            if (refreshToken.IsExpired)
            {
                return new RefreshTokenResponse
                {
                    Success = false,
                    Message = "Refresh token has expired."
                };
            }

            // Step 4: Get user
            var user = await _authRepository.GetUserByIdAsync(refreshToken.UserId);

            if (user == null)
            {
                return new RefreshTokenResponse
                {
                    Success = false,
                    Message = "User not found."
                };
            }

            // Step 5: Get user roles
            var roles = await _authRepository.GetUserRolesAsync(user);

            // Step 6: Revoke current refresh token
            refreshToken.RevokedAt = DateTime.UtcNow;

            await _authRepository.UpdateRefreshTokenAsync(refreshToken);

            // Step 7: Generate new access token
            var accessToken = _jwtService.GenerateToken(user, roles);

            // Step 8: Generate new refresh token
            var newRefreshToken = _jwtService.GenerateRefreshToken(user);

            // Step 9: Save new refresh token
            await _authRepository.SaveRefreshTokenAsync(newRefreshToken);

            // Step 10: Return both new tokens
            return new RefreshTokenResponse
            {
                Success = true,
                Message = "Token refreshed successfully.",
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token
            };
        }

        public async Task<LogoutResponse> LogoutAsync(RefreshTokenRequest request)
        {
            // Step 1: Get refresh token
            var refreshToken = await _authRepository
                .GetRefreshTokenAsync(request.RefreshToken);

            // Step 2: Validate token
            if (refreshToken == null)
            {
                return new LogoutResponse
                {
                    Success = false,
                    Message = "Invalid refresh token."
                };
            }

            // Step 3: Check if already revoked
            if (refreshToken.IsRevoked)
            {
                return new LogoutResponse
                {
                    Success = false,
                    Message = "Refresh token has already been revoked."
                };
            }

            // Step 4: Check if expired
            if (refreshToken.IsExpired)
            {
                return new LogoutResponse
                {
                    Success = false,
                    Message = "Refresh token has expired."
                };
            }

            // Step 5: Revoke token
            refreshToken.RevokedAt = DateTime.UtcNow;

            // Step 6: Save changes
            await _authRepository.UpdateRefreshTokenAsync(refreshToken);

            // Step 7: Return success
            return new LogoutResponse
            {
                Success = true,
                Message = "Logged out successfully."
            };
        }

        public async Task<LogoutResponse> LogoutAllDevicesAsync(string userId)
        {
            List<RefreshToken> refreshTokens = await _authRepository.GetActiveRefreshTokensByUserIdAsync(userId);

            if (refreshTokens.Count == 0)
            {
                return new LogoutResponse
                {
                    Success = true,
                    Message = "All devices are already logged out."
                };
            }
            
                foreach (RefreshToken refreshToken in refreshTokens)
                {
                    refreshToken.RevokedAt = DateTime.UtcNow;
                }
                await _authRepository.UpdateRefreshTokensAsync(refreshTokens);
                return new LogoutResponse
                {
                    Success = true,
                    Message = "All devices Logged out successfully."
                };
            
        }
    }
}