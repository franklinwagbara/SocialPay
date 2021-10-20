using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class SpectaRegisterCustomerRequest : BaseEntity
    {
        public SpectaRegisterCustomerRequest()
        {
            SpectaRegisterCustomerResponse = new HashSet<SpectaRegisterCustomerResponse>();
        }
        public long SpectaRegisterCustomerRequestId { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string name { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string surname { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string userName { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string emailAddress { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string password { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string dob { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string title { get; set; }
        [Column(TypeName = "NVARCHAR(11)")]
        public string bvn { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string phoneNumber { get; set; }
        [Column(TypeName = "NVARCHAR(150)")]
        public string address { get; set; }
        [Column(TypeName = "NVARCHAR(40)")]
        public string stateOfResidence { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string captchaResponse { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string RegistrationStatus { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ICollection<SpectaRegisterCustomerResponse> SpectaRegisterCustomerResponse { get; set; }

    }
}
