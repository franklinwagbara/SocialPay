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
            CreateMap<SpectaRegisterCustomerRequest, RegisterCustomerRequestDto>().ReverseMap();           
            //CreateMap<RegisterCustomerRequestDto, SpectaRegisterCustomerRequest>();

            CreateMap<SendEmailVerificationCodeRequest, SendEmailVerificationCodeRequestDto>().ReverseMap();
            //CreateMap<SendEmailVerificationCodeRequestDto, SendEmailVerificationCodeRequest>();
            CreateMap<VerifyEmailConfirmationCodeRequest, VerifyEmailConfirmationCodeRequestDto>().ReverseMap();
            //CreateMap<VerifyEmailConfirmationCodeRequestDto, VerifyEmailConfirmationCodeRequest>();
            CreateMap<VerifyBvnPhoneConfirmationCodeRequest, VerifyBvnPhoneConfirmationCodeRequestDto>().ReverseMap();
           // CreateMap<VerifyBvnPhoneConfirmationCodeRequestDto, VerifyBvnPhoneConfirmationCodeRequest>();

            CreateMap<ProductInventory, ProductInventoryDto>().ReverseMap();
            //CreateMap<ProductInventoryDto, ProductInventory>();

            CreateMap<AddOrrInformationRequest, AddOrrInformationRequestDto>().ReverseMap();
            CreateMap<ChargeCardRequestDto, ChargeCardRequest>().ReverseMap();

            CreateMap<ConfirmTicketRequest, ConfirmTicketRequestDto>().ReverseMap();
            CreateMap<ConfirmTicketRequestDto, ConfirmTicketRequest>().ReverseMap();

            CreateMap<RequestTicketRequest, RequestTicketDto>().ReverseMap();
            CreateMap<SendOtpRequestDto, SendOtpRequest>().ReverseMap();

            CreateMap<SetDisbursementAccountRequest, SetDisbursementAccountRequestDto>().ReverseMap();
           CreateMap<SendPinRequestDto, SendPinRequest>().ReverseMap();

            CreateMap<CreateIndividualCurrentAccountRequest, CreateIndividualCurrentAccountRequestDto>().ReverseMap();
            CreateMap<ValidateChargeRequestDto, ValidateChargeRequest>().ReverseMap();
            CreateMap<SendPhoneRequestDto, SendPhoneRequest>().ReverseMap();
        }
    }
}
