using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class ConfirmTokenizationResponseDTO
    {
        public bool status { get; set; }
        public ResponsData data { get; set; }
    }

    public class ResponsData
    {
        public CardDetails CardDetails { get; set; }
        public string EmailAddress { get; set; }
    }

    public class CardDetails
    {
        public string TokenizationReference { get; set; }
        public string CardToken { get; set; }
    }
}
