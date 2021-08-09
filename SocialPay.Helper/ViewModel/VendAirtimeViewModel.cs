using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.ViewModel
{
    public class VendAirtimeViewModel
    {
        public long VendAirtimeRequestLogId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string ReferenceId { get; set; }
        public string Translocation { get; set; }
        public string email { get; set; }
        public decimal Amount { get; set; }
        public string SubscriberInfo1 { get; set; }
        public string nuban { get; set; }
        public string Mobile { get; set; }
        public string TransactionType { get; set; }
        public string Paymentcode { get; set; }

        public int AppId { get; set; }
        public int RequestType { get; set; }
        public string TerminalID { get; set; }
        public DateTime DateEntered { get; set; }
    }
}
