using System;

namespace SocialPay.Helper.Dto.Response
{
    public class MerchantResponseDTO
    {
        public long MerchantOtherBankInfoId { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string AccountName { get; set; }
        public string Nuban { get; set; }
        public bool DefaultAccount { get; set; }
        public DateTime DateEntered { get; set; }
    }
}
