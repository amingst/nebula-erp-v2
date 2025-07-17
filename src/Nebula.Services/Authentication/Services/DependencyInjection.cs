using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Nebula.Authentication.Services;
using Nebula.Services.Authentication.Services;
using Nebula.Services.Authentication.Services.Data;
using Nebula.Services.Authentication.Services.Data.Postgres;
using Nebula.Services.Authentication.Shared.Helpers;
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
            //services.AddSingleton<IUserRepository, FileSystemUserRepository>();
            services.AddScoped<NebulaUserHelper>();
            services.AddScoped<ClaimsClient>();
            return services;
        }

        public static IServiceCollection AddAuthenticationDb(
            this IServiceCollection services,
            IConfiguration configuration
            )
        {
            services.AddDbContext<AuthDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("AuthDatabase")));

            // Keep existing file repository for now, add postgres as alternative
            services.AddScoped<IUserRepository, PostgresUserRepository>();

            return services;
        }

        public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGrpcService<UserService>();
        }
    }
}
