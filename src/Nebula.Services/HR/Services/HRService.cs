using Grpc.Core;
using Microsoft.Extensions.Logging;
using Nebula.Services.Fragments.HR;
using Nebula.Services.HR.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.HR.Services
{
    public class HRService : HRInterface.HRInterfaceBase
    {
        private readonly ILogger<HRService> _logger;
        private readonly IEmployeeRepository _employees;
        private readonly IPayrollRepository _payrolls;
        private readonly ITimesheetRepository _timesheets;

        public HRService(ILogger<HRService> logger, IPayrollRepository payrolls, ITimesheetRepository timesheets, IEmployeeRepository employees)
        {
            _logger = logger;
            _payrolls = payrolls;
            _timesheets = timesheets;
            _employees = employees;
        }

        public override Task<GetEmployeeByIdResponse> GetEmployeeById(GetEmployeeByIdRequest request, ServerCallContext context)
        {
            return base.GetEmployeeById(request, context);
        }

        public override Task<GetEmployeesResponse> GetEmployees(GetEmployeesRequest request, ServerCallContext context)
        {
            return base.GetEmployees(request, context);
        }

        public override Task<HRMutationResponse> CreateEmployee(EmployeeRecord request, ServerCallContext context)
        {
            return base.CreateEmployee(request, context);
        }

        public override Task<HRMutationResponse> UpdateEmployee(EmployeeRecord request, ServerCallContext context)
        {
            return base.UpdateEmployee(request, context);
        }

        public override Task<HRMutationResponse> TerminateEmployee(TerminateEmployeeRequest request, ServerCallContext context)
        {
            return base.TerminateEmployee(request, context);
        }

        public override Task<HRMutationResponse> DeleteEmployee(DeleteEmployeeRequest request, ServerCallContext context)
        {
            return base.DeleteEmployee(request, context);
        }

        public override Task<GetPayrollsResponse> GetPayrollsByEmployee(GetPayrollsByEmployeeRequest request, ServerCallContext context)
        {
            return base.GetPayrollsByEmployee(request, context);
        }

        public override Task<GetPayrollsResponse> ListPayrolls(ListPayrollsRequest request, ServerCallContext context)
        {
            return base.ListPayrolls(request, context);
        }

        public override Task<HRMutationResponse> CreatePayroll(PayrollRecord request, ServerCallContext context)
        {
            return base.CreatePayroll(request, context);
        }

        public override Task<GetTimesheetsResponse> GetTimesheetsByEmployee(GetTimesheetsByEmployeeRequest request, ServerCallContext context)
        {
            return base.GetTimesheetsByEmployee(request, context);
        }

        public override Task<GetTimesheetsResponse> ListTimesheets(ListTimesheetsRequest request, ServerCallContext context)
        {
            return base.ListTimesheets(request, context);
        }

        public override Task<HRMutationResponse> LogTimesheet(TimesheetRecord request, ServerCallContext context)
        {
            return base.LogTimesheet(request, context);
        }
    }
}
