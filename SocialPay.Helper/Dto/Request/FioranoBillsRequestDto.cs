using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class FioranoBillsRequestDto
    {
        public string TransactionReference { get; set; }
        public string DebitAmount { get; set; }
    }
}
