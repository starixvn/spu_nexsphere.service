using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NexSphere.Core.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(string username, List<Claim> claims, int expireMinutes = 30);

        Task<bool> ValidateToken(string token);
    }
    
    public class JwtTokenService : IJwtTokenService
    {
        private readonly string secretKey;
        private readonly string issuer;
        

        public JwtTokenService(string secretKey, string issuer)
        {
            this.secretKey = secretKey;
            this.issuer = issuer;
        }

        public string GenerateToken(string username, List<Claim> claims, int expireMinutes = 30)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                Issuer = issuer,
                SigningCredentials = credentials
            };

            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }

        public async Task<bool> ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JsonWebTokenHandler();

            try
            {
                var validationResult = await tokenHandler.ValidateTokenAsync(token, validationParameters);
                return validationResult.IsValid;
            }
            catch
            {
                return false;
            }
        }
    }
}
