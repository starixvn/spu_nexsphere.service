using NexSphere.Core;
using NexSphere.Core.Models;
using NexSphere.Core.Services;
using Ocelot.Values;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace NexSphere.Gateway.Middlewares
{
    public class JWTValidationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IJwtTokenService jwtTokenService;
        private readonly IAppSettings appSettings;

        public JWTValidationMiddleware(RequestDelegate next, IJwtTokenService jwtTokenService, IAppSettings appSettings)
        {
            this.next = next;
            this.jwtTokenService = jwtTokenService;
            this.appSettings = appSettings;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Value.Contains("authenticate"))
            {
                string token = GenerateAuthenticateAPIToken();

                context.Request.Headers.Append("Authorization", $"Bearer {token}");
            }

            await next(context);
        }

        private string GenerateAuthenticateAPIToken()
        {
            var tokenSetting = new TokenSetting()
            {
                SecretKey = appSettings.ApiSecretKey,
                Issuer = appSettings.ApiIssuer
            };

            string username = "gateway";
            int expireMinutes = 5;
            var claims = new List<Claim>()
            {
                    new Claim("username", username)
            };
            return jwtTokenService.GenerateToken(tokenSetting, username, claims, expireMinutes);
        }

    }
}
