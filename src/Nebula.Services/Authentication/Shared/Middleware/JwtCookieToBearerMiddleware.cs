using Microsoft.AspNetCore.Http;
using Nebula.Services.Authentication.Shared.Extensions;
using Nebula.Services.Authentication.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Authentication.Shared.Middleware
{
    public class JwtCookieToBearerMiddleware
    {
        private readonly RequestDelegate next;

        public JwtCookieToBearerMiddleware(RequestDelegate next)
        {
            this.next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                string token = context.Request.Cookies[JwtExtensions.JWT_COOKIE_NAME];

                if (token != null)
                    context.Request.Headers.Append("Authorization", "Bearer " + token);
            }

            await next(context);

            var user = NebulaUserHelper.ParseUser(context);
            if (user != null)
            {
                Console.WriteLine($"**** {user.UserName} - {string.Join(',', user.Roles)} ****");
            }
        }
    }
}
