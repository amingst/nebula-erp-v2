using Grpc.Core;
using Microsoft.Extensions.Logging;
using Nebula.Services.Fragments.Inventory;
using Nebula.Services.Inventory.Data;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Inventory.Services
{
    public class StockService : StockInterface.StockInterfaceBase
    {
        private readonly ILogger<StockService> _logger;
        private readonly IStockRepository _stockRepository;

        public StockService(ILogger<StockService> logger, IStockRepository stockRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _stockRepository = stockRepository ?? throw new ArgumentNullException(nameof(stockRepository));
        }

        public override async Task<GetStockForProductResponse> GetStockForProduct(GetStockForProductRequest request, ServerCallContext context)
        {
            var response = new GetStockForProductResponse();

            try
            {
                if (!Guid.TryParse(request.OrganizationId, out var orgId))
                {
                    response.Error = "Invalid OrganizationId format.";
                    return response;
                }
                if (!Guid.TryParse(request.ProductId, out var productId))
                {
                    response.Error = "Invalid ProductId format.";
                    return response;
                }

                // Here you might want to fetch stock for all locations or filter by LocationId if provided
                // For simplicity, if LocationId is provided, get stock for that location only
                if (!string.IsNullOrWhiteSpace(request.LocationId))
                {
                    if (!Guid.TryParse(request.LocationId, out var locationId))
                    {
                        response.Error = "Invalid LocationId format.";
                        return response;
                    }

                    var stockId = GenerateStockId(request.OrganizationId, request.ProductId, request.LocationId);
                    var stock = await _stockRepository.GetById(orgId, stockId);
                    if (stock != null)
                    {
                        // You can customize the response to include stock quantity or details
                        // For now, just no error means success
                        return response;
                    }
                    else
                    {
                        response.Error = "Stock record not found.";
                        return response;
                    }
                }
                else
                {
                    // LocationId not provided — you may want to aggregate stock across locations or return error
                    response.Error = "LocationId is required.";
                    return response;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetStockForProduct");
                response.Error = $"Internal error: {ex.Message}";
            }

            return response;
        }

        public override async Task<UpdateStockResponse> UpdateStock(UpdateStockRequest request, ServerCallContext context)
        {
            var response = new UpdateStockResponse();

            try
            {
                if (!Guid.TryParse(request.OrganizationId, out var orgId))
                {
                    response.Error = "Invalid OrganizationId format.";
                    return response;
                }
                if (!Guid.TryParse(request.ProductId, out var productId))
                {
                    response.Error = "Invalid ProductId format.";
                    return response;
                }
                if (!Guid.TryParse(request.LocationId, out var locationId))
                {
                    response.Error = "Invalid LocationId format.";
                    return response;
                }

                var stockId = GenerateStockId(request.OrganizationId, request.ProductId, request.LocationId);
                var existingStock = await _stockRepository.GetById(orgId, stockId);

                if (existingStock == null)
                {
                    // Create new stock record
                    var newStock = new StockRecord
                    {
                        StockId = stockId.ToString(),
                        OrganizationId = request.OrganizationId,
                        ProductId = request.ProductId,
                        LocationId = request.LocationId,
                        QuantityAvailible = (uint)request.NewQuantity
                    };

                    var created = await _stockRepository.Create(orgId, newStock);
                    if (!created)
                    {
                        response.Error = "Failed to create new stock record.";
                    }
                }
                else
                {
                    // Update existing quantity
                    existingStock.QuantityAvailible = (uint)request.NewQuantity;

                    // Assuming you have a Save method, otherwise use Create to overwrite
                    var saved = await _stockRepository.Create(orgId, existingStock);
                    if (!saved)
                    {
                        response.Error = "Failed to update stock record.";
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateStock");
                response.Error = $"Internal error: {ex.Message}";
            }

            return response;
        }

        // Deterministic GUID generator for stock entries by hashing orgId+productId+locationId
        private Guid GenerateStockId(string organizationId, string productId, string locationId)
        {
            var input = $"{organizationId}:{productId}:{locationId}";
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return new Guid(hash);
        }
    }
}
