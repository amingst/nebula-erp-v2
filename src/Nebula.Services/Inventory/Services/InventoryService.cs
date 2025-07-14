using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Nebula.Services.Base.Extensions;
using Nebula.Services.Fragments.Inventory;
using Nebula.Services.Inventory.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Inventory.Services
{
    public class InventoryService : InventoryInterface.InventoryInterfaceBase
    {
        private readonly ILogger<InventoryService> _logger;
        private readonly IProductRepository _products;

        public InventoryService(ILogger<InventoryService> logger, IProductRepository products)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _products = products ?? throw new ArgumentNullException(nameof(products));
        }

        public override async Task<CreateProductResponse> CreateProduct(CreateProductRequest request, ServerCallContext context)
        {
            var res = new CreateProductResponse();
            var now = Timestamp.FromDateTime(DateTime.UtcNow);
            var newProduct = new ProductRecord
            {
                ProductId = Guid.NewGuid().ToString(),
                OrganizationId = request.OrganizationId,
                Name = request.Name,
                Sku = request.Sku,
                Description = request.Description,
                ImageAssetId = request.ImageAssetId,
                Unit = request.Unit,
                CreatedUTC = now,
                CreatedBy = string.Empty,
                LastModifiedUTC = now,
                LastModifiedBy = string.Empty,
            };

            var success = await _products.Create(newProduct);
            if (!success)
            {
                res.Error = "Product could not be created";
                return res;
            }

            res.Error = "No Error";
            res.Record = newProduct;
            return res;
        }

        public override async Task<GetProductByIdResponse> GetProductById(GetProductByIdRequest request, ServerCallContext context)
        {
            var res = new GetProductByIdResponse();
            Guid.TryParse(request.ProductId, out var prodId);
            if (prodId == Guid.Empty)
            {
                res.Error = "Invalid Product ID";
                return res;
            }

            Guid.TryParse(request.OrganizationId, out var orgId);
            if (orgId == Guid.Empty)
            {
                res.Error = "Invalid Organization ID";
                return res;
            }

            var product = await _products.GetById(orgId, prodId);
            if (product == null || product.OrganizationId != orgId.ToString())
            {
                res.Error = "Product not found or does not belong to the organization";
                return res;
            }  

            res.Record = product;
            res.Error = "No Error";
            return res;
        }

        public override async Task<ListProductsResponse> ListProducts(ListProductsRequest request, ServerCallContext context)
        {
            var res = new ListProductsResponse();
            Guid.TryParse(request.OrganizationId, out var orgId);
            if (orgId == Guid.Empty)
                return res;

            var products = await _products.GetAll(orgId).ToList();
            if (products == null || !products.Any())
            {
                //res.Error = "No products found for the organization";
                return res;
            }

            res.Records.AddRange(products);
            return res;
        }

        public override Task<UpdateProductResponse> UpdateProduct(UpdateProductRequest request, ServerCallContext context)
        {
            return base.UpdateProduct(request, context);
        }
    }
}
