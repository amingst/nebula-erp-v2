using Nebula.Services.Fragments.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.HR.Data
{
    public interface IPayrollRepository
    {
        Task<bool> Create(PayrollRecord payroll);
        Task<bool> Exists(Guid organizationId, Guid payrollId);
        IAsyncEnumerable<PayrollRecord> GetByEmployee(Guid organizationId, Guid employeeId);
        IAsyncEnumerable<PayrollRecord> GetAll(Guid organizationId);
    }
}
