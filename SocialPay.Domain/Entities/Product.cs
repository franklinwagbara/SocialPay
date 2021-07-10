﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Domain.Entities
{
    public class Product: BaseEntity
    {
        public long ProductId { get; set; }
        public long ProductCategoryId { get; set; }
        public string ProductName { get; set; }
        public string ProductReference { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool Options { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public ProductCategory ProductCategory { get; set; }
    }
}
