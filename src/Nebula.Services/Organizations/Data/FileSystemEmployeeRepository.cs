using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nebula.Services.Base.Extensions;
using Nebula.Services.Base.Models;
using Nebula.Services.Fragments.Organizations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Organizations.Data
{
    public class FileSystemEmployeeRepository : IEmployeeRepository
    {
        private readonly ILogger<FileSystemEmployeeRepository> _logger;
        private readonly DirectoryInfo _dataDir;

        public FileSystemEmployeeRepository(ILogger<FileSystemEmployeeRepository> logger, IOptions<AppSettings> settings)
        {
            _logger = logger;

            var root = new DirectoryInfo(settings.Value.DataStore);
            root.Create();
            _dataDir = root.CreateSubdirectory("organizations").CreateSubdirectory("employees").CreateSubdirectory("data");
        }

        public async Task<bool> Create(EmployeeRecord employee)
        {
            var organizationId = employee.OrganizationId.ToGuid();
            var orgDir = _dataDir.CreateSubdirectory(organizationId.ToString());
            var employeeFile = orgDir.CreateGuidFileInfo(employee.EmployeeId.ToGuid());

            if (employeeFile.Exists)
                return false;

            await File.WriteAllBytesAsync(employeeFile.FullName, employee.ToByteArray());
            return true;
        }

        public Task<bool> Exists(Guid organizationId, Guid employeeId)
        {
            var orgDir = _dataDir.CreateSubdirectory(organizationId.ToString());
            var employeeFile = orgDir.CreateGuidFileInfo(employeeId);
            return Task.FromResult(employeeFile.Exists);
        }

        public async IAsyncEnumerable<EmployeeRecord> GetAll(Guid organizationId)
        {
            var orgDir = _dataDir.CreateSubdirectory(organizationId.ToString());
            foreach (var fd in GetAllDataFiles(orgDir))
            {
                yield return EmployeeRecord.Parser.ParseFrom(await File.ReadAllBytesAsync(fd.FullName));
            }
        }

        public async Task<EmployeeRecord> GetById(Guid organizationId, Guid employeeId)
        {
            var orgDir = _dataDir.CreateSubdirectory(organizationId.ToString());
            var employeeFile = orgDir.CreateGuidFileInfo(employeeId);

            if (!employeeFile.Exists)
                return null;

            return EmployeeRecord.Parser.ParseFrom(await File.ReadAllBytesAsync(employeeFile.FullName));
        }

        public async Task<bool> Update(EmployeeRecord employee)
        {
            var organizationId = employee.OrganizationId.ToGuid();
            var orgDir = _dataDir.CreateSubdirectory(organizationId.ToString());
            var employeeFile = orgDir.CreateGuidFileInfo(employee.EmployeeId.ToGuid());

            if (!employeeFile.Exists)
                return false;

            await File.WriteAllBytesAsync(employeeFile.FullName, employee.ToByteArray());
            return true;
        }

        public async Task<bool> Delete(Guid organizationId, Guid employeeId)
        {
            var orgDir = _dataDir.CreateSubdirectory(organizationId.ToString());
            var employeeFile = orgDir.CreateGuidFileInfo(employeeId);

            if (!employeeFile.Exists)
                return false;

            employeeFile.Delete();
            return true;
        }

        private IEnumerable<FileInfo> GetAllDataFiles(DirectoryInfo orgDir)
        {
            return orgDir.EnumerateFiles("*", SearchOption.AllDirectories);
        }
    }
}
