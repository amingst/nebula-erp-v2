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
    public class FileSystemProductRepository : IProductRepository
    {
        private readonly ILogger<FileSystemProductRepository> _logger;
        private readonly DirectoryInfo _dataDir;

        public FileSystemProductRepository(ILogger<FileSystemProductRepository> logger, IOptions<AppSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var root = new DirectoryInfo(settings.Value.DataStore);
            root.Create();
            _dataDir = root.CreateSubdirectory("inventory").CreateSubdirectory("products").CreateSubdirectory("data");
        }

        public async Task<bool> Create(ProductRecord product)
        {
            if (string.IsNullOrWhiteSpace(product.OrganizationId))
                throw new ArgumentException("Product must have an OrganizationId.");

            if (string.IsNullOrWhiteSpace(product.ProductId))
                throw new ArgumentException("Product must have a ProductId.");

            if (!Guid.TryParse(product.ProductId, out var productGuid))
                throw new ArgumentException("ProductId must be a valid Guid.");

            try
            {
                var file = GetDataFilePath(product.OrganizationId, productGuid);

                if (file.Exists)
                {
                    _logger.LogWarning("Product with ID {ProductId} already exists in org {OrgId}.", product.ProductId, product.OrganizationId);
                    return false;
                }

                await File.WriteAllBytesAsync(file.FullName, product.ToByteArray());

                _logger.LogInformation("Created product {ProductId} under organization {OrgId}", product.ProductId, product.OrganizationId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product {ProductId}", product.ProductId);
                return false;
            }
        }

        public Task<bool> Delete(Guid productId)
        {
            var file = FindProductFileById(productId);
            if (file == null || !file.Exists)
                return Task.FromResult(false);

            file.Delete();
            return Task.FromResult(true);
        }

        public Task<bool> Exists(Guid productId)
        {
            var file = FindProductFileById(productId);
            return Task.FromResult(file != null && file.Exists);
        }

        public async IAsyncEnumerable<ProductRecord> GetAll(Guid organizationId)
        {
            var orgFolder = new DirectoryInfo(Path.Combine(_dataDir.FullName, $"org_{organizationId}"));
            if (!orgFolder.Exists)
                yield break;

            foreach (var file in orgFolder.EnumerateFiles("*"))
            {
                var bytes = await File.ReadAllBytesAsync(file.FullName);
                yield return ProductRecord.Parser.ParseFrom(bytes);
            }
        }

        public Task<Guid[]> GetAllProductIds(Guid organizationId)
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

        public async Task<ProductRecord?> GetById(Guid organizationId, Guid productId)
        {
            var file = GetDataFilePath(organizationId.ToString(), productId);
            if (!file.Exists)
                return null;

            var bytes = await File.ReadAllBytesAsync(file.FullName);
            return ProductRecord.Parser.ParseFrom(bytes);
        }


        // Utility: Construct path to product file based on organization and product ID
        private FileInfo GetDataFilePath(string organizationId, Guid productId)
        {
            var orgFolder = new DirectoryInfo(Path.Combine(_dataDir.FullName, $"org_{organizationId}"));
            orgFolder.Create(); // Safe even if it already exists
            return new FileInfo(Path.Combine(orgFolder.FullName, productId.ToString()));
        }

        // Utility: Search all org folders for a given product ID
        private FileInfo? FindProductFileById(Guid productId)
        {
            foreach (var orgDir in _dataDir.EnumerateDirectories("org_*"))
            {
                var candidate = new FileInfo(Path.Combine(orgDir.FullName, productId.ToString()));
                if (candidate.Exists)
                    return candidate;
            }
            return null;
        }

        // Optional: Get all files in the entire product directory
        private IEnumerable<FileInfo> GetAllDataFiles()
        {
            return _dataDir.EnumerateFiles("*", SearchOption.AllDirectories);
        }
    }
}
