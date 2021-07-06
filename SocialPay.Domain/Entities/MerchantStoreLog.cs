using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialPay.Domain.Entities
{
    public class MerchantStoreLog
    {
        [Key]
        public int Id { get; set; }
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

        [ForeignKey("CategoryId")]
        public int CategoryId { get; set; }
        [ForeignKey("OptionId")]
        public int? OptionId { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
        public virtual StoreCategory StoreCategory { get; set; }
        public virtual ProductOption ProductOption { get; set; }



    }

}
