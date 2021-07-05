using System.Collections.Generic;

namespace SocialPay.Helper.Dto.Request
{
    public class InvoiceRequestDto
    {
        public string InvoiceName { get; set; }
        public long Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public string CustomerEmail { get; set; }
        public string Description { get; set; }
        public string DueDate { get; set; }
        public decimal discount { get; set; }
    }

    public class InvoiceRequestMultipleEmailsDto
    {
        public string InvoiceName { get; set; }
        public long Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public List<string> CustomerEmail { get; set; }
        public string Description { get; set; }
        public string DueDate { get; set; }
        public decimal discount { get; set; }
    }
}
