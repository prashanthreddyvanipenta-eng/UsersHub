using Microsoft.AspNetCore.Identity;
using UsersHub.API.Data;
using UsersHub.API.Models;

namespace UsersHub.API.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        return services;
    }
}