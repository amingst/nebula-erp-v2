using Nebula.Services.Fragments.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Inventory.Data
{
    public interface IProductRepository
    {
        Task<bool> Create(ProductRecord product);
        Task<bool> Exists(Guid productId);
        Task<ProductRecord?> GetById(Guid organizationId, Guid productId);
        Task<bool> Delete(Guid productId);
        Task<Guid[]> GetAllProductIds(Guid organizationId);
        IAsyncEnumerable<ProductRecord> GetAll(Guid organizationId);
    }
}
