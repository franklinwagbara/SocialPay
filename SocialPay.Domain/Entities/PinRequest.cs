using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace SocialPay.Domain.Entities
{
    public class PinRequest
    {
        public long PinRequestId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string Pin { get; set; }
        [Column(TypeName = "NVARCHAR(350)")]
        public string TokenSecret { get; set; }
        public bool Status { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
