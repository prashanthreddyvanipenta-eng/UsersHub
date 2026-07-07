using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using UsersHub.API.DTOs.User;
using UsersHub.API.Repositories.Interfaces;

namespace UsersHub.API.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly IConfiguration _configuration;

        public UserRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            using var connection = new SqlConnection(connectionString);

            await connection.OpenAsync();

            using var command = new SqlCommand(
                "sp_GetAllUsers",
                connection);

            command.CommandType = CommandType.StoredProcedure;

            using var reader = await command.ExecuteReaderAsync();

            var users = new List<UserDto>();
            while (await reader.ReadAsync())
            {
                var user = new UserDto
                {
                    Id = reader["Id"].ToString()!,
                    FirstName = reader["FirstName"].ToString()!,
                    LastName = reader["LastName"].ToString()!,
                    Email = reader["Email"].ToString()!
                };
                users.Add(user);
            }
            return users;
        }
    }
}