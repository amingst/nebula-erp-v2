using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nebula.Services.Inventory.Data;
using Nebula.Services.Inventory.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInventoryClasses(this IServiceCollection services)
        {
            services.AddSingleton<ILocationRepository, FileSystemLocationRepository>();
            services.AddSingleton<ISupplierRepository, FileSystemSupplierRepository>();
            services.AddSingleton<IProductRepository, FileSystemProductRepository>();
            services.AddSingleton<IBatchRepository, FileSystemBatchRepository>();
            services.AddSingleton<IStockMovementRepository, FileSystemStockMovementRepository>();
            services.AddSingleton<IStockRepository, FileSystemStockRepository>();
            return services;
        }

        public static void MapInventoryEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapGrpcService<InventoryService>();
            endpoints.MapGrpcService<BatchService>();
            endpoints.MapGrpcService<LocationService>();
            endpoints.MapGrpcService<MovementService>();
            endpoints.MapGrpcService<StockService>();
            endpoints.MapGrpcService<SupplierService>();
        }
    }
}
