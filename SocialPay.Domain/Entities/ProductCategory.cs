using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class ProductCategory : BaseEntity
    {
        public ProductCategory()
        {
            Product = new HashSet<Product>();
        }
        public long ProductCategoryId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string CategoryName { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public ClientAuthentication ClientAuthentication { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}
