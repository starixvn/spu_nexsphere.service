using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using NexSphere.Service.AuthAPI.Models;
using NexSphere.Service.Core.Models;
using System.Security.Claims;
using System.Text;

namespace NexSphere.Service.AuthAPI.Services
{
    public class JwtTokenService
    {
        private readonly List<User> _users = new List<User>()
    {
        new User(){
            Username= "admin",
            Password= "aDm1n", 
            Role =  "Administrator", 
           Scopes= new[] {"shoes.read"} 
        },
        new User(){ Username ="user01", Password = "u$3r01", Role= "User", Scopes= new[] {"shoes.read"} }
    };

        public AuthenticationToken? GenerateAuthToken_old(LoginModel loginModel)
        {
            var user = _users.FirstOrDefault(u => u.Username == loginModel.Username
                                               && u.Password == loginModel.Password);

            if (user == null)
            {
                return null;
            }



            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtExtensions.SecurityKey));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var expirationTimeStamp = DateTime.Now.AddMinutes(5);

            var claims = new List<Claim>
        {
            new Claim("username", user.Username),
            new Claim("role", user.Role),
            new Claim("scope", string.Join(" ", user.Scopes))
        };

            var tokenOptions = new JwtSecurityToken(
                issuer: "https://localhost:5002",
                claims: claims,
                expires: expirationTimeStamp,
                signingCredentials: signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);


            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", appSettings.ApiUrl) }),
                Expires = DateTime.UtcNow.AddMinutes(1),
                Issuer = appSettings.ApiUrl,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyJWT), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);

            return new AuthenticationToken(tokenString, (int)expirationTimeStamp.Subtract(DateTime.Now).TotalSeconds);
        }

        public string GenerateAuthToken(LoginModel loginModel)
        {
            var user = _users.FirstOrDefault(u => u.Username == loginModel.Username
                                               && u.Password == loginModel.Password);

            if (user == null)
            {
                return null;
            }



            var data = Encoding.UTF8.GetBytes("SomeStringFromConfig1234 SomeStringFromConfig1234");
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(data);

            var claims = new Dictionary<string, object>
            {
                [ClaimTypes.Name] = "Testuser",
                [ClaimTypes.GroupSid] = "Tenant1",
                [ClaimTypes.Sid] = "3c545f1c-cc1b-4cd5-985b-8666886f985b"
            };
            var descriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Issuer = "MyIssuer",
                Audience = "MyAudience",
                Claims = claims,
                IssuedAt = null,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(120),
                SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new Microsoft.IdentityModel.JsonWebTokens.JsonWebTokenHandler();
            handler.SetDefaultTimesOnTokenCreation = false;
            var tokenString = handler.CreateToken(descriptor);
            
            return tokenString;
        }
    }
}
