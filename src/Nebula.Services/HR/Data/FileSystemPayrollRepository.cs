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
    public class FileSystemPayrollRepository : IPayrollRepository
    {
        private readonly ILogger<FileSystemPayrollRepository> _logger;

        public FileSystemPayrollRepository(ILogger<FileSystemPayrollRepository> logger, IOptions<AppSettings> settings)
        {
            _logger = logger;
        }

        public Task<bool> Create(PayrollRecord payroll)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(Guid organizationId, Guid payrollId)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<PayrollRecord> GetAll(Guid organizationId)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<PayrollRecord> GetByEmployee(Guid organizationId, Guid employeeId)
        {
            throw new NotImplementedException();
        }
    }
}
