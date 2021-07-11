﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class ProductRequestDto
    {
        public long ProductCategoryId { get; set; }
        public long StoreId { get; set; }
        public string ProductName { get; set; }
        public string ProductReference { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool Options { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
    }

    public class ProductcategoryDto
    {
        public string CategoryName { get; set; }
    }
}
