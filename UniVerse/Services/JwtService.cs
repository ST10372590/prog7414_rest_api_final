using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Universe.Api.Models;

namespace Universe.Api.Services
{
    public class JwtService
    {
        private readonly IConfiguration _config;
        private readonly string _secret;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryMinutes;

        public JwtService(IConfiguration config)
        {
            _config = config;
            _secret = _config["Jwt:Key"] ?? throw new ArgumentNullException("JWT Key not configured");
            _issuer = _config["Jwt:Issuer"] ?? "UniverseApi";
            _audience = _config["Jwt:Audience"] ?? "UniverseApiUsers";
            _expiryMinutes = int.TryParse(_config["Jwt:ExpiryMinutes"], out var minutes) ? minutes : 60;
        }

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
