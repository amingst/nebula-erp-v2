using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Authentication.Shared.Helpers
{
    public class NebulaUserHelper
    {
        public readonly NebulaUser MyUser;
        public readonly bool IsLoggedIn;
        public readonly Guid MyUserId;

        public NebulaUserHelper(IHttpContextAccessor httpContextAccessor)
        {
            MyUser = ParseUser(httpContextAccessor.HttpContext);
            IsLoggedIn = MyUser != null;
            MyUserId = MyUser?.Id ?? Guid.Empty;
        }

        public static NebulaUser ParseUser(HttpContext context)
        {
            var claims = context.User.Claims.ToArray();
            var user = NebulaUser.Parse(claims);
            if (user != null)
                user.JwtToken = GrabToken(context);

            return user;
        }

        private static string GrabToken(HttpContext context)
        {
            string authorization = context.Request.Headers["Authorization"];

            if (string.IsNullOrWhiteSpace(authorization))
                return "";

            if (!authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                return "";

            return authorization.Substring(7).Trim();
        }
    }
}
