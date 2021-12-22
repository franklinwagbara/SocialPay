using AutoMapper;
using SocialPay.Domain.Entities;
using SocialPay.Helper.Dto.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.AutoMapperSettings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<SpectaRegisterCustomerRequest, RegisterCustomerRequestDto>();           
            CreateMap<RegisterCustomerRequestDto, SpectaRegisterCustomerRequest>();

            CreateMap<SendEmailVerificationCodeRequest, SendEmailVerificationCodeRequestDto>();
            CreateMap<SendEmailVerificationCodeRequestDto, SendEmailVerificationCodeRequest>();
            CreateMap<VerifyEmailConfirmationCodeRequest, VerifyEmailConfirmationCodeRequestDto>();
            CreateMap<VerifyEmailConfirmationCodeRequestDto, VerifyEmailConfirmationCodeRequest>();
            CreateMap<ProductInventory, ProductInventoryDto>();
            CreateMap<ProductInventoryDto, ProductInventory>();
        }
    }
}
