using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class TransactionLog
    {
        public long TransactionLogId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public long CustomerInfo { get; set; }
        [Column(TypeName = "NVARCHAR(40)")]
        public string CustomerEmail { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string Category { get; set; }
        [Column(TypeName = "NVARCHAR(10)")]
        public string PaymentChannel { get; set; }
        [Column(TypeName = "NVARCHAR(5)")]
        public string OrderStatus { get; set; }
        [Column(TypeName = "NVARCHAR(5)")]
        public string TransactionStatus { get; set; }
        [Column(TypeName = "NVARCHAR(5)")]
        public string ActivityStatus { get; set; }
        [Column(TypeName = "NVARCHAR(5)")]
        public string TransactionJourney { get; set; }
        [Column(TypeName = "NVARCHAR(5)")]
        public string DeliveryDayTransferStatus { get; set; }
        [Column(TypeName = "NVARCHAR(550)")]
        public string Message { get; set; }
        [Column(TypeName = "NVARCHAR(5)")]
        public string LinkCategory { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string TransactionReference { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
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
        [Column(TypeName = "NVARCHAR(90)")]
        public string PaymentReference { get; set; }
        [Column(TypeName = "decimal(18,2)")]
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
