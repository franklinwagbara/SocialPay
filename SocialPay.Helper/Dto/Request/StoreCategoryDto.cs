using System;
using System.ComponentModel.DataAnnotations;

namespace SocialPay.Helper.Dto.Request
{
    public class StoreCategoryDto
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        public string Name { get; set; }
        public DateTime DateAdded { get; set; }

    }
}
