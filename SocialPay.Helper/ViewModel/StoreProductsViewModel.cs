﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.ViewModel
{
    public class StoreProductsViewModel
    {
        public string StoreName { get; set; }
        public string ProductName { get; set; }
        public long ProductId { get; set; }
        public string ProductDescription { get; set; }
        public string StoreDescription { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Category { get; set; }
        public bool Options { get; set; }
        public List<ProductItemViewModel> ProductItemsViewModel { get; set; }
    }

    public class ProductItemViewModel
    {
        public string FileLocation { get; set; }
        public string Url { get; set; }
    }
}
