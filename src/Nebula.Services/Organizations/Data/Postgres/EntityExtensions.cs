using Google.Protobuf.WellKnownTypes;
using Nebula.Services.Base.Extensions;
using Nebula.Services.Fragments.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Organizations.Data.Postgres
{
    public static class EntityExtensions
    {
        public static OrganizationInviteRecord ToRecord(this OrganizationInviteEntity entity)
        {
            if (entity == null) return null;
            return new OrganizationInviteRecord
            {
                InviteId = entity.InviteId.ToString(),
                OrganizationId = entity.OrganizationId.ToString(),
                UserId = entity.UserId.ToString(),
                InvitedUTC = Timestamp.FromDateTime(entity.InvitedUtc.ToUniversalTime()),
                InvitedBy = entity.InvitedBy,
                ValidUntilUTC = Timestamp.FromDateTime(entity.ValidUntilUtc.ToUniversalTime()),
                UsedUTC = entity.UsedUtc == default ? null : Timestamp.FromDateTime(entity.UsedUtc.ToUniversalTime())
            };
        }

        public static OrganizationInviteEntity ToEntity(this OrganizationInviteRecord record)
        {
            if (record == null) return null;
            return new OrganizationInviteEntity
            {
                InviteId = Guid.Parse(record.InviteId),
                OrganizationId = Guid.Parse(record.OrganizationId),
                UserId = Guid.Parse(record.UserId),
                InvitedUtc = record.InvitedUTC.ToDateTime(),
                InvitedBy = record.InvitedBy,
                ValidUntilUtc = record.ValidUntilUTC.ToDateTime(),
                UsedUtc = record.UsedUTC?.ToDateTime() ?? default
            };
        }

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
