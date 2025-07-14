using Nebula.Services.Fragments.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Inventory.Data
{
    public interface IStockRepository
    {
        Task<bool> Create(Guid organizationId, StockRecord stock);
        Task<bool> Exists(Guid organizationId, Guid stockId);
        Task<StockRecord?> GetById(Guid organizationId, Guid stockId);
        Task<bool> Delete(Guid organizationId, Guid stockId);
        Task<Guid[]> GetAllStockIds(Guid organizationId);
        IAsyncEnumerable<StockRecord> GetAll(Guid organizationId);
    }

}
