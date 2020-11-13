using Microsoft.AspNetCore.Http;
using SocialPay.Helper.Validator;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class DisputeItemRequestDto
    {
        public string TransactionReference { get; set; }
        public string CustomerTransactionReference { get; set; }
        public string Comment { get; set; }
        [DataType(DataType.Upload)]
        [MaxFileSize(1518592)]
        [AllowedExtensions(new string[] { ".jpg", ".png", ".jpeg", "pdf" })]
        public IFormFile Document { get; set; }
    }
}
