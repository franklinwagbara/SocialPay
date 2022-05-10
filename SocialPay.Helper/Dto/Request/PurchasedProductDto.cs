using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class PurchasedProductDto
    {
        public long ProductId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public long Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;

    }
}
