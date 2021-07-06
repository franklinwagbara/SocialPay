using System.ComponentModel.DataAnnotations;

namespace SocialPay.Helper.Dto.Request
{
    public class MerchantStoreLogRequest
    {
        [Required(ErrorMessage = "Product name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Product description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Product Url is required")]
        public string Url { get; set; }

        [Required(ErrorMessage = "Product Image is required")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Price is required")]
        public decimal Price { get; set; }
     
    }
}
