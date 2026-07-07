using UsersHub.API.DTOs.User;

namespace UsersHub.API.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserDto>> GetAllUsersAsync();
    }
}