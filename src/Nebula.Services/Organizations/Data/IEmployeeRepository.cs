using Nebula.Services.Fragments.Organizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Organizations.Data
{
    public interface IEmployeeRepository
    {
        Task<bool> Create(EmployeeRecord employee);
        Task<bool> Exists(Guid organizationId, Guid employeeId);
        Task<EmployeeRecord?> GetById(Guid organizationId, Guid employeeId);
        Task<EmployeeRecord?> GetByUserId(Guid organizationId, Guid userId);
        IAsyncEnumerable<EmployeeRecord?> GetAll(Guid organizationId);
        Task<bool> Update(EmployeeRecord employee);
    }
}
