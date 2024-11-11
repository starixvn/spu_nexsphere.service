using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NexSphere.AuthenticateAPI.Services
{
    public interface IAuthService
    {
        string GenerateJwtToken(string username, IEnumerable<string> roles);

        IEnumerable<string> GetUserRoles(int userId);
    }

    public class AuthService(IConfiguration configuration) : IAuthService
    {
        public string GenerateJwtToken(string username, IEnumerable<string> roles)
        {
            var secretKey = configuration["JwtSettings:SecretKey"];

            var claims = new List<Claim>
                        {
                            new(ClaimTypes.Name, username)
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

        public IEnumerable<string> GetUserRoles(int userId)
        {
            if (userId == 1)
            {
                return ["admin"];
            }

            return ["user"];
        }
    }
}
