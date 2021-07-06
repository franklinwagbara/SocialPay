using System;
using System.ComponentModel.DataAnnotations;

namespace SocialPay.Domain.Entities
{
    public class ProductOption
    {
        [Key]
        public int OptionId { get; set; }

        [Required(ErrorMessage = "Product option name is required")]
        public string Name { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;

    }
}
