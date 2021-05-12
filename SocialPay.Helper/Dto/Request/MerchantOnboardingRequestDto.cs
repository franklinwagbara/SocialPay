using Microsoft.AspNetCore.Http;
using SocialPay.Helper.Validator;
using System.ComponentModel.DataAnnotations;

namespace SocialPay.Helper.Dto.Request
{
    public class MerchantOnboardingInfoRequestDto
    {
        [Required(ErrorMessage = "Business Name")]
        [MaxLength(65, ErrorMessage = "Business name cannot be greater than 65")]
        public string BusinessName { get; set; }
        [Required(ErrorMessage = "Business Phone number")]
        [MaxLength(20, ErrorMessage = "Business Phone number can not be greater than 20")]
        [RegularExpression(@"^\d*[0-9]\d*$", ErrorMessage = "Only number between 0 - 9 allowed")]
        public string BusinessPhoneNumber { get; set; }
        [Required(ErrorMessage = "Business email")]
        [MaxLength(40, ErrorMessage = "Business Email can not be greater than 40")]
        public string BusinessEmail { get; set; }
        [MaxLength(40, ErrorMessage = "Tin can not be greater than 40")]
        public string Tin { get; set; }
        [MaxLength(50, ErrorMessage = "Specta Merchant ID can not be greater than 50")]
        public string SpectaMerchantID { get; set; }
        public string SpectaMerchantKey { get; set; }
        public string SpectaMerchantKeyValue { get; set; }
        [Required(ErrorMessage = "Business country")]
        public string Country { get; set; }       
        public string Chargebackemail { get; set; }
        //[Required(ErrorMessage = "Please select a file.")]
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
        //public string BVN { get; set; }
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

    public class CustomerReceiptRequestDto
    {
        public string TransactionReference { get; set; }
        public string CustomerTransactionReference { get; set; }

    }

    public class MerchantWalletRequestDto
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mobile { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
        public string CURRENCYCODE { get; set; }
        public string AccountTier { get; set; }
        //public decimal customField { get; set; }
    }

    public class WalletRequestDto
    {
        public string UserId { get; set; }
      
    }


    public class MerchantpaymentLinkRequestDto
    {
        [Required(ErrorMessage = "Payment Link Name")]
        public string PaymentLinkName { get; set; }
        public string MerchantDescription { get; set; }
        public decimal MerchantAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public string CustomUrl { get; set; }
        public string DeliveryMethod { get; set; }
        public long DeliveryTime { get; set; }
        public bool RedirectAfterPayment { get; set; }
        public string AdditionalDetails { get; set; }
        public string PaymentCategory { get; set; }
        public string PaymentMethod { get; set; }
       // [Required(ErrorMessage = "Please select a document.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1518592)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg", ".pdf", ".svg" })]
        public IFormFile Document { get; set; }
        //public decimal customField { get; set; }
    }
}
