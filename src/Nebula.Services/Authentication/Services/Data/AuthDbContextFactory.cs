using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Nebula.Services.Authentication.Services.Data.Postgres;

namespace Nebula.Services.Authentication.Services.Data
{
    public class AuthDbContextFactory : IDesignTimeDbContextFactory<AuthDbContext>
    {
        public AuthDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AuthDbContext>();
            
            // Use the connection string for design-time migrations
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=nebula_erp;Username=auth_service;Password=auth_service_password;Search Path=auth;Include Error Detail=true");
            
            return new AuthDbContext(optionsBuilder.Options);
        }
    }
}
