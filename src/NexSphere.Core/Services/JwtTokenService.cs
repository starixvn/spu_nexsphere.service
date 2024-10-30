using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NexSphere.Core.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NexSphere.Core.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(TokenSetting tokenSetting, string username, List<Claim> claims, int expireMinutes = 30);

        Task<bool> ValidateToken(TokenSetting tokenSetting, string token);
    }
    
    public class JwtTokenService : IJwtTokenService
    {
        public string GenerateToken(TokenSetting tokenSetting, string username, List<Claim> claims, int expireMinutes = 30)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSetting.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                Issuer = tokenSetting.Issuer,
                SigningCredentials = credentials
            };

            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }

        public async Task<bool> ValidateToken(TokenSetting tokenSetting, string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSetting.SecretKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateIssuer = true,
                ValidIssuer = tokenSetting.Issuer,
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
