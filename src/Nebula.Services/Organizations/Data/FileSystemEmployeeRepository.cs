using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nebula.Services.Base.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nebula.Services.Fragments.Organizations;

namespace Nebula.Services.Organizations.Data
{
    public class FileSystemEmployeeRepository : IEmployeeRepository
    {
        private readonly ILogger<FileSystemEmployeeRepository> _logger;

        public FileSystemEmployeeRepository(ILogger<FileSystemEmployeeRepository> logger, IOptions<AppSettings> settings)
        {
            _logger = logger;
        }

        public Task<bool> Create(EmployeeRecord employee)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(Guid organizationId, Guid employeeId)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<EmployeeRecord> GetAll(Guid organizationId)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<Guid> GetAllIds(Guid organizationId)
        {
            throw new NotImplementedException();
        }

        public Task<EmployeeRecord> GetById(Guid organizationId, Guid employeeId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(EmployeeRecord employee)
        {
            throw new NotImplementedException();
        }
    }
}
