using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexSphere.AuthenticateAPI.Services;
using NexSphere.Core;
using NexSphere.Core.Models;
using NexSphere.Core.Services;
using NexSphere.Model;
using System.Security.Claims;

namespace NexSphere.AuthenticateAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthenticationController(IAppSettings appSettings, IIdentityService identityService, IAuthService authService) : ControllerBase
    {
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(string username, string password)
        {
            string passwordHash = Utilities.HashPassword(password);
            identityService.RegisterUser(username, passwordHash);

            return Ok("User registered successfully.");
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login(string username)
        {
            int? userId = identityService.LoginUser(username);
            if (userId == null)
                return Unauthorized("Invalid credentials.");

            var roles = authService.GetUserRoles((int)userId);
            var token = authService.GenerateJwtToken(username, roles);

            return Ok(new { UserId = userId, AccessToken = token });
        }

        [Authorize]
        [HttpGet("role/{userId}")]
        public IActionResult GetRole(int userId)
        {
            var role = identityService.GetUserRole(userId);

            return Ok(role);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult GetAdminPermissions(int userId)
        {
            return Ok("Admin!");
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
