using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nebula.Services.Base.Models;
using Nebula.Services.Fragments.HR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.HR.Data
{
    public class FileSystemTimesheetRepository : ITimesheetRepository
    {
        private readonly ILogger<FileSystemTimesheetRepository> _logger;

        public FileSystemTimesheetRepository(ILogger<FileSystemTimesheetRepository> logger, IOptions<AppSettings> settings)
        {
            _logger = logger;
        }

        public Task<bool> Create(TimesheetRecord timesheet)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(Guid organizationId, Guid TimesheetId)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<TimesheetRecord> GetAll(Guid organizationId)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<TimesheetRecord> GetByEmployee(Guid organizationId, Guid employeeId)
        {
            throw new NotImplementedException();
        }
    }
}
