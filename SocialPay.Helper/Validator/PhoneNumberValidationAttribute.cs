using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SocialPay.Helper.Validator
{
    public class PhoneNumberValidationAttribute : ValidationAttribute
    {
      
        public IServiceProvider Services { get; }
        public PhoneNumberValidationAttribute(IServiceProvider services)
        {
            Services = services;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var file = value as string;
            using (var scope = Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                if (context.MerchantBusinessInfo.Any(x => x.BusinessEmail == file))
                    return new ValidationResult(GetErrorMessage());
            }
            
          

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"This file extension is not allowed!";
        }
    }
}
