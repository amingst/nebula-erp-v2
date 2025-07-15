using Nebula.Services.Fragments.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.HR.Data
{
    public interface ITimesheetRepository
    {
        Task<bool> Create(TimesheetRecord timesheet);
        Task<bool> Exists(Guid organizationId, Guid TimesheetId);
        IAsyncEnumerable<TimesheetRecord> GetByEmployee(Guid organizationId, Guid employeeId);
        IAsyncEnumerable<TimesheetRecord> GetAll(Guid organizationId);
    }
}
