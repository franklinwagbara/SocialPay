using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class TenantProfile : BaseEntity
    {
        public long TenantProfileId { get; set; }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TenantName { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string Email { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string PhoneNumber { get; set; }
        [Column(TypeName = "NVARCHAR(350)")]
        public string Address { get; set; }
        [Column(TypeName = "NVARCHAR(190)")]
        public string WebSiteUrl { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string ClientId { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string ClientSecret { get; set; }
        [Column(TypeName = "NVARCHAR(150)")]
        public string AuthKey { get; set; }
        public bool Status { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
