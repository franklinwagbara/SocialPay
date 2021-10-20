using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class RegisterCustomerRequestDto
    {
        public string name { get; set; }
        public string surname { get; set; }
        public string userName { get; set; }
        public string emailAddress { get; set; }
        public string password { get; set; }
        public string dob { get; set; }
        public string title { get; set; }
        public string bvn { get; set; }
        public string phoneNumber { get; set; }
        public string address { get; set; }
        public string stateOfResidence { get; set; }
        public string captchaResponse { get; set; }
    }

}
