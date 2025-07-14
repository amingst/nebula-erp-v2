using Grpc.Core;
using Microsoft.Extensions.Logging;
using Nebula.Services.Fragments.Inventory;
using Nebula.Services.Inventory.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Inventory.Services
{
    public class BatchService : BatchInterface.BatchInterfaceBase
    {
        private readonly ILogger<BatchService> _logger;
        private readonly IBatchRepository _batches;

        public BatchService(ILogger<BatchService> logger, IBatchRepository batches)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _batches = batches ?? throw new ArgumentNullException(nameof(batches));
        }

        public override Task<CreateBatchResponse> CreateBatch(CreateBatchRequest request, ServerCallContext context)
        {
            return base.CreateBatch(request, context);
        }

        public override Task<GetBatchesResponse> GetBatches(GetBatchesRequest request, ServerCallContext context)
        {
            return base.GetBatches(request, context);
        }
    }
}
