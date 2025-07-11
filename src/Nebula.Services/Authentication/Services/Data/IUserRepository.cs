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
        Task<UserFullRecord?> GetById(Guid userId);
        Task<UserFullRecord?> GetByEmail(string email);
        Task<UserFullRecord?> GetByUserName(string userName);
        Task<bool> Delete(Guid userId);
        Task<Guid[]> GetAllUserIds();
        IAsyncEnumerable<UserRecord> GetAll();
        IAsyncEnumerable<UserRecord> GetAllByOrganization(Guid organizationId);
    }
}
