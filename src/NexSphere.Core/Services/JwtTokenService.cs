using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NexSphere.Core.Services
{
    public class JwtTokenService
    {
        private readonly string _secretKey = "secretJWTsigningKey@123-secretJWTsigningKey@123";
        private readonly string _issuer = "https://localhost:5002";
        

        public JwtTokenService()
        {
            
        }

        public string GenerateToken(string username, List<Claim> claims, int expireMinutes = 30)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                Issuer = _issuer,
                SigningCredentials = credentials
            };

            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }

        public async Task<bool> ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = _issuer,
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
