using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NexSphere.AuthenticateAPI.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(uint userID, IEnumerable<string> roles);

        string GetRefreshToken(uint userID, IEnumerable<string> roles);
    }

    public class AuthService(IConfiguration configuration) : IAuthService
    {
        public string GenerateJwtToken(uint userID, IEnumerable<string> roles)
        {
            var secretKey = configuration["JwtSettings:SecretKey"];

            var claims = new List<Claim>
                        {
                            new(ClaimTypes.Name, userID.ToString())
                        };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "yourIssuer",
                audience: "yourAudience",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetRefreshToken(uint userID, IEnumerable<string> roles)
        {
            return GenerateJwtToken(userID, roles);
        }
    }
}
