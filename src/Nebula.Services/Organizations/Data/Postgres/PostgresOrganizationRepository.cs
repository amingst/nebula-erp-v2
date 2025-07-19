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
    public class PostgresOrganizationRepository : IOrganizationRepository
    {
        private readonly ILogger<PostgresOrganizationRepository> _logger;
        private readonly DbSet<OrganizationEntity> _organizations;

        public PostgresOrganizationRepository(ILogger<PostgresOrganizationRepository> logger, OrganizationsDbContext dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _organizations = dbContext.Organizations ?? throw new ArgumentNullException(nameof(dbContext.Organizations));
        }

        public async Task<bool> Create(OrganizationRecord organization)
        {
            try
            {
                var entity = organization.ToEntity();
                _organizations.Add(entity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create organization {OrganizationId}", organization.OrganizationId);
                return false;
            }
        }

        public async Task<bool> Delete(Guid organizationId)
        {
            try
            {
                var entity = await _organizations.FindAsync(organizationId);
                if (entity == null) return false;

                // Soft delete
                entity.IsActive = false;
                entity.DisabledUtc = DateTime.UtcNow;
                _organizations.Update(entity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete organization {OrganizationId}", organizationId);
                return false;
            }
        }

        public async Task<bool> Exists(Guid organizationId)
        {
            try
            {
                return await _organizations
                    .Where(o => o.OrganizationId == organizationId && o.IsActive)
                    .AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check if organization exists {OrganizationId}", organizationId);
                return false;
            }
        }

        public async IAsyncEnumerable<OrganizationRecord> GetAll()
        {
            var organizations = _organizations
                .Where(o => o.IsActive)
                .AsAsyncEnumerable();

            await foreach (var org in organizations)
            {
                yield return org.ToRecord();
            }
        }

        public async Task<Guid[]> GetAllOrganizationIds()
        {
            try
            {
                return await _organizations
                    .Where(o => o.IsActive)
                    .Select(o => o.OrganizationId)
                    .ToArrayAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all organization IDs");
                return Array.Empty<Guid>();
            }
        }

        public async Task<OrganizationRecord> GetById(Guid organizationId)
        {
            try
            {
                var entity = await _organizations
                    .Where(o => o.OrganizationId == organizationId && o.IsActive)
                    .FirstOrDefaultAsync();

                return entity?.ToRecord();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get organization by ID {OrganizationId}", organizationId);
                return null;
            }
        }

        public async Task<bool> Update(OrganizationRecord organization)
        {
            try
            {
                var entity = organization.ToEntity();

                if (entity == null)
                    return false;

                 _organizations.Update(entity);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update organization {OrganizationId}", organization.OrganizationId);
                return false;
            }
        }
    }
}
