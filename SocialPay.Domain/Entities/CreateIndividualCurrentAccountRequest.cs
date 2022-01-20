using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class CreateIndividualCurrentAccountRequest : BaseEntity
    {
        public long CreateIndividualCurrentAccountRequestId { get; set; }
        public string BranchCode { get; set; }
        public string TaxId { get; set; }
        public string CountryOfBirth { get; set; }
        public string OtherNationality { get; set; }
        public string IdentityCard { get; set; }
        public string UtilityBill { get; set; }
        public string Signature { get; set; }
        public string Passport { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
