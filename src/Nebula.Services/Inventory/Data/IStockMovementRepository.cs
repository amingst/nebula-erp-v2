using Nebula.Services.Fragments.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Inventory.Data
{
    public interface IStockMovementRepository
    {
        Task<bool> Create(StockMovementRecord stockMovement);
        Task<bool> Exists(Guid stockMovementId);
        Task<StockMovementRecord?> GetById(Guid stockMovementId);
        Task<bool> Delete(Guid stockMovementId);
        Task<Guid[]> GetAllStockMovementIds();
        IAsyncEnumerable<StockMovementRecord> GetAll();
    }
}
