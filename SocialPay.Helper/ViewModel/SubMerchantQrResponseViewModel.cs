using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.ViewModel
{
    public class SubMerchantQrResponseViewModel
    {
        public long SubMerchantQRCodeOnboardingResponseId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public long SubMerchantQRCodeOnboardingId { get; set; }
        public string ReturnCode { get; set; }
        public string ReturnMsg { get; set; }
        public string MchNo { get; set; }
        public string MerchantName { get; set; }
        public string SubMchNo { get; set; }
        public string QrCode { get; set; }
        public string JsonResponse { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateEntered { get; set; }
    }
}
