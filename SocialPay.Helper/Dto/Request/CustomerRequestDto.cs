namespace SocialPay.Helper.Dto.Request
{
    public class CustomerRequestDto
    {
        public string TransactionReference { get; set; }
    }

    public class CustomerPaymentRequestDto
    {
        public string TransactionReference { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Document { get; set; }
        public string FileLocation { get; set; }
    }

    public class PaymentValidationRequestDto
    {
        public string TransactionReference { get; set; }
        public long CustomerId { get; set; }
        public string Message { get; set; }
     
    }

    public class AcceptRejectRequestDto
    {
        public string TransactionReference { get; set; }
        public string Comment { get; set; }
        public long RequestId { get; set; }
        public string Status { get; set; }
       
    }
}
