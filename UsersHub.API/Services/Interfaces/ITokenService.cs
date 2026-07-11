using UsersHub.API.Models;

namespace UsersHub.API.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);

        RefreshToken GenerateRefreshToken(ApplicationUser user);
    }
}