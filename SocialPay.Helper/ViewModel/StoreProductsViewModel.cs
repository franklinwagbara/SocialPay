using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.ViewModel
{
    public class StoreDetailsViewModel
    {
        public string TransactionReference { get; set; }
        public string StoreName { get; set; }
        public string StoreDescription { get; set; }
        public string StoreLogoUrl { get; set; }
        public List<StoreProductsDetailsViewModel> StoreDetails { get; set; }
    }


    public class StoreProductsDetailsViewModel
    {
        public string ProductName { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public string ProductDescription { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Category { get; set; }
        public bool Options { get; set; }
        public List<ProductItemViewModel> Products { get; set; }
    }


    public class StoreProductsViewModel
    {
        public string StoreName { get; set; }
        public string ProductName { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
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

    public class ProductsDetailsViewModel
    {
        public string ProductName { get; set; }
        public long ProductId { get; set; }
        public int Quantity { get; set; }
        public string ProductDescription { get; set; }
        public decimal Price { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public string Category { get; set; }
        public bool Options { get; set; }
        public List<ProductItemViewModel> ProductItemsViewModel { get; set; }
    }
}
