using Nebula.Services.Fragments.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Inventory.Data
{
    public interface ISupplierRepository
    {
        Task<bool> Create(Guid organizationId, SupplierRecord supplier);
        Task<bool> Exists(Guid organizationId, Guid supplierId);
        Task<SupplierRecord?> GetById(Guid organizationId, Guid supplierId);
        Task<bool> Delete(Guid organizationId, Guid supplierId);
        Task<Guid[]> GetAllSupplierIds(Guid organizationId);
        IAsyncEnumerable<SupplierRecord> GetAll(Guid organizationId);
    }

}
