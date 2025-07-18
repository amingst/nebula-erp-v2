using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Organizations.Data.Postgres
{
    [Table("organizations")]
    public class OrganizationEntity
    {
        [Key]
        public Guid OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid OwnerId { get; set; } // References User from auth schema
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public ICollection<EmployeeEntity> Employees { get; set; } = new List<EmployeeEntity>();
        
        // For protobuf compatibility - will be computed from navigation properties
        public List<string> EmployeeIds { get; set; } = new();
        public List<string> CustomerIds { get; set; } = new();
        
        public DateTime CreatedUtc { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime LastModifiedUtc { get; set; }
        public string LastModifiedBy { get; set; } = string.Empty;
        public DateTime DisabledUtc { get; set; }
        public string DisabledBy { get; set; } = string.Empty;
        public string DisabledReason { get; set; } = string.Empty;
    }
}
