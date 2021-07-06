using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SocialPay.Helper.Dto.Request
{
    public class CreateSpectaRequestDto
    {
        [Required(ErrorMessage = "Company Name is required")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Company Address is required")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Email address is required")]
        public string EmailAddress { get; set; }
        public string WebsiteUrl { get; set; } 
        public int BusinessSegmentId { get; set; }
        [Required(ErrorMessage = "TinNumber is required")]
        public string TinNumber { get; set; }
        [Required(ErrorMessage = "RcNumber is required")]
        public string RcNumber { get; set; }
        public int YearsInBusiness { get; set; }
        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }
        [Required(ErrorMessage = "SourceOfAwareness is required")]
        public string SourceOfAwareness { get; set; }
        [Required(ErrorMessage = "StoreDescription is required")]
        public string StoreDescription { get; set; }

        [Required(ErrorMessage = "Phone number  is required")]
        public List<string> PhoneNumbers { get; set; }

        [Required(ErrorMessage = "Enter a minimum of one director")]
        public List<string> Directors { get; set; }

        public IFormFile InsidePicture { get; set; }
        public IFormFile OutsidePicture { get; set; }
        public IFormFile OtherPicture { get; set; }
    }
}
