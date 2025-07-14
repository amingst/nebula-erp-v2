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
    public class MovementService : MovementInterface.MovementInterfaceBase
    {
        private readonly ILogger<MovementService> _logger;
        private readonly IStockMovementRepository _stockMovements;

        public MovementService(ILogger<MovementService> logger, IStockMovementRepository stockMovements)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _stockMovements = stockMovements ?? throw new ArgumentNullException(nameof(stockMovements));
        }

        public override Task<GetMovementHistoryResponse> GetMovementHistory(GetMovementHistoryRequest request, ServerCallContext context)
        {
            return base.GetMovementHistory(request, context);
        }

        public override Task<RecordMovementResponse> RecordMovement(RecordMovementRequest request, ServerCallContext context)
        {
            return base.RecordMovement(request, context);
        }
    }
}
