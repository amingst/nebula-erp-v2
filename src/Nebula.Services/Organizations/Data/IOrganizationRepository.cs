using Nebula.Services.Fragments.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Organizations.Data
{
    public interface IOrganizationRepository
    {
        Task<bool> Create(OrganizationRecord organization);
        Task<bool> Exists(Guid organizationId);
        Task<OrganizationRecord?> GetById(Guid organizationId);
        Task<bool> Delete(Guid organizationId);
        Task<Guid[]> GetAllOrganizationIds();
        IAsyncEnumerable<OrganizationRecord> GetAll();
        Task<bool> Update(OrganizationRecord organization);
    }
}
