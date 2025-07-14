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
    public class FileSystemStockMovementRepository : IStockMovementRepository
    {
        private readonly ILogger<FileSystemStockMovementRepository> _logger;
        private readonly DirectoryInfo _dataDir;

        public FileSystemStockMovementRepository(ILogger<FileSystemStockMovementRepository> logger, IOptions<AppSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            var root = new DirectoryInfo(settings.Value.DataStore);
            root.Create();
            _dataDir = root.CreateSubdirectory("inventory").CreateSubdirectory("stock_movements").CreateSubdirectory("data");
        }

        public async Task<bool> Create(StockMovementRecord movement)
        {
            if (!Guid.TryParse(movement.MovementId, out var id) || string.IsNullOrWhiteSpace(movement.OrganizationId))
                return false;

            var file = GetDataFile(movement.OrganizationId, id);
            if (file.Exists) return false;

            await File.WriteAllBytesAsync(file.FullName, movement.ToByteArray());
            return true;
        }

        public Task<bool> Delete(Guid movementId)
        {
            var file = FindDataFileById(movementId);
            if (file?.Exists == true)
            {
                file.Delete();
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> Exists(Guid movementId)
        {
            var file = FindDataFileById(movementId);
            return Task.FromResult(file?.Exists == true);
        }

        public async IAsyncEnumerable<StockMovementRecord> GetAll()
        {
            foreach (var file in GetAllDataFiles())
                yield return StockMovementRecord.Parser.ParseFrom(await File.ReadAllBytesAsync(file.FullName));
        }

        public Task<Guid[]> GetAllStockMovementIds()
        {
            var ids = GetAllDataFiles().Select(f => Guid.TryParse(f.Name, out var g) ? g : Guid.Empty)
                                       .Where(g => g != Guid.Empty).ToArray();
            return Task.FromResult(ids);
        }

        public async Task<StockMovementRecord?> GetById(Guid movementId)
        {
            var file = FindDataFileById(movementId);
            if (file?.Exists != true) return null;
            return StockMovementRecord.Parser.ParseFrom(await File.ReadAllBytesAsync(file.FullName));
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
