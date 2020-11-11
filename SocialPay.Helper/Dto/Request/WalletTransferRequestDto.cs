using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class WalletTransferRequestDto
    {
        public string amt { get; set; }
        public string toacct { get; set; }
        public string frmacct { get; set; }
        public string paymentRef { get; set; }
        public string remarks { get; set; }
        public int channelID { get; set; }
        public string CURRENCYCODE { get; set; }
        public int TransferType { get; set; }
    }
}
