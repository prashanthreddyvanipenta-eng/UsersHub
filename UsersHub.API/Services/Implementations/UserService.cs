using UsersHub.API.DTOs.User;
using UsersHub.API.Repositories.Interfaces;
using UsersHub.API.Services.Interfaces;

namespace UsersHub.API.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }
    }
}