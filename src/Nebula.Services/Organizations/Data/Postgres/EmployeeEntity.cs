using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Nebula.Services.Organizations.Data.Postgres
{
    [Table("employees")]
    public class EmployeeEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        [ForeignKey("Organization")]
        public Guid OrganizationId { get; set; }
        public OrganizationEntity Organization { get; set; } = null!;
        
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public bool IsActive { get; set; }
        
        [ForeignKey("Manager")]
        public Guid? ManagerId { get; set; }
        public EmployeeEntity? Manager { get; set; }
        public ICollection<EmployeeEntity> DirectReports { get; set; } = new List<EmployeeEntity>();
        
        public Guid UserId { get; set; } // References User from auth schema
        
        public DateTime CreatedUtc { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime LastModifiedUtc { get; set; }
        public string LastModifiedBy { get; set; } = string.Empty;
        public DateTime DisabledUtc { get; set; }
        public string DisabledBy { get; set; } = string.Empty;
        public string DisabledReason { get; set; } = string.Empty;
    }
}
