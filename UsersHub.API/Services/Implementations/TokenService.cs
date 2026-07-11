using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UsersHub.API.Configurations;
using UsersHub.API.Models;
using UsersHub.API.Services.Interfaces;
using System.Security.Cryptography;

namespace UsersHub.API.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;

        public TokenService(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;
        }

        public string GenerateToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Email, user.Email!)
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public RefreshToken GenerateRefreshToken(ApplicationUser user)
            {
                var randomBytes = RandomNumberGenerator.GetBytes(64);

                return new RefreshToken
                {
                    UserId = user.Id,
                    Token = Convert.ToBase64String(randomBytes),
                    CreatedAt = DateTime.UtcNow,
                    //ExpiresAt = DateTime.UtcNow.AddDays(7)
                    ExpiresAt = DateTime.UtcNow.AddMinutes(1)
                };
            }
}
}