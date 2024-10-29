using NexSphere.Core.Services;

namespace NexSphere.AuthenticateAPI.Models
{
    public class JWTValidationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IJwtTokenService jwtTokenService;

        public JWTValidationMiddleware(RequestDelegate next, IJwtTokenService jwtTokenService)
        {
            this.next = next;
            this.jwtTokenService = jwtTokenService;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            if (string.IsNullOrWhiteSpace(token) || !(await jwtTokenService.ValidateToken(token)))
            {
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                return;
            }

            await next(context);
        }

    }
}
