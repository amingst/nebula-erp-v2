using Nebula.Services.Fragments.Organizations;
using Nebula.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.WellKnownTypes;

namespace Nebula.Services.Organizations.Data.Postgres
{
    public static class OrganizationInviteEntityExtensions
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
    }
}
