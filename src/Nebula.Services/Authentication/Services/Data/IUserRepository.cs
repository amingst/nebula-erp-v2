using Nebula.Services.Fragments.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Authentication.Services.Data
{
    public interface IUserRepository
    {
        Task<bool> Create(UserFullRecord user);
        Task<bool> Exists(Guid userId);
        Task<bool> EmailTaken(string email);
        Task<bool> UserNameTaken(string userName);
        Task<UserRecord?> GetById(Guid userId);
        Task<UserRecord?> GetByEmail(string email);
        Task<UserRecord?> GetByUserName(string userName);
        Task<bool> Delete(Guid userId);
        Task<Guid[]> GetAllUserIds();
        IAsyncEnumerable<UserRecord> GetAll();
        IAsyncEnumerable<UserRecord> GetAllByOrganization(Guid organizationId);
    }
}
