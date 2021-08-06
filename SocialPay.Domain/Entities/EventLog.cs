using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class EventLog : BaseEntity
    {
        public long EventLogId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string UserId { get; set; }
        [Column(TypeName = "NVARCHAR(40)")]
        public string IpAddress { get; set; }
        [Column(TypeName = "NVARCHAR(40)")]
        public string ModuleAccessed { get; set; }
        [Column(TypeName = "NVARCHAR(120)")]
        public string Description { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
