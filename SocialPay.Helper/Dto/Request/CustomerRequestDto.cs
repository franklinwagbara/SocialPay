using Microsoft.AspNetCore.Http;
using SocialPay.Helper.Validator;
using System.ComponentModel.DataAnnotations;

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
        public string Channel { get; set; }
        public string CustomerDescription { get; set; } 
        public decimal CustomerAmount { get; set; }
        [DataType(DataType.Upload)]
        [MaxFileSize(1518592)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg", "pdf" })]
        public IFormFile Document { get; set; }
        // public string FileLocation { get; set; }
    }

    public class PaymentValidationRequestDto
    {
        public string TransactionReference { get; set; }
        public string Channel { get; set; }
        public long CustomerId { get; set; }
        public string Message { get; set; }
     
    }

    public class AcceptRejectRequestDto
    {
        public string TransactionReference { get; set; }
        public string Comment { get; set; }
        public long RequestId { get; set; }
        public string Status { get; set; }
        [Required(ErrorMessage = "Customer Transaction Reference")]
        public string CustomerTransactionReference { get; set; }

    }
}
