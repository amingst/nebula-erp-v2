using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Nebula.Services.Accounting.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAccountingClasses(this IServiceCollection services)
        {
            return services;
        }

        public static void MapAccountingEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGrpcService<AccountingService>();
        }
    }
}
