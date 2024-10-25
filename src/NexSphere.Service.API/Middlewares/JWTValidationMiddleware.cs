using System.Threading.Tasks;
using System;
using System.Linq;
using System.Collections.Generic;

namespace NexSphere.Core.Middlewares
{
    public class JWTValidationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IAppSettings appSettings;

        public JWTValidationMiddleware(RequestDelegate next, IAppSettings appSettings)
        {
            this.next = next;
            this.appSettings = appSettings;
        }

        public async Task Invoke(HttpContext context)
        {
            if(this.appSettings.Environment == "PROD" || this.appSettings.Environment == "UAT")
            {
                if (!context.Request.Path.Value.Contains("generatetoken/audience"))
                {
                    var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(' ').Last();

                    if (string.IsNullOrWhiteSpace(token) || !ValidateToken(context, token))
                    {
                        context.Response.Clear();
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                        return;
                    }
                }
            }            

            await next(context);
        }

        private bool ValidateToken(HttpContext context, string token)
        {
            if (!string.IsNullOrWhiteSpace(token))
            {
                var jwtSetting = appSettings.Configuration.GetSection("AppConfig:JWT:SubscriberConfig").Get<IEnumerable<SubscrbierTokenModel>>();
                var subscriber = context.Request.Path.Value.Split('/')[3];
                foreach (var item in jwtSetting)
                {
                    if (item.Token.ToLower().Equals(token.ToLower()))
                    {
                        var subscribers = item.Subscribers.Split(',');

                        if (subscribers.Length > 0 && subscribers.Any(x => x.Trim().ToLower().Equals(subscriber.ToLower())))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;

        }
    }
}
