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
    public class FileSystemSupplierRepository : ISupplierRepository
    {
        private readonly ILogger<FileSystemSupplierRepository> _logger;
        private readonly DirectoryInfo _dataDir;

        public FileSystemSupplierRepository(ILogger<FileSystemSupplierRepository> logger, IOptions<AppSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var root = new DirectoryInfo(settings.Value.DataStore);
            root.Create();
            _dataDir = root.CreateSubdirectory("inventory").CreateSubdirectory("suppliers").CreateSubdirectory("data");
        }

        public async Task<bool> Create(Guid organizationId, SupplierRecord supplier)
        {
            if (supplier == null)
                throw new ArgumentNullException(nameof(supplier));

            if (string.IsNullOrWhiteSpace(supplier.SupplierId))
                throw new ArgumentException("Supplier must have a valid SupplierId");

            if (!Guid.TryParse(supplier.SupplierId, out var supplierGuid))
                throw new ArgumentException("SupplierId must be a valid Guid.");

            try
            {
                var file = GetDataFilePath(organizationId, supplierGuid);

                if (file.Exists)
                {
                    _logger.LogWarning("Supplier {SupplierId} already exists for org {OrgId}", supplier.SupplierId, organizationId);
                    return false;
                }

                await File.WriteAllBytesAsync(file.FullName, supplier.ToByteArray());
                _logger.LogInformation("Created supplier {SupplierId} for org {OrgId}", supplier.SupplierId, organizationId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating supplier {SupplierId} for org {OrgId}", supplier.SupplierId, organizationId);
                return false;
            }
        }

        public Task<bool> Delete(Guid organizationId, Guid supplierId)
        {
            var file = GetDataFilePath(organizationId, supplierId);
            if (!file.Exists)
                return Task.FromResult(false);

            file.Delete();
            return Task.FromResult(true);
        }

        public Task<bool> Exists(Guid organizationId, Guid supplierId)
        {
            var file = GetDataFilePath(organizationId, supplierId);
            return Task.FromResult(file.Exists);
        }

        public async Task<SupplierRecord?> GetById(Guid organizationId, Guid supplierId)
        {
            var file = GetDataFilePath(organizationId, supplierId);
            if (!file.Exists)
                return null;

            var bytes = await File.ReadAllBytesAsync(file.FullName);
            return SupplierRecord.Parser.ParseFrom(bytes);
        }

        public async IAsyncEnumerable<SupplierRecord> GetAll(Guid organizationId)
        {
            var orgFolder = new DirectoryInfo(Path.Combine(_dataDir.FullName, $"org_{organizationId}"));
            if (!orgFolder.Exists)
                yield break;

            foreach (var file in orgFolder.EnumerateFiles("*"))
            {
                var bytes = await File.ReadAllBytesAsync(file.FullName);
                yield return SupplierRecord.Parser.ParseFrom(bytes);
            }
        }

        public Task<Guid[]> GetAllSupplierIds(Guid organizationId)
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

        private FileInfo GetDataFilePath(Guid organizationId, Guid supplierId)
        {
            var orgFolder = new DirectoryInfo(Path.Combine(_dataDir.FullName, $"org_{organizationId}"));
            orgFolder.Create(); // Safe even if it already exists
            return new FileInfo(Path.Combine(orgFolder.FullName, supplierId.ToString()));
        }
    }
}
