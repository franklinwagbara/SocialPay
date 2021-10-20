using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class CreateIndividualCurrentAccountRequestDto
    {
        public string BranchCode { get; set; }
        public string TaxId { get; set; }
        public string CountryOfBirth { get; set; }
        public string OtherNationality { get; set; }
        public IFormFile IdentityCard { get; set; }
        public IFormFile UtilityBill { get; set; }
        public IFormFile Signature { get; set; }
        public IFormFile Passport { get; set; }
    }

}
