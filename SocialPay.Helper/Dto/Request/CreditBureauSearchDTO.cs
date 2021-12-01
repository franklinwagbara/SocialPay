using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class CreditBureauSearchDTO
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string bvn { get; set; }
        public string dateOfBirth { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string phoneNumber { get; set; }
        public int gender { get; set; }
    }

}
