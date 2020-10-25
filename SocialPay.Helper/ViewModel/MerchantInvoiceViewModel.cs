using System;

namespace SocialPay.Helper.ViewModel
{
    public class MerchantInvoiceViewModel
    {
        public string InvoiceName { get; set; }
        public string TransactionReference { get; set; }
        public long Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public string CustomerEmail { get; set; }
        public bool TransactionStatus { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime DateEntered { get; set; }
    }
}
