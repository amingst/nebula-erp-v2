using Nebula.Services.Fragments.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Inventory.Data
{
    public interface IBatchRepository
    {
        Task<bool> Create(BatchRecord batch);
        Task<bool> Exists(Guid batchId);
        Task<BatchRecord?> GetById(Guid batchId);
        Task<bool> Delete(Guid batchId);
        Task<Guid[]> GetAllBatchIds();
        IAsyncEnumerable<BatchRecord> GetAll();
    }
}
