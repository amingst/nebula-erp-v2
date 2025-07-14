using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nebula.Services.Base.Models;
using Nebula.Services.Fragments.Inventory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nebula.Services.Inventory.Data
{
    public class FileSystemBatchRepository : IBatchRepository
    {
        private readonly ILogger<FileSystemBatchRepository> _logger;
        private readonly DirectoryInfo _dataDir;

        public FileSystemBatchRepository(ILogger<FileSystemBatchRepository> logger, IOptions<AppSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var root = new DirectoryInfo(settings.Value.DataStore);
            root.Create();
            _dataDir = root.CreateSubdirectory("inventory").CreateSubdirectory("batches").CreateSubdirectory("data");
        }

        public async Task<bool> Create(BatchRecord batch)
        {
            if (!Guid.TryParse(batch.BatchId, out var id) || string.IsNullOrWhiteSpace(batch.OrganizationId))
                return false;

            var file = GetDataFile(batch.OrganizationId, id);
            if (file.Exists) return false;

            await File.WriteAllBytesAsync(file.FullName, batch.ToByteArray());
            return true;
        }

        public Task<bool> Delete(Guid batchId)
        {
            var file = FindDataFileById(batchId);
            if (file?.Exists == true)
            {
                file.Delete();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> Exists(Guid batchId)
        {
            var file = FindDataFileById(batchId);
            return Task.FromResult(file?.Exists == true);
        }

        public async IAsyncEnumerable<BatchRecord> GetAll()
        {
            foreach (var file in GetAllDataFiles())
                yield return BatchRecord.Parser.ParseFrom(await File.ReadAllBytesAsync(file.FullName));
        }

        public Task<Guid[]> GetAllBatchIds()
        {
            var ids = GetAllDataFiles().Select(f => Guid.TryParse(f.Name, out var g) ? g : Guid.Empty)
                                       .Where(g => g != Guid.Empty).ToArray();
            return Task.FromResult(ids);
        }

        public async Task<BatchRecord?> GetById(Guid batchId)
        {
            var file = FindDataFileById(batchId);
            if (file?.Exists != true) return null;
            return BatchRecord.Parser.ParseFrom(await File.ReadAllBytesAsync(file.FullName));
        }

        private FileInfo GetDataFile(string orgId, Guid id)
        {
            var dir = new DirectoryInfo(Path.Combine(_dataDir.FullName, $"org_{orgId}"));
            dir.Create();
            return new FileInfo(Path.Combine(dir.FullName, id.ToString()));
        }

        private FileInfo? FindDataFileById(Guid id)
        {
            return _dataDir.EnumerateDirectories("org_*")
                           .Select(d => new FileInfo(Path.Combine(d.FullName, id.ToString())))
                           .FirstOrDefault(f => f.Exists);
        }

        private IEnumerable<FileInfo> GetAllDataFiles()
        {
            return _dataDir.EnumerateFiles("*", SearchOption.AllDirectories);
        }
    }
}
