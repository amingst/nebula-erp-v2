using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Organizations.Data.Postgres
{
    public class OrganizationsDbContextHelper
    {
        private readonly OrganizationsDbContext _dbContext;

        public OrganizationsDbContextHelper(OrganizationsDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            // Let EF Core manage the connection state automatically
            return await _dbContext.Database.BeginTransactionAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            // Let EF Core manage the connection - don't manually close it
            return await _dbContext.SaveChangesAsync();
        }
    }
}
