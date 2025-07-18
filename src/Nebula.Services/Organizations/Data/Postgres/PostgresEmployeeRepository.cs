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
    public class PostgresEmployeeRepository : IEmployeeRepository
    {
        private readonly ILogger<PostgresEmployeeRepository> _logger;
        private readonly OrganizationsDbContext _context;

        public PostgresEmployeeRepository(ILogger<PostgresEmployeeRepository> logger, OrganizationsDbContext dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<bool> Create(EmployeeRecord employee)
        {
            try
            {
                var entity = employee.ToEntity();
                _context.Employees.Add(entity);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create employee {EmployeeId}", employee.EmployeeId);
                return false;
            }
        }

        public async Task<bool> Exists(Guid organizationId, Guid employeeId)
        {
            try
            {
                return await _context.Employees
                    .Where(e => e.OrganizationId == organizationId && e.Id == employeeId && e.IsActive)
                    .AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check if employee exists {EmployeeId} in organization {OrganizationId}", employeeId, organizationId);
                return false;
            }
        }

        public async IAsyncEnumerable<EmployeeRecord> GetAll(Guid organizationId)
        {
            var employees = _context.Employees
                .Where(e => e.OrganizationId == organizationId && e.IsActive)
                .AsAsyncEnumerable();

            await foreach (var employee in employees)
            {
                yield return employee.ToRecord();
            }
        }

        public async Task<EmployeeRecord> GetById(Guid organizationId, Guid employeeId)
        {
            try
            {
                var entity = await _context.Employees
                    .Where(e => e.OrganizationId == organizationId && e.Id == employeeId && e.IsActive)
                    .FirstOrDefaultAsync();

                return entity?.ToRecord();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get employee by ID {EmployeeId} in organization {OrganizationId}", employeeId, organizationId);
                return null;
            }
        }

        public async Task<bool> Update(EmployeeRecord employee)
        {
            try
            {
                var existingEntity = await _context.Employees
                    .Where(e => e.Id == employee.EmployeeId.ToGuid())
                    .FirstOrDefaultAsync();

                if (existingEntity == null) return false;

                // Update fields from record
                existingEntity.FirstName = employee.FirstName ?? string.Empty;
                existingEntity.LastName = employee.LastName ?? string.Empty;
                existingEntity.Email = employee.Email ?? string.Empty;
                existingEntity.Phone = employee.Phone ?? string.Empty;
                existingEntity.JobTitle = employee.JobTitle ?? string.Empty;
                existingEntity.Department = employee.Department ?? string.Empty;
                existingEntity.IsActive = employee.IsActive;
                existingEntity.LastModifiedUtc = DateTime.UtcNow;
                existingEntity.LastModifiedBy = employee.LastModifiedBy ?? string.Empty;

                if (!string.IsNullOrEmpty(employee.ManagerId))
                    existingEntity.ManagerId = employee.ManagerId.ToGuid();

                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update employee {EmployeeId}", employee.EmployeeId);
                return false;
            }
        }
    }
}
