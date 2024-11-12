using NexSphere.Core.Models;
using NexSphere.Core;
using NexSphere.Core.Services;

namespace NexSphere.AuthenticateAPI.Middlewares
{
    public class JWTValidationMiddleware(RequestDelegate next, IJwtTokenService jwtTokenService, IAppSettings appSettings)
    {
        private readonly RequestDelegate next = next;
        private readonly IJwtTokenService jwtTokenService = jwtTokenService;
        private readonly IAppSettings appSettings = appSettings;

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

            var tokenSetting = new TokenSetting(SecretKey: appSettings.GatewaySecretKey, Issuer: appSettings.ApiIssuer);

            if (string.IsNullOrWhiteSpace(token) || !(await jwtTokenService.ValidateToken(tokenSetting, token)))
            {
                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                return;
            }

            await next(context);
        }

    }
}
