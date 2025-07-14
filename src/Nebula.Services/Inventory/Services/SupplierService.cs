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
    public class SupplierService : SupplierInterface.SupplierInterfaceBase
    {
        private readonly ILogger<SupplierService> _logger;
        private readonly ISupplierRepository _suppliers;

        public SupplierService(ILogger<SupplierService> logger, ISupplierRepository suppliers)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _suppliers = suppliers ?? throw new ArgumentNullException(nameof(suppliers));
        }

        public override async Task<CreateSupplierResponse> CreateSupplier(CreateSupplierRequest request, ServerCallContext context)
        {
            var res = new CreateSupplierResponse();
            Guid.TryParse(request.OrganizationId, out var orgId);
            if (orgId == Guid.Empty)
            {
                res.Error = "Invalid Organization ID";
                return res;
            }

            var newSupplier = new SupplierRecord
            {
                SupplierId = Guid.NewGuid().ToString(),
                OrganizationId = orgId.ToString(),
                Name = request.Name,
                ContactEmail = request.ContactEmail,
                Phone = request.Phone,
                Address = request.Address,
                CreatedUTC = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
                CreatedBy = string.Empty,
                LastModifiedUTC = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.UtcNow),
                LastModifiedBy = string.Empty
            };

            var success = await _suppliers.Create(orgId, newSupplier);
            if (!success)
            {
                res.Error = "Failed To Create Supplier";
                return res;
            }

            res.Record = newSupplier;
            res.Error = "No Error";
            return res;
        }

        public override async Task<ListSuppliersResponse> ListSuppliers(ListSuppliersRequest request, ServerCallContext context)
        {
            var res = new ListSuppliersResponse();
            Guid.TryParse(request.OrganizationId, out var orgId);
            if (orgId == Guid.Empty)
            {
                res.Error = "Invalid Organization ID";
                return res;
            }

            var suppliers = await _suppliers.GetAll(orgId).ToList();
            if (suppliers == null || !suppliers.Any())
            {
                res.Error = "No Suppliers Found";
                return res;
            }

            res.Error = "No Error";
            res.Suppliers.AddRange(suppliers);
            return res;
        }
    }
}
