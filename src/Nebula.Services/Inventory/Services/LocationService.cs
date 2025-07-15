using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Nebula.Services.Base.Extensions;
using Nebula.Services.Inventory.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nebula.Services.Fragments.Inventory;

namespace Nebula.Services.Inventory.Services
{
    public class LocationService : LocationInterface.LocationInterfaceBase
    {
        private readonly ILogger<LocationService> _logger;
        private readonly ILocationRepository _locations;

        public LocationService(ILogger<LocationService> logger, ILocationRepository locations)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _locations = locations ?? throw new ArgumentNullException(nameof(locations));
        }

        public override async Task<CreateLocationResponse> CreateLocation(CreateLocationRequest request, ServerCallContext context)
        {
            var res = new CreateLocationResponse();
            Guid.TryParse(request.OrganizationId, out var orgId);
            if (orgId == Guid.Empty)
            {
                res.Error = "Invalid Organization ID";
                return res;
            }

            var now = Timestamp.FromDateTime(DateTime.UtcNow);
            var newLocation = new LocationRecord
            {
                LocationId = Guid.NewGuid().ToString(),
                OrganizationId = request.OrganizationId,
                Name = request.Name,
                Type = request.Type,
                Address = request.Address,
                CreatedUTC = now,
                CreatedBy = string.Empty,
                LastModifiedUTC = now,
                LastModifiedBy = string.Empty
            };

            var success = await _locations.Create(orgId, newLocation);
            if (!success)
            {
                res.Error = "Location could not be created";
                return res;
            }

            res.Record = newLocation;
            res.Error = "No Error";
            return res;
        }

        public override async Task<ListLocationsResponse> ListLocations(ListLocationsRequest request, ServerCallContext context)
        {
            var res = new ListLocationsResponse();

            Guid.TryParse(request.OrganizationId, out var orgId);
            if (orgId == Guid.Empty)
            {
                res.Error = "Invalid Organization ID";
                return res;
            }

            var records = await _locations.GetAll(orgId).ToList();
            if (records == null || !records.Any())
            {
                res.Error = "No locations found for the organization";
                return res;
            }

            res.Records.AddRange(records);
            res.Error = "No Error";
            return res;
        }
    }
}
