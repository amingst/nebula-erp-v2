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
    public class FileSystemStockRepository : IStockRepository
    {
        private readonly ILogger<FileSystemStockRepository> _logger;
        private readonly DirectoryInfo _dataDir;

        public FileSystemStockRepository(ILogger<FileSystemStockRepository> logger, IOptions<AppSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var root = new DirectoryInfo(settings.Value.DataStore);
            root.Create();
            _dataDir = root.CreateSubdirectory("inventory").CreateSubdirectory("stock").CreateSubdirectory("data");
        }

        public async Task<bool> Create(Guid organizationId, StockRecord stock)
        {
            if (stock == null)
                throw new ArgumentNullException(nameof(stock));

            if (string.IsNullOrWhiteSpace(stock.StockId))
                throw new ArgumentException("StockRecord must have a valid StockId");

            if (!Guid.TryParse(stock.StockId, out var stockGuid))
                throw new ArgumentException("StockId must be a valid Guid.");

            try
            {
                var file = GetDataFilePath(organizationId, stockGuid);

                if (file.Exists)
                {
                    _logger.LogWarning("Stock {StockId} already exists for org {OrgId}", stock.StockId, organizationId);
                    return false;
                }

                await File.WriteAllBytesAsync(file.FullName, stock.ToByteArray());
                _logger.LogInformation("Created stock {StockId} for org {OrgId}", stock.StockId, organizationId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating stock {StockId} for org {OrgId}", stock.StockId, organizationId);
                return false;
            }
        }

        public Task<bool> Delete(Guid organizationId, Guid stockId)
        {
            var file = GetDataFilePath(organizationId, stockId);
            if (!file.Exists)
                return Task.FromResult(false);

            file.Delete();
            return Task.FromResult(true);
        }

        public Task<bool> Exists(Guid organizationId, Guid stockId)
        {
            var file = GetDataFilePath(organizationId, stockId);
            return Task.FromResult(file.Exists);
        }

        public async Task<StockRecord?> GetById(Guid organizationId, Guid stockId)
        {
            var file = GetDataFilePath(organizationId, stockId);
            if (!file.Exists)
                return null;

            var bytes = await File.ReadAllBytesAsync(file.FullName);
            return StockRecord.Parser.ParseFrom(bytes);
        }

        public async IAsyncEnumerable<StockRecord> GetAll(Guid organizationId)
        {
            var orgFolder = new DirectoryInfo(Path.Combine(_dataDir.FullName, $"org_{organizationId}"));
            if (!orgFolder.Exists)
                yield break;

            foreach (var file in orgFolder.EnumerateFiles("*"))
            {
                var bytes = await File.ReadAllBytesAsync(file.FullName);
                yield return StockRecord.Parser.ParseFrom(bytes);
            }
        }

        public Task<Guid[]> GetAllStockIds(Guid organizationId)
        {
            var orgFolder = new DirectoryInfo(Path.Combine(_dataDir.FullName, $"org_{organizationId}"));
            if (!orgFolder.Exists)
                return Task.FromResult(Array.Empty<Guid>());

            var ids = orgFolder
                .EnumerateFiles("*")
                .Select(f => Guid.TryParse(f.Name, out var guid) ? guid : (Guid?)null)
                .Where(g => g.HasValue)
                .Select(g => g.Value)
                .ToArray();

            return Task.FromResult(ids);
        }

        private FileInfo GetDataFilePath(Guid organizationId, Guid stockId)
        {
            var orgFolder = new DirectoryInfo(Path.Combine(_dataDir.FullName, $"org_{organizationId}"));
            orgFolder.Create(); // Safe even if exists
            return new FileInfo(Path.Combine(orgFolder.FullName, stockId.ToString()));
        }
    }
}
