using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NexSphere.AuthenticateAPI.Models;
using NexSphere.Core;
using NexSphere.Core.Services;
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

        [HttpPost]
        public IActionResult Login([FromForm] string username, [FromForm] string password)
        {
            List<Claim> claims = null;
            if (username == "admin" || password == "123456") {
                claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim("role", "admin"),
                };
            }
            if (username == "user" || password == "123456")
            {
                claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, username),
                    new Claim("role", "user"),
                };
            }

            return claims != null ? Ok(new JwtTokenService(appSettings.ApiSecretKey, appSettings.ApiIssuer).GenerateToken(username, claims, 10)) : Unauthorized() ;

        }
    }
}
