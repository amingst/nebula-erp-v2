using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly OrganizationsDbContext _context;

        public PostgresOrganizationInviteRepository(ILogger<PostgresOrganizationInviteRepository> logger, OrganizationsDbContext dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public Task<bool> AcceptInvite(Guid inviteId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> CreateInvite(OrganizationInviteRecord record)
        {
            var entity = record.ToEntity();
            _context.OrganizationInvites.Add(entity);
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create organization invite {InviteId}", record.InviteId);
                return false;
            }
        }

        public async Task<OrganizationInviteRecord> GetInviteById(Guid inviteId)
        {
            return await _context.OrganizationInvites
                .Where(i => i.InviteId == inviteId)
                .Select(i => i.ToRecord())
                .FirstOrDefaultAsync() ?? throw new KeyNotFoundException($"Invite with ID {inviteId} not found.");
        }

        public async IAsyncEnumerable<OrganizationInviteRecord> GetInvitesByOrganization(Guid organizationId)
        {
            var invites = _context.OrganizationInvites
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
            var invite = await _context.OrganizationInvites
                .FirstOrDefaultAsync(i => i.InviteId == inviteId && i.UserId == userId);

            if (invite == null)
                return false;

            // Check if the invite is still valid
            return invite.ValidUntilUtc > DateTime.UtcNow && invite.UsedUtc == default;
        }

        public async Task<bool> RevokeInvite(Guid inviteId)
        {
            var invite = await _context.OrganizationInvites
                .FirstOrDefaultAsync(i => i.InviteId == inviteId);

            if (invite == null)
                return false;

            _context.OrganizationInvites.Remove(invite);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to revoke invite {InviteId}", inviteId);
                return false;
            }
        }
    }
}
