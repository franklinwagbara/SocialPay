using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class WebHookRequest : BaseEntity
    {
        public long WebHookRequestId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "NVARCHAR(150)")]
        public string description { get; set; }
        [Column(TypeName = "NVARCHAR(250)")]
        public string webHookUri { get; set; }
        public bool isActive { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string filters { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string headers { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
