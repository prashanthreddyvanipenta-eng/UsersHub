using Microsoft.AspNetCore.Identity;
using UsersHub.API.Models;

namespace UsersHub.API.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<bool> UserExistsAsync(string email);

        Task<IdentityResult> RegisterUserAsync(ApplicationUser user, string password);

        Task<ApplicationUser?> GetUserByEmailAsync(string email);

        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);

        Task AddUserToRoleAsync(ApplicationUser user, string role);

        Task<IList<string>> GetUserRolesAsync(ApplicationUser user);

        Task<ApplicationUser?> GetUserByIdAsync(string userId);
    }
}