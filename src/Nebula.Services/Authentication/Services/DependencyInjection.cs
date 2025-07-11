using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nebula.Services.Authentication.Services;
using Nebula.Services.Authentication.Services.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAuthenticationClasses(this IServiceCollection services)
        {
            services.AddSingleton<IUserRepository, FileSystemUserRepository>();
            return services;
        }

        public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGrpcService<UserService>();
        }
    }
}
