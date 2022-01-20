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
            CreateMap<VerifyBvnPhoneConfirmationCodeRequest, VerifyBvnPhoneConfirmationCodeRequestDto>();
            CreateMap<VerifyBvnPhoneConfirmationCodeRequestDto, VerifyBvnPhoneConfirmationCodeRequest>();

            CreateMap<ProductInventory, ProductInventoryDto>();
            CreateMap<ProductInventoryDto, ProductInventory>();

            CreateMap<AddOrrInformationRequest, AddOrrInformationRequestDto>();
            CreateMap<AddOrrInformationRequestDto, AddOrrInformationRequest>();

            CreateMap<ConfirmTicketRequest, ConfirmTicketRequestDto>();
            CreateMap<ConfirmTicketRequestDto, ConfirmTicketRequest>();

            CreateMap<RequestTicketRequest, RequestTicketDto>();
            CreateMap<RequestTicketDto, RequestTicketRequest>();

            CreateMap<SetDisbursementAccountRequest, SetDisbursementAccountRequestDto>();
            CreateMap<SetDisbursementAccountRequestDto, SetDisbursementAccountRequest>();

            CreateMap<CreateIndividualCurrentAccountRequest, CreateIndividualCurrentAccountRequestDto>();
            CreateMap<CreateIndividualCurrentAccountRequestDto, CreateIndividualCurrentAccountRequest>();
        }
    }
}
