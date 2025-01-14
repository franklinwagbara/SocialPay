﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class MerchantStore : BaseEntity
    {
        //public Store()
        //{
        //    Product = new HashSet<Product>();
        //}
        public long MerchantStoreId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string StoreName { get; set; }
        public string StoreLink { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "NVARCHAR(90)")]
        public string Image { get; set; }
        [Column(TypeName = "NVARCHAR(190)")]
        public string FileLocation { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public ClientAuthentication ClientAuthentication { get; set; }
       // public virtual ICollection<Product> Product { get; set; }
    }
}
