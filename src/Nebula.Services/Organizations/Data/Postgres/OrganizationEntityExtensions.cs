using Nebula.Services.Base.Extensions;
using Nebula.Services.Fragments.Organizations;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Organizations.Data.Postgres
{
    public static class OrganizationEntityExtensions
    {
        public static OrganizationEntity ToEntity(this OrganizationRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            return new OrganizationEntity
            {
                OrganizationId = record.OrganizationId.ToGuid(),
                OrganizationName = record.OrganizationName,
                OwnerId = record.OwnerId.ToGuid(),
                EmployeeIds = record.EmployeeIds.ToList(),
                CustomerIds = record.CustomerIds.ToList(),
                IsActive = true, // Default to active for new organizations
                CreatedUtc = record.CreatedUTC?.ToDateTime() ?? DateTime.UtcNow,
                CreatedBy = record.CreatedBy,
                LastModifiedUtc = record.LastModifiedUTC?.ToDateTime() ?? DateTime.UtcNow,
                LastModifiedBy = record.LastModifiedBy,
                DisabledUtc = record.DisabledUTC?.ToDateTime() ?? default(DateTime),
                DisabledBy = record.DisabledBy,
            };
        }

        public static OrganizationRecord ToRecord(this OrganizationEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var record = new OrganizationRecord
            {
                OrganizationId = entity.OrganizationId.ToString(),
                OrganizationName = entity.OrganizationName,
                OwnerId = entity.OwnerId.ToString(),
                CreatedUTC = Timestamp.FromDateTime(entity.CreatedUtc),
                CreatedBy = entity.CreatedBy,
                LastModifiedUTC = Timestamp.FromDateTime(entity.LastModifiedUtc),
                LastModifiedBy = entity.LastModifiedBy,
                DisabledBy = entity.DisabledBy,
            };
            
            // Only set DisabledUTC if it's not the default value (meaning it was actually disabled)
            if (entity.DisabledUtc != default(DateTime))
            {
                record.DisabledUTC = Timestamp.FromDateTime(entity.DisabledUtc);
            }
            
            // Add collection data
            record.EmployeeIds.AddRange(entity.EmployeeIds);
            record.CustomerIds.AddRange(entity.CustomerIds);
            
            return record;
        }
    }
}
