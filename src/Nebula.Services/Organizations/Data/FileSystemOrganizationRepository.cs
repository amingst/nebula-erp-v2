using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nebula.Services.Base.Extensions;
using Nebula.Services.Base.Models;
using Nebula.Services.Fragments.Authentication;
using Nebula.Services.Fragments.Organizations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Organizations.Data
{
    public class FileSystemOrganizationRepository : IOrganizationRepository
    {
        private readonly DirectoryInfo _dataDir;
        private readonly ILogger<FileSystemOrganizationRepository> _logger;

        public FileSystemOrganizationRepository(IOptions<AppSettings> settings, ILogger<FileSystemOrganizationRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var root = new DirectoryInfo(settings.Value.DataStore);
            root.Create();
            _dataDir = root.CreateSubdirectory("organizations").CreateSubdirectory("data");
        }
        public async Task<bool> Create(OrganizationRecord organization)
        {
            var id = organization.OrganizationId.ToGuid();
            var fd = GetDataFilePath(id);

            if (fd.Exists)
                return false;

            await File.WriteAllBytesAsync(fd.FullName, organization.ToByteArray());
            return true;
        }

        public Task<bool> Delete(Guid organizationId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Exists(Guid organizationId)
        {
            var fd = GetDataFilePath(organizationId);
            return Task.FromResult(fd.Exists);
        }

        public async IAsyncEnumerable<OrganizationRecord> GetAll()
        {
            foreach (var fd in GetAllDataFiles())
            {
                yield return OrganizationRecord.Parser.ParseFrom(await File.ReadAllBytesAsync(fd.FullName));
            }
        }

        public Task<Guid[]> GetAllOrganizationIds()
        {
            throw new NotImplementedException();
        }

        public async Task<OrganizationRecord> GetById(Guid organizationId)
        {
            var fd = GetDataFilePath(organizationId);
            if (!fd.Exists)
                return null;

            return OrganizationRecord.Parser.ParseFrom(await File.ReadAllBytesAsync(fd.FullName));
        }

        private IEnumerable<FileInfo> GetAllDataFiles()
        {
            return _dataDir.EnumerateFiles("*", SearchOption.AllDirectories);
        }

        private FileInfo GetDataFilePath(Guid organizationId)
        {
            return _dataDir.CreateGuidFileInfo(organizationId);
        }
    }
}
