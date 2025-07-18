using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Organizations.Data.Postgres
{
    public class OrganizationsDbContext : DbContext
    {
        public OrganizationsDbContext(DbContextOptions<OrganizationsDbContext> options) : base(options) { }
        
        // DbSets for your entities
        public DbSet<OrganizationEntity> Organizations { get; set; }
        public DbSet<EmployeeEntity> Employees { get; set; }
        public DbSet<OrganizationInviteEntity> OrganizationInvites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Organization entity
            modelBuilder.Entity<OrganizationEntity>(entity =>
            {
                entity.HasKey(e => e.OrganizationId);
                entity.Property(e => e.OrganizationName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.CreatedBy).HasMaxLength(255);
                entity.Property(e => e.LastModifiedBy).HasMaxLength(255);
                entity.Property(e => e.DisabledBy).HasMaxLength(255);
                entity.Property(e => e.DisabledReason).HasMaxLength(500);
                
                // Configure JSON columns for string arrays (for protobuf compatibility)
                entity.Property(e => e.EmployeeIds)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                        
                entity.Property(e => e.CustomerIds)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                
                // Configure navigation properties
                entity.HasMany(o => o.Employees)
                    .WithOne(e => e.Organization)
                    .HasForeignKey(e => e.OrganizationId)
                    .OnDelete(DeleteBehavior.Cascade);
                    
                // Configure indexes for organizations
                entity.HasIndex(o => o.OwnerId);
                entity.HasIndex(o => o.OrganizationName);
                entity.HasIndex(o => o.IsActive);
                entity.HasIndex(o => new { o.OwnerId, o.IsActive }); // Composite index for owner's active orgs
            });

            // Configure Employee entity
            modelBuilder.Entity<EmployeeEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.JobTitle).HasMaxLength(100);
                entity.Property(e => e.Department).HasMaxLength(100);
                entity.Property(e => e.CreatedBy).HasMaxLength(255);
                entity.Property(e => e.LastModifiedBy).HasMaxLength(255);
                entity.Property(e => e.DisabledBy).HasMaxLength(255);
                entity.Property(e => e.DisabledReason).HasMaxLength(500);
                
                // Configure self-referencing Manager relationship
                entity.HasOne(e => e.Manager)
                    .WithMany(e => e.DirectReports)
                    .HasForeignKey(e => e.ManagerId)
                    .OnDelete(DeleteBehavior.SetNull);
                    
                // Configure Organization relationship (already configured above, but adding index)
                entity.HasIndex(e => e.OrganizationId);
                entity.HasIndex(e => e.ManagerId);
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<OrganizationInviteEntity>(entity =>
            {
                entity.HasKey(e => e.InviteId);
                entity.Property(e => e.UserId).IsRequired();
                entity.Property(e => e.OrganizationId).IsRequired();
                entity.Property(e => e.InvitedBy).IsRequired().HasMaxLength(255);
                entity.Property(e => e.ValidUntilUtc).IsRequired();
                entity.Property(e => e.UsedUtc).IsRequired();
                // Configure indexes for invites
                entity.HasIndex(i => i.UserId);
                entity.HasIndex(i => i.OrganizationId);
            });
        }
    }
}
