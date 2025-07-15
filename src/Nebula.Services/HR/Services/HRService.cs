using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Nebula.Services.Base.Extensions;
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

        public override async Task<GetEmployeeByIdResponse> GetEmployeeById(GetEmployeeByIdRequest request, ServerCallContext context)
        {
            var res = new GetEmployeeByIdResponse();
            Guid.TryParse(request.OrganizationId, out var orgGuid);
            if (orgGuid == Guid.Empty)
            {
                res.Error = "Invalid Organization Guid";
                return res;
            }

            Guid.TryParse(request.EmployeeId, out var empGuid);
            if (empGuid == Guid.Empty)
            {
                res.Error = "Invalid Employee Guid";
                return res;
            }

            var employee = await _employees.GetById(orgGuid, empGuid);
            if (employee is not null)
                res.Record = employee;

            res.Error = "No Error";
            return res;
        }

        public override async Task<GetEmployeesResponse> GetEmployees(GetEmployeesRequest request, ServerCallContext context)
        {
            var res = new GetEmployeesResponse();
            Guid.TryParse(request.OrganizationId, out var orgGuid);
            if (orgGuid == Guid.Empty)
            {
                res.Error = "Invalid Organization Guid";
                return res;
            }

            var employees = await _employees.GetAll(orgGuid).ToList();
            if (employees is not null && employees.Any())
                res.Records.AddRange(employees);

            res.Error = "No Error";
            return res;
        }

        public override async Task<HRMutationResponse> CreateEmployee(CreateEmployeeRequest request, ServerCallContext context)
        {
            var res = new HRMutationResponse();
            Guid.TryParse(request.OrganizationId, out var orgGuid);
            if (orgGuid == Guid.Empty)
            {
                res.Error = "Invalid Organization Guid";
                return res;
            }

            var now = Timestamp.FromDateTime(DateTime.UtcNow);
            var newEmployee = new EmployeeRecord()
            {
                EmployeeId = Guid.NewGuid().ToString(),
                OrganizationId = orgGuid.ToString(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                JobTitle = request.JobTitle,
                Department = request.Department,
                CreatedUTC = now,
                LastModifiedUTC = now
            };

            var success = await _employees.Create(newEmployee);
            if (!success)
            {
                res.Error = "Employee could not be created";
                return res;
            }

            res.Success = true;
            res.Error = "No Error";
            return res;
        }

        public override async Task<HRMutationResponse> UpdateEmployee(UpdateEmployeeRequest request, ServerCallContext context)
        {
            var res = new HRMutationResponse();

            Guid.TryParse(request.Record.OrganizationId, out var orgGuid);
            if (orgGuid == Guid.Empty)
            {
                res.Error = "Invalid Organization Guid";
                return res;
            }

            Guid.TryParse(request.Record.EmployeeId, out var empGuid);
            if (empGuid == Guid.Empty)
            {
                res.Error = "Invalid Employee Guid";
                return res;
            }

            var updatedRecord = request.Record.Clone();
            updatedRecord.LastModifiedUTC = Timestamp.FromDateTime(DateTime.UtcNow);
            updatedRecord.LastModifiedBy = string.Empty; // TODO: Parse User From Request Context

            res.Error = "No Error";
            return res;
        }

        public override async Task<HRMutationResponse> TerminateEmployee(TerminateEmployeeRequest request, ServerCallContext context)
        {
            var res = new HRMutationResponse();
            Guid.TryParse(request.OrganizationId, out var orgGuid);
            if (orgGuid == Guid.Empty)
            {
                res.Error = "Invalid Organization Guid";
                return res;
            }

            Guid.TryParse(request.EmployeeId, out var empGuid);
            if (empGuid == Guid.Empty)
            {
                res.Error = "Invalid Employee Guid";
                return res;
            }

            var employee = await _employees.GetById(orgGuid, empGuid);
            if (employee is null)
            {
                res.Error = "Employee not found";
                return res;
            }

            if (employee.DisabledUTC != null)
            {
                res.Error = "Employee is already terminated";
                return res;
            }

            var updatedEmployee = employee.Clone();
            updatedEmployee.DisabledUTC = Timestamp.FromDateTime(DateTime.UtcNow);
            updatedEmployee.DisabledBy = string.Empty; // TODO: Parse User From Request Context
            updatedEmployee.DisabledReason = request.Reason;

            var success = await _employees.Update(updatedEmployee);
            if (!success)
            {
                res.Error = "Failed To Terminate Employee";
                return res;
            }

            res.Error = "No Error";
            return res;
        }

        public override Task<HRMutationResponse> DeleteEmployee(DeleteEmployeeRequest request, ServerCallContext context)
        {
            return base.DeleteEmployee(request, context);
        }

        public override async Task<GetPayrollsResponse> GetPayrollsByEmployee(GetPayrollsByEmployeeRequest request, ServerCallContext context)
        {
            var res = new GetPayrollsResponse();
            Guid.TryParse(request.OrganizationId, out var orgGuid);
            if (orgGuid == Guid.Empty)
            {
                res.Error = "Invalid Organization Guid";
                return res;
            }

            Guid.TryParse(request.EmployeeId, out var empGuid);
            if (empGuid == Guid.Empty)
            {
                res.Error = "Invalid Employee Guid";
                return res;
            }

            var payrolls = await _payrolls.GetByEmployee(orgGuid, empGuid).ToList();
            if (payrolls is not null && payrolls.Any())
            {
                res.Records.AddRange(payrolls);
            }

            res.Error = "No Error";
            return res;
        }

        public override async Task<GetPayrollsResponse> ListPayrolls(ListPayrollsRequest request, ServerCallContext context)
        {
            var res = new GetPayrollsResponse();
            Guid.TryParse(request.OrganizationId, out var orgGuid);
            if (orgGuid == Guid.Empty)
            {
                res.Error = "Invalid Organization Guid";
                return res;
            }
            var payrolls = await _payrolls.GetAll(orgGuid).ToList();
            if (payrolls is not null && payrolls.Any())
            {
                res.Records.AddRange(payrolls);
            }

            res.Error = "No Error";
            return res;
        }

        public override async Task<CreatePayrollResponse> CreatePayroll(CreatePayrollRequest request, ServerCallContext context)
        {
            var res = new CreatePayrollResponse();
            Guid.TryParse(request.OrganizationId, out var orgGuid);
            if (orgGuid == Guid.Empty)
            {
                res.Error = "Invalid Organization Guid";
                return res;
            }

            Guid.TryParse(request.EmployeeId, out var empGuid);
            if (empGuid == Guid.Empty)
            {
                res.Error = "Invalid Employee Guid";
                return res;
            }

            var newPayroll = new PayrollRecord()
            {
                PayrollId = Guid.NewGuid().ToString(),
                OrganizationId = orgGuid.ToString(),
                EmployeeId = empGuid.ToString(),
                PayPeriod = request.PayPeriod,
                GrossPay = request.GrossPay,
                Deductions = request.Deductions,
                NetPay = request.NetPay,
                IssuedAtUTC = Timestamp.FromDateTime(DateTime.UtcNow),
                IssuedBy = string.Empty, // TODO: Parse User From Request Context
                Notes = request.Notes,
            };

            var success = await _payrolls.Create(newPayroll);
            if (!success)
            {
                res.Error = "Payroll could not be created";
                return res;
            }

            res.Record = newPayroll;
            res.Error = "No Error";
            return res;
        }

        public override async Task<GetTimesheetsResponse> GetTimesheetsByEmployee(GetTimesheetsByEmployeeRequest request, ServerCallContext context)
        {
            var res = new GetTimesheetsResponse();
            Guid.TryParse(request.OrganizationId, out var orgGuid);
            if (orgGuid == Guid.Empty)
            {
                res.Error = "Invalid Organization Guid";
                return res;
            }

            Guid.TryParse(request.EmployeeId, out var empGuid);
            if (empGuid == Guid.Empty)
            {
                res.Error = "Invalid Employee Guid";
                return res;
            }

            var timesheets = await _timesheets.GetByEmployee(orgGuid, empGuid).ToList();
            if (timesheets is not null && timesheets.Any())
            {
                res.Records.AddRange(timesheets);
            }


            res.Error = "No Error";
            return res;
        }

        public override async Task<GetTimesheetsResponse> ListTimesheets(ListTimesheetsRequest request, ServerCallContext context)
        {
            var res = new GetTimesheetsResponse();
            Guid.TryParse(request.OrganizationId, out var orgGuid);
            if (orgGuid == Guid.Empty)
            {
                res.Error = "Invalid Organization Guid";
                return res;
            }
            var timesheets = await _timesheets.GetAll(orgGuid).ToList();
            if (timesheets is not null && timesheets.Any())
            {
                res.Records.AddRange(timesheets);
            }
            res.Error = "No Error";
            return res;
        }

        public override async Task<LogTimesheetResponse> LogTimesheet(LogTimesheetRequest request, ServerCallContext context)
        {
            throw new NotImplementedException();
        }
    }
}
