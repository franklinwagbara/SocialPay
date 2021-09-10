using Microsoft.AspNetCore.Http;
using SocialPay.Helper.Validator;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialPay.Helper.Dto.Request
{


    public class TestVideoDto
    {
        public int Amount { get; set; }
        public virtual IList<string> AllowedCountries { get; set; } = new List<string>();
    }
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



    public class CustomerStorePaymentRequestDto
    {
        public string TransactionReference { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Channel { get; set; }
        public string CustomerDescription { get; set; }
        public decimal CustomerAmount { get; set; }
        public List<CheckOutItems> Items { get; set; }
    }

    public class CheckOutItems
    {
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class PaymentValidationRequestDto
    {
        public string TransactionReference { get; set; }
        public string Channel { get; set; }
        public long CustomerId { get; set; }
        public string InvoiceReference { get; set; }
        public string PaymentReference { get; set; }
        public string Message { get; set; }
     
    }

    public class TransactionPinRequestDto
    {
        public long TransactionPin { get; set; }
    }

    public class AcceptRejectRequestDto
    {
        public string TransactionReference { get; set; }
        public string Comment { get; set; }
        public string ProcessedBy { get; set; }
        public long RequestId { get; set; }
        public string Status { get; set; }
        [Required(ErrorMessage = "PaymentReference Reference")]
        public string PaymentReference { get; set; }

    }
}
