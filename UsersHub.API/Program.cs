using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UsersHub.API.Configurations;
using UsersHub.API.Configurations;
using UsersHub.API.Data;
using UsersHub.API.Extensions;
using UsersHub.API.Middlewares;
using UsersHub.API.Models;
using UsersHub.API.Repositories.Implementations;
using UsersHub.API.Repositories.Interfaces;
using UsersHub.API.Services.Implementations;
using UsersHub.API.Services.Implementations;
using UsersHub.API.Services.Interfaces;
using UsersHub.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();


builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentityServices();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices();
// Configure JwtSettings
builder.Services.AddJwtAuthentication(builder.Configuration);


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await UsersHub.API.Data.Seed.RoleSeeder.SeedRolesAsync(roleManager);
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();