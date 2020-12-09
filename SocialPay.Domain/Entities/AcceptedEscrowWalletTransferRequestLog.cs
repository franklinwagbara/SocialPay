using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class AcceptedEscrowWalletTransferRequestLog
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string PaymentReference { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string TransactionReference { get; set; }
        public string CustomerTransactionReference { get; set; }
        public string amt { get; set; }
        public string toacct { get; set; }
        public string frmacct { get; set; }
        public string remarks { get; set; }
        public int channelID { get; set; }
        public string CURRENCYCODE { get; set; }
        public int TransferType { get; set; }
        public string ChannelMode { get; set; }
        public string RequestId { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
