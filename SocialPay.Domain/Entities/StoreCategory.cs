using System;
using System.ComponentModel.DataAnnotations;

namespace SocialPay.Domain.Entities
{
    public class StoreCategory
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        public string Name { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;

    }
}
