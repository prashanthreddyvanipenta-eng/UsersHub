using UsersHub.API.DTOs.User;

namespace UsersHub.API.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();
    }
}