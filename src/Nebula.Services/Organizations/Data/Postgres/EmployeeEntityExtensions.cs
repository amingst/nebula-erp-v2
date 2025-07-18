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
    public static class EmployeeEntityExtensions
    {
        public static EmployeeEntity ToEntity(this EmployeeRecord record)
        {
            if (record == null) throw new ArgumentNullException(nameof(record));
            return new EmployeeEntity
            {
                Id = record.EmployeeId.ToGuid(),
                OrganizationId = record.OrganizationId.ToGuid(),
                UserId = record.UserId.ToGuid(),
                FirstName = record.FirstName ?? string.Empty,
                LastName = record.LastName ?? string.Empty,
                Email = record.Email ?? string.Empty,
                Phone = record.Phone ?? string.Empty,
                JobTitle = record.JobTitle ?? string.Empty,
                Department = record.Department ?? string.Empty,
                StartUtc = record.StartUTC?.ToDateTime() ?? DateTime.UtcNow,
                EndUtc = record.EndUTC?.ToDateTime() ?? DateTime.MinValue,
                IsActive = record.IsActive,
                ManagerId = string.IsNullOrEmpty(record.ManagerId) ? null : record.ManagerId.ToGuid(),
                CreatedUtc = record.CreatedUTC?.ToDateTime() ?? DateTime.UtcNow,
                CreatedBy = record.CreatedBy ?? string.Empty,
                LastModifiedUtc = record.LastModifiedUTC?.ToDateTime() ?? DateTime.UtcNow,
                LastModifiedBy = record.LastModifiedBy ?? string.Empty,
                DisabledUtc = record.DisabledUTC?.ToDateTime() ?? DateTime.MinValue,
                DisabledBy = record.DisabledBy ?? string.Empty,
                DisabledReason = record.DisabledReason ?? string.Empty
            };
        }

        public static EmployeeRecord ToRecord(this EmployeeEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var record = new EmployeeRecord
            {
                EmployeeId = entity.Id.ToString(),
                OrganizationId = entity.OrganizationId.ToString(),
                UserId = entity.UserId.ToString(),
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Email = entity.Email,
                Phone = entity.Phone,
                JobTitle = entity.JobTitle,
                Department = entity.Department,
                StartUTC = Timestamp.FromDateTime(entity.StartUtc),
                IsActive = entity.IsActive,
                ManagerId = entity.ManagerId?.ToString() ?? string.Empty,
                CreatedUTC = Timestamp.FromDateTime(entity.CreatedUtc),
                CreatedBy = entity.CreatedBy,
                LastModifiedUTC = Timestamp.FromDateTime(entity.LastModifiedUtc),
                LastModifiedBy = entity.LastModifiedBy,
                DisabledBy = entity.DisabledBy,
                DisabledReason = entity.DisabledReason
            };

            // Handle optional end date and disabled date
            if (entity.EndUtc != DateTime.MinValue)
                record.EndUTC = Timestamp.FromDateTime(entity.EndUtc);
                
            if (entity.DisabledUtc != DateTime.MinValue)
                record.DisabledUTC = Timestamp.FromDateTime(entity.DisabledUtc);

            return record;
        }
    }
}
