using Nebula.Services.Fragments.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.HR.Data
{
    public interface IEmployeeRepository
    {
        Task<bool> Create(EmployeeRecord employee);
        Task<bool> Exists(Guid organizationId, Guid employeeId);
        Task<EmployeeRecord?> GetById(Guid organizationId, Guid employeeId);
        IAsyncEnumerable<Guid> GetAllIds(Guid organizationId);
        IAsyncEnumerable<EmployeeRecord?> GetAll(Guid organizationId);
        Task<bool> Update(EmployeeRecord employee);
    }
}
