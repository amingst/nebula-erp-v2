using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Nebula.Services.Base.Extensions;
using Nebula.Services.Fragments.Organizations;
using Nebula.Services.Organizations.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nebula.Services.HR.Services
{
    public class EmployeeService : EmployeeInterface.EmployeeInterfaceBase
    {
        private readonly ILogger<EmployeeService> _logger;
        private readonly IEmployeeRepository _employees;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employees)
        {
            _logger = logger;
            _employees = employees;
        }

        public override Task<HRMutationResponse> CreateEmployee(CreateEmployeeRequest request, ServerCallContext context)
        {
            return base.CreateEmployee(request, context);
        }

        public override Task<HRMutationResponse> DeleteEmployee(DeleteEmployeeRequest request, ServerCallContext context)
        {
            return base.DeleteEmployee(request, context);
        }

        public override Task<GetEmployeeByIdResponse> GetEmployeeById(GetEmployeeByIdRequest request, ServerCallContext context)
        {
            return base.GetEmployeeById(request, context);
        }

        public override Task<GetEmployeesResponse> GetEmployees(GetEmployeesRequest request, ServerCallContext context)
        {
            return base.GetEmployees(request, context);
        }

        public override Task<HRMutationResponse> TerminateEmployee(TerminateEmployeeRequest request, ServerCallContext context)
        {
            return base.TerminateEmployee(request, context);
        }

        public override Task<HRMutationResponse> UpdateEmployee(UpdateEmployeeRequest request, ServerCallContext context)
        {
            return base.UpdateEmployee(request, context);
        }
    }
}
