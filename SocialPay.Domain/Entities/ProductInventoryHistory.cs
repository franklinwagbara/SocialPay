using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class ProductInventoryHistory : BaseEntity
    {
        public long ProductInventoryHistoryId { get; set; }
        public long ProdId { get; set; }
        public long ProductInventoryId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public long Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        public bool IsAdded { get; set; }
        public bool IsUpdated { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual ProductInventory ProductInventory { get; set; }
    }
}
