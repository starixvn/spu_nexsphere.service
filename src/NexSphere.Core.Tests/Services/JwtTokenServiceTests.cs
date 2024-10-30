using NexSphere.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NexSphere.Core.Test.Services
{
    public class JwtTokenServiceTests
    {
        [Fact]
        public async Task ValidateToken_WhenValidToken_ShouldReturnTrue()
        {
            //  Arrange
            string secretKey = "secretJWTsigningKey@123-secretJWTsigningKey@123";
            string issuer = "https://localhost:5002";
            var service = new JwtTokenService(secretKey, issuer);
            string username = "username";
            int expireMinutes = 30;
            var claims = new List<Claim>()
            {
                new Claim("username", username)
            };
            string token = service.GenerateToken(username, claims, expireMinutes);

            bool isValid = await service.ValidateToken(token);

            Assert.True(isValid);
        }

        [Fact]
        public async Task ValidateToken_WhenInValidToken_ShouldReturnFalse()
        {
            //  Arrange
            string secretKey = "secretJWTsigningKey@123-secretJWTsigningKey@123";
            string issuer = "https://localhost:5002";
            var service = new JwtTokenService(secretKey, issuer);
            string token = "abc";

            bool isValid = await service.ValidateToken(token);

            Assert.False(isValid);
        }
    }
}
