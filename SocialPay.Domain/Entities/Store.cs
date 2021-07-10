using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class Store : BaseEntity
    {
        public long StoreId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string StoreName { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string Image { get; set; }
        [Column(TypeName = "NVARCHAR(190)")]
        public string FileLocation { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public ClientAuthentication ClientAuthentication { get; set; }
    }
}
