using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class ProductInventory : BaseEntity
    {
        public long ProductInventoryId { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual Product Product { get; set; }
    }
}
