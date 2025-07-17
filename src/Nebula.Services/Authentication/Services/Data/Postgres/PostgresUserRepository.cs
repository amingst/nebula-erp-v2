using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Nebula.Services.Base.Extensions;
using Nebula.Services.Fragments.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Authentication.Services.Data.Postgres
{
    public class PostgresUserRepository : IUserRepository
    {
        private readonly AuthDbContext _context;
        private readonly DbSet<UserEntity> _users;
        private readonly ILogger<PostgresUserRepository> _logger;

        public PostgresUserRepository(AuthDbContext dbContext, ILogger<PostgresUserRepository> logger)
        {
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _users = dbContext.Users;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Create(UserFullRecord user)
        {
            var userEntity = user.ToEntity();
            try
            {
                await _users.AddAsync(userEntity);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user {UserName}", user.Public.UserName);
                return false;
            }
        }

        public Task<bool> Delete(Guid userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> EmailTaken(string email)
        {
            var exists = await _users.AnyAsync(u => u.Email == email);
            if (exists)
            {
                _logger.LogWarning("Email {Email} is already taken", email);
            }
            return exists;
        }

        public async Task<bool> Exists(Guid userId)
        {
            var found = await _users.FindAsync(userId); 
            return found != null;
        }

        public IAsyncEnumerable<UserRecord> GetAll()
        {
            return _users
                .Select(u => u.ToUserRecord())
                .AsAsyncEnumerable();
        }

        public IAsyncEnumerable<UserRecord> GetAllByOrganization(Guid organizationId)
        {
            // TODO: REMOVE FROM INTERFACE
            throw new NotImplementedException();
        }

        public async Task<Guid[]> GetAllUserIds()
        {
            return await _users
                .Select(u => u.Id)
                .ToArrayAsync();
        }

        public async Task<UserFullRecord> GetByEmail(string email)
        {
            return await _users
                .Where(u => u.Email == email)
                .Select(u => u.ToFullRecord())
                .FirstOrDefaultAsync();
        }

        public async Task<UserFullRecord> GetById(Guid userId)
        {
            return await _users
                .Where(u => u.Id == userId)
                .Select(u => u.ToFullRecord())
                .FirstOrDefaultAsync();
        }

        public async Task<UserFullRecord> GetByUserName(string userName)
        {
            return await _users
                .Where(u => u.UserName == userName)
                .Select(u => u.ToFullRecord())
                .FirstOrDefaultAsync();
        }

        public Task<bool> UserNameTaken(string userName)
        {
            var exists = _users.AnyAsync(u => u.UserName == userName);
            if (exists.Result)
            {
                _logger.LogWarning("Username {UserName} is already taken", userName);
            }
            return exists;
        }
    }
}
