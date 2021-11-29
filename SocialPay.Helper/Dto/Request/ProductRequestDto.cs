using Microsoft.AspNetCore.Http;
using SocialPay.Helper.Validator;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialPay.Helper.Dto.Request
{
    public class ProductRequestDto
    {
        public long ProductCategoryId { get; set; }
        public long StoreId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public List<int> Size { get; set; }
        public List<string> Color { get; set; }
        [Required(ErrorMessage = "Please select image.")]
        [DataType(DataType.Upload)]
        [MaxFileSize(1518592)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg", ".svg" })]
        public List<IFormFile> Image { get; set; }
    }
    public class ProductImageDto
    {
        public IFormFile Image { get; set; }
    }
    public class ProductcategoryDto
    {
        public string CategoryName { get; set; }
        public string Description { get; set; }
    }

    public class ProductUpdateDto
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public List<int> Size { get; set; }
        public List<string> Color { get; set; }
        [MaxFileSize(1518592)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg", ".svg" })]
        public List<IFormFile> Image { get; set; }
    }
}
