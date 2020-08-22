using Microsoft.AspNetCore.Http;
using SocialPay.Helper.Validator;
using System.ComponentModel.DataAnnotations;

namespace SocialPay.Helper.Dto.Request
{
    public class MerchantOnboardingInfoRequestDto
    {
        [Required(ErrorMessage = "Business Name")]
        public string BusinessName { get; set; }
        [Required(ErrorMessage = "Business Phone number")]
       // [PhoneNumberValidationAttribute]
        public string BusinessPhoneNumber { get; set; }
        [Required(ErrorMessage = "Business email")]
        public string BusinessEmail { get; set; }
        [Required(ErrorMessage = "Business country")]
        public string Country { get; set; }       
        public string Chargebackemail { get; set; }
        [Required(ErrorMessage = "Please select a file.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1518592)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg" })]
        public IFormFile Logo { get; set; }
    }

    public class MerchantBankInfoRequestDto
    {
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string Nuban { get; set; }
        public string Currency { get; set; }
        public string BVN { get; set; }
        public string Country { get; set; }
        public bool DefaultAccount { get; set; }
    }

    public class MerchantActivitySetupRequestDto
    {
        public string PayOrchargeMe { get; set; }
        public bool ReceiveEmail { get; set; }
        public decimal WithinLagos { get; set; }
        public decimal OutSideLagos { get; set; }
        public decimal OutSideNigeria { get; set; }
        //public decimal customField { get; set; }
    }

    public class MerchantWalletRequestDto
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mobile { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string CURRENCYCODE { get; set; }
        //public decimal customField { get; set; }
    }
}
