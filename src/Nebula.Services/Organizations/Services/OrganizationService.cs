using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Nebula.Services.Authentication.Shared.Helpers;
using Nebula.Services.Base.Extensions;
using Nebula.Services.Fragments.Organizations;
using Nebula.Services.Fragments.Organziations;
using Nebula.Services.Organizations.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Organizations.Services
{
    [Authorize]
    public class OrganizationService : OrganizationInterface.OrganizationInterfaceBase
    {
        private readonly ILogger<OrganizationService> _logger;
        private readonly IOrganizationRepository _organizations;
        private readonly IEmployeeRepository _employees;

        public OrganizationService(ILogger<OrganizationService> logger, IOrganizationRepository organizations, IEmployeeRepository employees)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _organizations = organizations ?? throw new ArgumentNullException(nameof(organizations));
            _employees = employees;
        }

        public override async Task<CreateOrganizationResponse> CreateOrganization(CreateOrganizationRequest request, ServerCallContext context)
        {
            var newOrg = new OrganizationRecord
            {
                OrganizationId = Guid.NewGuid().ToString(),
                OrganizationName = request.OrganizationName,
            };

            var createdByUser = NebulaUserHelper.ParseUser(context.GetHttpContext());
            if (createdByUser == null)
            {
                return new CreateOrganizationResponse
                {
                    Error = "User not authenticated"
                };
            }
            var createdById = createdByUser?.Id.ToString();
            newOrg.OwnerId = createdById;
            newOrg.CreatedBy = createdById;
            newOrg.LastModifiedBy = createdById;

            var now = Timestamp.FromDateTime(DateTime.UtcNow);
            newOrg.CreatedUTC = now;
            newOrg.LastModifiedUTC = now;

            var ownerEmployeeId = Guid.NewGuid().ToString();
            var ownerEmployee = new EmployeeRecord()
            {
                EmployeeId = ownerEmployeeId,
                OrganizationId = newOrg.OrganizationId,
                StartUTC = now,
                IsActive = true,
                UserId = createdById,
                CreatedUTC = now,
                CreatedBy = createdById,
                LastModifiedUTC = now,
                LastModifiedBy = createdById,
            };

            newOrg.EmployeeIds.Add(ownerEmployee.EmployeeId);

            var orgSuccess = await _organizations.Create(newOrg);
            var employeeSuccess = await _employees.Create(ownerEmployee);

            if (!orgSuccess)
                return new CreateOrganizationResponse
                {
                    Error = "Organization could not be created"
                };

            if (!employeeSuccess)
                return new CreateOrganizationResponse
                {
                    Error = "Employee could not be created for the organization owner"
                };

            return new CreateOrganizationResponse()
            {
                Record = newOrg,
                Error = "No Error"
            };
        }

        public override Task<JoinOrganizationResponse> JoinOrganization(JoinOrganizationRequest request, ServerCallContext context)
        {
            return base.JoinOrganization(request, context);
        }

        public override Task<LeaveOrganizationResponse> LeaveOrganization(LeaveOrganizationRequest request, ServerCallContext context)
        {
            return base.LeaveOrganization(request, context);
        }

        public override async Task<GetOrganizationResponse> GetOrganization(GetOrganizationRequest request, ServerCallContext context)
        {
            var res = new GetOrganizationResponse();

            Guid.TryParse(request.OrganizationId, out var orgId);
            if (orgId == Guid.Empty)
            {
                res.Error = "Invalid Organization ID";
                return res;
            }

            var org = await _organizations.GetById(orgId);
            if (org == null)
            {
                res.Error = "Organization not found";
                return res;
            }

            res.Record = org;
            res.Error = "No Error";
            return res;
        }

        public override Task<GetEmployeeByIdResponse> GetEmployeeById(GetEmployeeByIdRequest request, ServerCallContext context)
        {
            return base.GetEmployeeById(request, context);
        }

        public override async Task<GetEmployeesResponse> GetEmployees(GetEmployeesRequest request, ServerCallContext context)
        {
            Guid.TryParse(request.OrganizationId, out var orgId);
            if (orgId == Guid.Empty)
            {
                return new GetEmployeesResponse
                {
                    Error = "Invalid organization ID"
                };
            }

            var res = new GetEmployeesResponse();

            var employees = await _employees.GetAll(orgId).ToList();
            if (employees != null && employees.Count > 0)
            {
                res.Records.AddRange(employees);
            }

            res.Error = "No Error";
            return res;
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
