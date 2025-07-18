using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebula.Services.Organizations.Data.Postgres
{
    [Table("invites")]
    public class OrganizationInviteEntity
    {
        [Key]
        public Guid InviteId { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid OrganizationId { get; set; }
        public DateTime InvitedUtc { get; set; }
        public string InvitedBy { get; set; } = string.Empty;
        public DateTime ValidUntilUtc { get; set; }
        public DateTime UsedUtc { get; set; }
    }
}
