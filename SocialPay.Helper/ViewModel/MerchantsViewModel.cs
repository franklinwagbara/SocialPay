using System.Collections.Generic;

namespace SocialPay.Helper.ViewModel
{
    public class MerchantsViewModel
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public List<BankInfoViewModel> bankInfo { get; set; }
        public List<BusinessInfoViewModel> businessInfo { get; set; }
    }

    public class BankInfoViewModel
    {
        public string BankName { get; set; }
        public string Nuban { get; set; }
        public string AccountName { get; set; }
        public string Currency { get; set; }
        public string BVN { get; set; }
        public string Country { get; set; }
    }

    public class BusinessInfoViewModel
    {
        public string BusinessName { get; set; }
        public string BusinessPhoneNumber { get; set; }
        public string BusinessEmail { get; set; }
        public string Country { get; set; }
        public string Chargebackemail { get; set; }
        public string Logo { get; set; }
    }

    public class MerchantBusinessInfoViewModel
    {
        public string BusinessName { get; set; }
        public string BusinessPhoneNumber { get; set; }
        public string BusinessEmail { get; set; }
        public string Country { get; set; }
        public string Chargebackemail { get; set; }
        public string Logo { get; set; }
        public BankInfoViewModel BankInfo { get; set; }
    }
}
