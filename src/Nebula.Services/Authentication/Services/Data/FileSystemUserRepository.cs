using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nebula.Services.Base.Extensions;
using Nebula.Services.Base.Models;
using Nebula.Services.Fragments.Authentication;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Authentication.Services.Data
{
    public class FileSystemUserRepository : IUserRepository
    {
        private readonly DirectoryInfo _dataDir;
        private readonly ILogger<FileSystemUserRepository> _logger;
        private readonly ConcurrentDictionary<string, Guid> emailIndex = new();
        private readonly ConcurrentDictionary<string, Guid> userNameIndex = new();

        public FileSystemUserRepository(IOptions<AppSettings> settings, ILogger<FileSystemUserRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var root = new DirectoryInfo(settings.Value.DataStore);
            root.Create();
            _dataDir = root.CreateSubdirectory("auth").CreateSubdirectory("data");

            LoadIndex().Wait();
        }

        private async Task LoadIndex()
        {
            await foreach (var r in GetAll())
            {

                emailIndex.TryAdd(r.Private.Email, r.UserIdGuid);
                userNameIndex.TryAdd(r.Public.UserName, r.UserIdGuid);
            }
        }

        public async Task<bool> Create(UserFullRecord user)
        {
            Guid.TryParse(user.Public.UserId, out var id);
            var fd = GetDataFilePath(id);

            if (fd.Exists)
                return false;

            if (!userNameIndex.TryAdd(user.Public.UserName, id))
                return false;

            if (!emailIndex.TryAdd(user.Private.Email, id))
                return false;

            await File.WriteAllBytesAsync(fd.FullName, user.ToByteArray());
            return true;
        }

        public Task<bool> Delete(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EmailTaken(string email)
        {
            return Task.FromResult(emailIndex.TryGetValue(email, out var dummy));
        }

        public Task<bool> Exists(Guid userId)
        {
            var fd = GetDataFilePath(userId);
            return Task.FromResult(fd.Exists);
        }

        public async IAsyncEnumerable<UserRecord> GetAll()
        {
            foreach (var fd in GetAllDataFiles())
                yield return UserRecord.Parser.ParseFrom(await File.ReadAllBytesAsync(fd.FullName));
        }

        public IAsyncEnumerable<UserRecord> GetAllByOrganization(Guid organizationId)
        {
            //foreach (var fd in GetAllDataFiles())
            //{
            //    var record = UserRecord.Parser.ParseFrom(await File.ReadAllBytesAsync(fd.FullName));
            //    if (record.Public.OrganizationId.ToGuid() == organizationId.ToString())
            //        yield return record;
            //}
            throw new NotImplementedException();
        }

        public Task<Guid[]> GetAllUserIds()
        {
            return Task.FromResult(GetAllDataFiles().Select(f => Guid.Parse(f.Name)).ToArray());
        }

        public async Task<UserFullRecord> GetByEmail(string email)
        {
            if (emailIndex.TryGetValue(email, out var id))
                return await GetById(id);

            return null;
        }

        public async Task<UserFullRecord> GetById(Guid userId)
        {
            var fd = GetDataFilePath(userId);
            if (!fd.Exists)
                return null;

            return UserFullRecord.Parser.ParseFrom(await File.ReadAllBytesAsync(fd.FullName));
        }

        public async Task<UserFullRecord> GetByUserName(string userName)
        {
            if (userNameIndex.TryGetValue(userName, out var id))
                return await GetById(id);

            return null;
        }

        public Task<bool> UserNameTaken(string userName)
        {
            return Task.FromResult(userNameIndex.TryGetValue(userName, out var dummy));
        }

        public Task<bool> ChangeEmailIndex(string email, Guid id)
        {
            var toDel = emailIndex.Where(kv => kv.Value == id).Select(kv => kv.Key).ToArray();
            foreach (var e in toDel)
                emailIndex.TryRemove(e, out var dummy);

            emailIndex.TryAdd(email.ToLower(), id);

            return Task.FromResult(true);
        }

        public Task<bool> ChangeUserNameIndex(string userName, Guid id)
        {
            var toDel = userNameIndex.Where(kv => kv.Value == id).Select(kv => kv.Key).ToArray();
            foreach (var e in toDel)
                userNameIndex.TryRemove(e, out var dummy);

            userNameIndex.TryAdd(userName.ToLower(), id);

            return Task.FromResult(true);
        }

        public async Task Save(UserFullRecord Record)
        {
            Guid.TryParse(Record.Public.UserId, out var id);
            var fd = GetDataFilePath(id);
            await File.WriteAllBytesAsync(fd.FullName, Record.ToByteArray());
        }

        private IEnumerable<FileInfo> GetAllDataFiles()
        {
            return _dataDir.EnumerateFiles("*", SearchOption.AllDirectories);
        }

        private FileInfo GetDataFilePath(Guid userID)
        {
            return _dataDir.CreateGuidFileInfo(userID);
        }
    }
}
