using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class WalletTransferRequestLog
    {
        public long WalletTransferRequestLogId { get; set; }
        public string TransactionReference { get; set; }
        public string CustomerTransactionReference { get; set; }
        public string amt { get; set; }
        public string toacct { get; set; }
        public string frmacct { get; set; }
        public string paymentRef { get; set; }
        public string remarks { get; set; }
        public int channelID { get; set; }
        public string CURRENCYCODE { get; set; }
        public int TransferType { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
    }
}
