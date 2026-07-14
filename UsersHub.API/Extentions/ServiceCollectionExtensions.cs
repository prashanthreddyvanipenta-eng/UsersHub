using UsersHub.API.Repositories.Implementations;
using UsersHub.API.Repositories.Interfaces;
using UsersHub.API.Services.Implementations;
using UsersHub.API.Services.Interfaces;

namespace UsersHub.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddScoped<ITokenService, TokenService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}