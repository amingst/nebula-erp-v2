﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nebula.Services.Organizations.Data;
using Nebula.Services.Organizations.Data.Postgres;
using Nebula.Services.Organizations.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Organizations.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddOrganizationClasses(this IServiceCollection services)
        {
            //services.AddSingleton<IOrganizationRepository, FileSystemOrganizationRepository>();
            //services.AddSingleton<IEmployeeRepository, FileSystemEmployeeRepository>();
            return services;
        }

        public static IServiceCollection AddOrganizationsDb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IOrganizationRepository, PostgresOrganizationRepository>();
            services.AddScoped<IEmployeeRepository, PostgresEmployeeRepository>();
            services.AddScoped<IOrganizationInviteRepository, PostgresOrganizationInviteRepository>();
            services.AddDbContext<OrganizationsDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("OrganizationDatabase")), ServiceLifetime.Scoped);
            services.AddScoped<OrganizationsDbContextHelper>();

            return services;
        }

        public static void MapOrganizationEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGrpcService<OrganizationService>();
        }
    }
}
