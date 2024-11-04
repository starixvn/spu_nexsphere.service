using Microsoft.AspNetCore.Mvc;
using NexSphere.Core;
using NexSphere.Core.Models;
using NexSphere.Core.Services;
using NexSphere.Model;
using System.Security.Claims;

namespace NexSphere.AuthenticateAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAppSettings appSettings;
        public AuthenticationController(IAppSettings appSettings)
        {
            this.appSettings = appSettings;
        }

        [HttpPost("authenticate")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var claims = GetClaims(loginModel.Username, loginModel.Password);

            var tokenSetting = new TokenSetting()
            {
                SecretKey = appSettings.DataAccessApiSecretKey,
                Issuer = appSettings.ApiIssuer
            };

            return claims != null ? Ok(new JwtTokenService().GenerateToken(tokenSetting, loginModel.Username, claims, 10)) : Unauthorized();

        }

        private List<Claim>? GetClaims(string username, string password)
        {
            if (username == "admin" || password == "123456")
            {
                return new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim("customrole", "admin"),
                };
            }

            if (username == "user" || password == "123456")
            {
                return new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim("customrole", "user"),
                };
            }

            return null;
        }
    }
}
