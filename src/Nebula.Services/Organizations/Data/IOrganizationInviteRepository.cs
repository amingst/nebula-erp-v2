using Nebula.Services.Fragments.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Organizations.Data
{
    public interface IOrganizationInviteRepository
    {
        Task<bool> CreateInvite(OrganizationInviteRecord record);
        Task<bool> AcceptInvite(Guid inviteId, Guid userId);
        Task<bool> RevokeInvite(Guid inviteId);
        Task<bool> IsInviteValid(Guid inviteId, Guid userId);
        IAsyncEnumerable<OrganizationInviteRecord> GetInvitesByOrganization(Guid organizationId);
        Task<OrganizationInviteRecord?> GetInviteById(Guid inviteId);
    }
}
