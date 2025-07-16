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
            Console.WriteLine($"[JWT Middleware] Processing request: {context.Request.Path}");
            Console.WriteLine($"[JWT Middleware] Has Authorization header: {context.Request.Headers.ContainsKey("Authorization")}");
            
            if (!context.Request.Headers.ContainsKey("Authorization"))
            {
                string token = context.Request.Cookies[JwtExtensions.JWT_COOKIE_NAME];
                Console.WriteLine($"[JWT Middleware] Cookie token found: {!string.IsNullOrEmpty(token)}");

                if (token != null)
                {
                    context.Request.Headers.Append("Authorization", "Bearer " + token);
                    Console.WriteLine($"[JWT Middleware] Added Authorization header from cookie");
                }
            }
            else
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                Console.WriteLine($"[JWT Middleware] Authorization header: {authHeader?.Substring(0, Math.Min(20, authHeader?.Length ?? 0))}...");
            }

            await next(context);

            var user = NebulaUserHelper.ParseUser(context);
            if (user != null)
            {
                Console.WriteLine($"**** User parsed successfully: {user.UserName} - {string.Join(',', user.Roles)} ****");
            }
            else
            {
                Console.WriteLine($"**** User parsing failed - no user found ****");
                var claims = context.User.Claims.ToArray();
                Console.WriteLine($"**** Claims count: {claims.Length} ****");
                foreach (var claim in claims)
                {
                    Console.WriteLine($"**** Claim: {claim.Type} = {claim.Value} ****");
                }
            }
        }
    }
}
