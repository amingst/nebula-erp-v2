using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Nebula.Services.Organizations.Data.Postgres
{
    public class OrganizationsDbContextFactory : IDesignTimeDbContextFactory<OrganizationsDbContext>
    {
        public OrganizationsDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrganizationsDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=nebula_erp;Username=org_service;Password=org_service_password;Search Path=organizations;Include Error Detail=true");
            return new OrganizationsDbContext(optionsBuilder.Options);
        }
    }
}
