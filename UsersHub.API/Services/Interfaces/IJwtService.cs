using UsersHub.API.Models;

namespace UsersHub.API.Services.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(ApplicationUser user, IList<string> roles);
    }
}