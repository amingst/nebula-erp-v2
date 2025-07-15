using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nebula.Services.HR.Data;
using Nebula.Services.HR.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddHRClasses(this IServiceCollection services)
        {
            services.AddSingleton<IEmployeeRepository, FileSystemEmployeeRepository>();
            services.AddSingleton<IPayrollRepository, FileSystemPayrollRepository>();
            services.AddSingleton<ITimesheetRepository, FileSystemTimesheetRepository>();
            return services;
        }

        public static void MapHREndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGrpcService<EmployeeService>();
            endpoints.MapGrpcService<PayrollService>();
            endpoints.MapGrpcService<TimesheetService>();
        }
    }
}
