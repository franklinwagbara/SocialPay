using System.Collections.Generic;

namespace SocialPay.Helper.Dto.Request
{
    public class DynamicPaymentRequestDto
    {
        public string amount { get; set; }    

    }

    public class DynamicPaymentDefaultRequestDto : DynamicPaymentRequestDto
    {
        public string mchNo { get; set; }   
        public string subMchNo { get; set; }
        public string orderNo { get; set; }
        public string orderType { get; set; }
    }
    
    public class RegisterWebhookRequestDto
    {
        public string webHookUri { get; set; }
        public string description { get; set; }
        public bool? isActive { get; set; } 
        public List<WebFilter> filters { get; set; }
        public List<WebHookHeader> headers { get; set; }
    }

    public class WebFilter
    {
        public string name { get; set; }
    }

    public class WebHookHeader
    {
        public string headerName { get; set; }
        public string headerValue { get; set; }
    }

}
