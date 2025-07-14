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
    public class FileSystemLocationRepository : ILocationRepository
    {
        private readonly ILogger<FileSystemLocationRepository> _logger;
        private readonly DirectoryInfo _dataDir;

        public FileSystemLocationRepository(ILogger<FileSystemLocationRepository> logger, IOptions<AppSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var root = new DirectoryInfo(settings.Value.DataStore);
            root.Create();
            _dataDir = root.CreateSubdirectory("inventory").CreateSubdirectory("locations").CreateSubdirectory("data");
        }

        public async Task<bool> Create(Guid organizationId, LocationRecord location)
        {
            if (location == null)
                throw new ArgumentNullException(nameof(location));

            if (string.IsNullOrWhiteSpace(location.LocationId))
                throw new ArgumentException("LocationId is required.");

            if (!Guid.TryParse(location.LocationId, out var locationGuid))
                throw new ArgumentException("LocationId must be a valid Guid.");

            try
            {
                var file = GetDataFilePath(organizationId, locationGuid);

                if (file.Exists)
                {
                    _logger.LogWarning("Location {LocationId} already exists under org {OrgId}", location.LocationId, organizationId);
                    return false;
                }

                await File.WriteAllBytesAsync(file.FullName, location.ToByteArray());
                _logger.LogInformation("Created location {LocationId} under org {OrgId}", location.LocationId, organizationId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating location {LocationId} under org {OrgId}", location.LocationId, organizationId);
                return false;
            }
        }

        public Task<bool> Delete(Guid organizationId, Guid locationId)
        {
            var file = GetDataFilePath(organizationId, locationId);
            if (!file.Exists)
                return Task.FromResult(false);

            file.Delete();
            return Task.FromResult(true);
        }

        public Task<bool> Exists(Guid organizationId, Guid locationId)
        {
            var file = GetDataFilePath(organizationId, locationId);
            return Task.FromResult(file.Exists);
        }

        public async IAsyncEnumerable<LocationRecord> GetAll(Guid organizationId)
        {
            var orgFolder = new DirectoryInfo(Path.Combine(_dataDir.FullName, $"org_{organizationId}"));
            if (!orgFolder.Exists)
                yield break;

            foreach (var file in orgFolder.EnumerateFiles("*"))
            {
                var bytes = await File.ReadAllBytesAsync(file.FullName);
                yield return LocationRecord.Parser.ParseFrom(bytes);
            }
        }

        public Task<Guid[]> GetAllLocationIds(Guid organizationId)
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

        public async Task<LocationRecord> GetById(Guid organizationId, Guid locationId)
        {
            var file = GetDataFilePath(organizationId, locationId);
            if (!file.Exists)
                return null;

            var bytes = await File.ReadAllBytesAsync(file.FullName);
            return LocationRecord.Parser.ParseFrom(bytes);
        }

        private FileInfo GetDataFilePath(Guid organizationId, Guid locationId)
        {
            var orgFolder = new DirectoryInfo(Path.Combine(_dataDir.FullName, $"org_{organizationId}"));
            orgFolder.Create(); // Safe even if it exists
            return new FileInfo(Path.Combine(orgFolder.FullName, locationId.ToString()));
        }
    }
}
