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
        private readonly DbSet<EmployeeEntity> _employees;

        public PostgresEmployeeRepository(ILogger<PostgresEmployeeRepository> logger, OrganizationsDbContext dbContext)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _employees = dbContext.Employees ?? throw new ArgumentNullException(nameof(dbContext.Employees));
        }

        public async Task<bool> Create(EmployeeRecord employee)
        {
            try
            {
                var entity = employee.ToEntity();
                _employees.Add(entity);
                return true;
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
                return await _employees
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
            var employees = _employees
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
                var entity = await _employees
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

        public async Task<EmployeeRecord> GetByUserId(Guid organizationId, Guid userId)
        {
            try
            {
                var entity = await _employees
                    .Where(e => e.OrganizationId == organizationId && e.UserId == userId && e.IsActive)
                    .FirstOrDefaultAsync();
                return entity?.ToRecord();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get employee by User ID {UserId} in organization {OrganizationId}", userId, organizationId);
                return null;
            }
        }

        public async Task<bool> Update(EmployeeRecord employee)
        {

            var existingEntity = await _employees
                .Where(e => e.Id == employee.EmployeeId.ToGuid())
                .FirstOrDefaultAsync();

            if (existingEntity == null) return false;

            // Update fields from record
            existingEntity.FirstName = employee.FirstName ?? existingEntity.FirstName;
            existingEntity.LastName = employee.LastName ?? existingEntity.LastName;
            existingEntity.Email = employee.Email ?? existingEntity.Email;
            existingEntity.Phone = employee.Phone ?? existingEntity.Phone;
            existingEntity.JobTitle = employee.JobTitle ?? existingEntity.JobTitle;
            existingEntity.Department = employee.Department ?? existingEntity.Department;
            existingEntity.IsActive = employee.IsActive;
            existingEntity.LastModifiedUtc = DateTime.UtcNow;
            existingEntity.LastModifiedBy = employee.LastModifiedBy;

            if (!string.IsNullOrEmpty(employee.ManagerId))
                existingEntity.ManagerId = employee.ManagerId.ToGuid();

            return true;
        }
    }
}
