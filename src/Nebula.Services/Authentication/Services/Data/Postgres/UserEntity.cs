using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Authentication.Services.Data.Postgres
{
    [Table("users")]
    public class UserEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [MaxLength(100)]
        public string DisplayName { get; set; }

        [Required]
        public byte[] PasswordHash { get; set; }

        [Required]
        public byte[] Salt { get; set; }

        public DateTime CreatedUtc { get; set; }
        public DateTime LastModifiedUtc { get; set; }
        public DateTime LastLoginUtc { get; set; }
        public DateTime DisabledUtc { get; set; }

        public List<string> Roles { get; set; } = new();
        public List<string> Identities { get; set; } = new();

        // public ICollection<OrganizationRoleEntity> OrganizationRoles { get; set; }
    }
}
