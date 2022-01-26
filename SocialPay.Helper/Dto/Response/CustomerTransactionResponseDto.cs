using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class CustomerTransactionResponseDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public long TransactionLogId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string TransactionReference { get; set; }
        public string PaymentChannel { get; set; }
        public string TransactionStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public string Message { get; set; }
        public string TransactionType { get; set; }
        public DateTime transactionDate { get; set; }

    }
}
