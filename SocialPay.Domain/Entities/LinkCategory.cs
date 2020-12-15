using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class LinkCategory
    {
        public long LinkCategoryId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TransactionReference { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string Channel { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
