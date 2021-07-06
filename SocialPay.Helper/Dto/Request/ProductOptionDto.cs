using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class ProductOptionDto
    {
        [Key]
        public int OptionId { get; set; }

        [Required(ErrorMessage = "Product option name is required")]
        public string Name { get; set; }
        public DateTime DateAdded { get; set; }

    }
}
