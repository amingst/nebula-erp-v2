using Nebula.Services.Fragments.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Inventory.Data
{
    public interface ILocationRepository
    {
        Task<bool> Create(Guid organizationId, LocationRecord location);
        Task<bool> Exists(Guid organizationId, Guid locationId);
        Task<LocationRecord?> GetById(Guid organizationId, Guid locationId);
        Task<bool> Delete(Guid organizationId, Guid locationId);
        Task<Guid[]> GetAllLocationIds(Guid organizationId);
        IAsyncEnumerable<LocationRecord> GetAll(Guid organizationId);
    }
}
