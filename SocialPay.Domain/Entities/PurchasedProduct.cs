using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class PurchasedProduct : BaseEntity
    {
        public long PurchasedProductId { get; set; }
        public long ProductId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public long Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;
        public virtual ICollection<Product> Product { get; set; }

    }
}
