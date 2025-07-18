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
        private readonly IOrganizationInviteRepository _invites;

        public OrganizationService(ILogger<OrganizationService> logger, IOrganizationRepository organizations, IEmployeeRepository employees, IOrganizationInviteRepository invites)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _organizations = organizations ?? throw new ArgumentNullException(nameof(organizations));
            _employees = employees;
            _invites = invites;
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
                FirstName = createdByUser.FirstName,
                LastName = createdByUser.LastName,
                Email = "", // Could be extracted from user claims if needed
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

            // TODO: Add Scoped Org Role to the user

            return new CreateOrganizationResponse()
            {
                Record = newOrg,
                Error = "No Error"
            };
        }

        public override async Task<InviteUserResponse> InviteUser(InviteUserRequest request, ServerCallContext context)
        {
            var res = new InviteUserResponse();

            Guid.TryParse(request.UserId, out var userId);
            if (userId == Guid.Empty)
            {
                res.Error = "Invalid User ID";
                return res;
            }

            Guid.TryParse(request.OrganizationId, out var orgId);
            if (orgId == Guid.Empty)
            {
                res.Error = "Invalid Organization ID";
                return res;
            }

            var orgExists = await _organizations.Exists(orgId);
            if (!orgExists)
            {
                res.Error = "Organization does not exist";
                return res;
            }

            var newInvite = new OrganizationInviteRecord
            {
                InviteId = Guid.NewGuid().ToString(),
                OrganizationId = orgId.ToString(),
                UserId = userId.ToString(),
                InvitedBy = NebulaUserHelper.ParseUser(context.GetHttpContext())?.Id.ToString(),
                InvitedUTC = Timestamp.FromDateTime(DateTime.UtcNow),
                ValidUntilUTC = request.ValidUntilUTC ?? Timestamp.FromDateTime(DateTime.UtcNow.AddDays(7)),
            };

            var inviteCreated = await _invites.CreateInvite(newInvite);
            if (!inviteCreated)
            {
                res.Error = "Failed to create organization invite";
                return res;
            }

            res.Record = newInvite;
            res.Error = "No Error";
            return res;
        }

        public override async Task<JoinOrganizationResponse> JoinOrganization(JoinOrganizationRequest request, ServerCallContext context)
        {
            var res = new JoinOrganizationResponse();

            var user = NebulaUserHelper.ParseUser(context.GetHttpContext());
            if (user == null)
            {
                res.Error = "User not authenticated";
                return res;
            }

            Guid.TryParse(request.InviteId, out var inviteId);
            if (inviteId == Guid.Empty)
            {
                res.Error = "Invalid Invite ID";
                return res;
            }

            var invite = await _invites.GetInviteById(inviteId);
            if (invite == null)
            {
                res.Error = "Invite not found";
                return res;
            }

            var isValid = await _invites.IsInviteValid(inviteId, user.Id);
            if (!isValid)
            {
                res.Error = "Invite is not valid or has expired";
                return res;
            }

            var accepted = await _invites.AcceptInvite(inviteId, user.Id);
            if (!accepted)
            {
                res.Error = "Failed to accept invite";
                return res;
            }

            var organization = await _organizations.GetById(Guid.Parse(invite.OrganizationId));
            if (organization == null)
            {
                res.Error = "Organization not found";
                return res;
            }

            var newEmployee = new EmployeeRecord
            {
                EmployeeId = Guid.NewGuid().ToString(),
                OrganizationId = invite.OrganizationId,
                UserId = user.Id.ToString(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                StartUTC = Timestamp.FromDateTime(DateTime.UtcNow),
                IsActive = true,
                CreatedUTC = Timestamp.FromDateTime(DateTime.UtcNow),
                CreatedBy = user.Id.ToString(),
                LastModifiedUTC = Timestamp.FromDateTime(DateTime.UtcNow),
                LastModifiedBy = user.Id.ToString(),
            };

            var employeeCreated = await _employees.Create(newEmployee);
            if (!employeeCreated)
            {
                res.Error = "Failed to create employee record";
                return res;
            }

            organization.EmployeeIds.Add(newEmployee.EmployeeId);
            var orgUpdated = await _organizations.Update(organization);
            if (!orgUpdated)
            {
                res.Error = "Failed to update organization with new employee";
                return res;
            }

            // TODO: Add Scoped Org Role to the user

            res.Error = "No Error";
            return res;
        }

        public override Task<LeaveOrganizationResponse> LeaveOrganization(LeaveOrganizationRequest request, ServerCallContext context)
        {
            return base.LeaveOrganization(request, context);
        }

        public override async Task<GetOrganizationsForUserResponse> GetOrganizationsForUser(GetOrganizationsForUserRequest request, ServerCallContext context)
        {
            var res = new GetOrganizationsForUserResponse();

            var user = NebulaUserHelper.ParseUser(context.GetHttpContext());
            if (user == null)
            {
                res.Error = "User not authenticated";
                return res;
            }

            var orgs = await _organizations.GetAll().ToList();
            if (orgs == null || orgs.Count == 0)
            {
                res.Error = "No organizations found";
                return res;
            }

            foreach (var org in orgs)
            {
                if (org.OwnerId == user.Id.ToString() || org.EmployeeIds.Contains(user.Id.ToString()))
                {
                    res.Records.Add(org);
                }
            }

            res.Error = "No Error";
            return res;
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
