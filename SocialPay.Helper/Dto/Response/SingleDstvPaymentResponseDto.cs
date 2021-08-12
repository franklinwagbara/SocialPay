using System.Collections.Generic;

namespace SocialPay.Helper.Dto.Response
{
    public class Customfield
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class CustomFields
    {
        public List<Customfield> customfield { get; set; }
    }

    public class SingleDstvPaymentResponseDto
    {
        public string merchantReference { get; set; }
        public string payUVasReference { get; set; }
        public string resultCode { get; set; }
        public string resultMessage { get; set; }
        public string vasProvider { get; set; }
        public string vasProviderReference { get; set; }
        public string transactionReference { get; set; }
        public CustomFields customFields { get; set; }
    }
}
