using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class SpectaRegisterCustomerResponse : BaseEntity
    {
        public long SpectaRegisterCustomerResponseId { get; set; }
        public long SpectaRegisterCustomerRequestId { get; set; }
        public bool success { get; set; }
        public bool unAuthorizedRequest { get; set; }
        public bool __abp { get; set; }
        public int code { get; set; }
        [Column(TypeName = "NVARCHAR(290)")]
        public string message { get; set; }
        [Column(TypeName = "NVARCHAR(290)")]
        public string details { get; set; }
        [Column(TypeName = "NVARCHAR(290)")]
        public string validationErrors { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual SpectaRegisterCustomerRequest SpectaRegisterCustomerRequest { get; set; }
    }
}
