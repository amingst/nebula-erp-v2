using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nebula.Services.Base.Extensions;
using Nebula.Services.Fragments.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Organizations.Data.Postgres
{
    public class PostgresOrganizationInviteRepository : IOrganizationInviteRepository
    {
        private readonly ILogger<PostgresOrganizationInviteRepository> _logger;
        private readonly DbSet<OrganizationInviteEntity> _invites;

        public PostgresOrganizationInviteRepository(ILogger<PostgresOrganizationInviteRepository> logger, OrganizationsDbContext dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _invites = dbContext.OrganizationInvites ?? throw new ArgumentNullException(nameof(dbContext.OrganizationInvites));
        }

        public async Task<bool> AcceptInvite(Guid inviteId, Guid userId)
        {
            try
            {
                var invite = (await GetInviteById(inviteId)).ToEntity();
                if (invite == null)
                    return false;
                if (invite.UserId != userId)
                    return false;

                var now = DateTime.UtcNow;
                invite.UsedUtc = now;
                invite.ValidUntilUtc = now;

                _invites.Update(invite);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to accept invite {InviteId} for user {UserId}", inviteId, userId);
                return false;
            }
        }

        public async Task<bool> CreateInvite(OrganizationInviteRecord record)
        {
            var entity = record.ToEntity();
            _invites.Add(entity);
            return true;
        }

        public async Task<OrganizationInviteRecord> GetInviteById(Guid inviteId)
        {
            return await _invites
                .Where(i => i.InviteId == inviteId)
                .Select(i => i.ToRecord())
                .FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Invite with ID {inviteId} not found.");
        }

        public async IAsyncEnumerable<OrganizationInviteRecord> GetInvitesByOrganization(Guid organizationId)
        {
            var invites = _invites
                .Where(i => i.OrganizationId == organizationId)
                .Select(i => i.ToRecord())
                .AsAsyncEnumerable();

            await foreach (var invite in invites)
            {
                yield return invite;
            }
        }

        public async Task<bool> IsInviteValid(Guid inviteId, Guid userId)
        {
            var invite = await _invites
                .FirstOrDefaultAsync(i => i.InviteId == inviteId && i.UserId == userId);

            if (invite == null)
                return false;

            return invite.ValidUntilUtc > DateTime.UtcNow && invite.UsedUtc == default;
        }

        public async Task<bool> RevokeInvite(Guid inviteId)
        {
            var invite = await _invites
                .FirstOrDefaultAsync(i => i.InviteId == inviteId);

            if (invite == null)
                return false;

            _invites.Remove(invite);

           return true;
        }
    }
}
