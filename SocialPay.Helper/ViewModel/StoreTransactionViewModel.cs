using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.ViewModel
{
    public class StoreTransactionViewModel
    {
        public string PaymentReference { get; set; }
        public string CustomerEmail { get; set; }
        public string Category { get; set; }
        public string PaymentChannel { get; set; }
        public string TransactionStatus { get; set; }
        public string TransactionJourney { get; set; }
        public string Message { get; set; }
        public string LinkCategory { get; set; }
        public string TransactionReference { get; set; }
        public string CustomerTransactionReference { get; set; }
        public string TransactionType { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime TransactionDate { get; set; }
        public List<StoreTransactionDetailsViewModel> TransactionDetails { get; set; }
    }

    public class StoreTransactionDetailsViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal TotalAmount { get; set; }
        public string TransactionStatus { get; set; }
        public DateTime DateEntered { get; set; }
    }
}
