using SocialPay.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class InventoryHistoryDto : BaseEntity
    {
        public long ProductId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public long Quantity { get; set; }
        public decimal Amount { get; set; }
        public bool IsAdded { get; set; }
        public bool IsUpdated { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}
