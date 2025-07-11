using Grpc.Core;
using Microsoft.Extensions.Logging;
using Nebula.Services.Fragments.Organziations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Organizations.Services
{
    public class OrganizationService : OrganizationInterface.OrganizationInterfaceBase
    {
        private readonly ILogger<OrganizationService> _logger;

        public OrganizationService(ILogger<OrganizationService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override Task<CreateOrganizationResponse> CreateOrganization(CreateOrganizationRequest request, ServerCallContext context)
        {
            return base.CreateOrganization(request, context);
        }

        public override Task<JoinOrganizationResponse> JoinOrganization(JoinOrganizationRequest request, ServerCallContext context)
        {
            return base.JoinOrganization(request, context);
        }

        public override Task<LeaveOrganizationResponse> LeaveOrganization(LeaveOrganizationRequest request, ServerCallContext context)
        {
            return base.LeaveOrganization(request, context);
        }
    }
}
