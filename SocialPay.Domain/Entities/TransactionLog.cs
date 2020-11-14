using System;

namespace SocialPay.Domain.Entities
{
    public class TransactionLog
    {
        public long TransactionLogId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public long CustomerInfo { get; set; }
        public string CustomerEmail { get; set; }
        public string Category { get; set; }
        public string PaymentChannel { get; set; }
        public string OrderStatus { get; set; }
        public string DeliveryDayTransferStatus { get; set; }
        public string Message { get; set; }
        public string TransactionReference { get; set; }
        public string CustomerTransactionReference { get; set; }
        public bool Status { get; set; }
        public bool IsApproved { get; set; }
        public bool IsQueuedPayWithCard { get; set; }
        public bool IsCompletedPayWithCard { get; set; }
        public bool IsWalletQueued { get; set; }
        public bool IsWalletCompleted { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsNotified { get; set; }
        public bool TransactionCompleted { get; set; }
        public string PaymentReference { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime DeliveryFinalDate { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public DateTime DateNotified { get; set; }
        public DateTime WalletFundDate { get; set; }
        public DateTime AcceptRejectLastDateModified { get; set; }
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
