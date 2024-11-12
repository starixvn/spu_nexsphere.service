using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NexSphere.AuthenticateAPI.Services;
using NexSphere.Core.Services;
using NexSphere.Model;

namespace NexSphere.AuthenticateAPI.Controllers
{
    [Route("api")]
    [ApiController]
    public class AuthenticationController(IUserService userService
                                        , IAuthService authService) : ControllerBase
    {
        [HttpPost("register")]
        public IActionResult Register(string username, string password)
        {
            string passwordHash = Utilities.HashPassword(password);
            userService.RegisterUser(username, passwordHash);

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public IActionResult Login(string username, string password)
        {
            uint? userID = userService.LoginUser(username, password);
            if (userID == null)
                return Unauthorized("Invalid credentials.");

            var roles = userService.GetUserRoles((uint)userID);
            var token = authService.GenerateJwtToken((uint)userID, roles);

            return Ok(new UserTokenResponse(UserID: (uint)userID, AccessToken: token));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin")]
        public IActionResult GetAdminPermissions()
        {
            return Ok("Admin Permissions!");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-user-list")]
        public IActionResult GetUserList()
        {
            var userList = userService.GetUserList();

            return Ok(userList);
        }

        [Authorize]
        [HttpGet("get-user-info")]
        public IActionResult GetUserList(uint userID)
        {
            var userList = userService.GetUserInfo(userID);

            return Ok(userList);
        }


        [Authorize]
        [HttpGet("get-role/{userID}")]
        public IActionResult GetRole(int userID)
        {
            var role = userService.GetUserRole(userID);

            return Ok(role);
        }

        [Authorize]
        [HttpGet("get-refresh-token")]
        public IActionResult GetRefreshToken(uint userID)
        {
            var roles = userService.GetUserRoles(userID);
            var role = authService.GetRefreshToken(userID, roles);

            return Ok(role);
        }
    }
}
