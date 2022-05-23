using AutoMapper;
using SocialPay.Domain.Entities;
using SocialPay.Helper.Dto.Request;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Core.Extensions.AutoMapper
{
    public class AutoMapperProfile : Profile
    {

        public AutoMapperProfile()
        {
            CreateMap<SpectaRegisterCustomerRequest, RegisterCustomerRequestDto>().ReverseMap();
            //CreateMap<RegisterCustomerRequestDto, SpectaRegisterCustomerRequest>();
            CreateMap<AddOrrInformationRequest, AddOrrInformationRequestDto>().ReverseMap();
            //CreateMap<AddOrrInformationRequestDto, AddOrrInformationRequest>();
            CreateMap<ChargeCardRequest, ChargeCardRequestDto>().ReverseMap();
            //CreateMap<ChargeCardRequestDto, ChargeCardRequest>();
            CreateMap<ConfirmTicketRequest, ConfirmTicketRequestDto>().ReverseMap();
            //CreateMap<ConfirmTicketRequestDto, ConfirmTicketRequest>();
            CreateMap<CreateIndividualCurrentAccountRequest, CreateIndividualCurrentAccountRequestDto>().ReverseMap();
            //CreateMap<CreateIndividualCurrentAccountRequestDto, CreateIndividualCurrentAccountRequest>();
            CreateMap<RequestTicketRequest, RequestTicketDto>().ReverseMap();
            //CreateMap<RequestTicketDto, RequestTicketRequest>();
            CreateMap<SendEmailVerificationCodeRequest, SendEmailVerificationCodeRequestDto>().ReverseMap();
           // CreateMap<SendEmailVerificationCodeRequestDto, SendEmailVerificationCodeRequest>();
            CreateMap<SendOtpRequest, SendOtpRequestDto>().ReverseMap();
            //CreateMap<SendOtpRequestDto, SendOtpRequest>();
            CreateMap<SendPhoneRequest, SendPhoneRequestDto>().ReverseMap();
            //CreateMap<SendPhoneRequestDto, SendPhoneRequest>();
            CreateMap<SendPinRequest, SendPinRequestDto>().ReverseMap();
            //CreateMap<SendPinRequestDto, SendPinRequest>();
            CreateMap<SetDisbursementAccountRequest, SetDisbursementAccountRequestDto>().ReverseMap();
            //CreateMap<SetDisbursementAccountRequestDto, SetDisbursementAccountRequest>();
            CreateMap<ValidateChargeRequest, ValidateChargeRequestDto>().ReverseMap();
            //CreateMap<ValidateChargeRequestDto, ValidateChargeRequest>();
            CreateMap<VerifyBvnPhoneConfirmationCodeRequest, VerifyBvnPhoneConfirmationCodeRequestDto>().ReverseMap();
           // CreateMap<VerifyBvnPhoneConfirmationCodeRequestDto, VerifyBvnPhoneConfirmationCodeRequest>();
            CreateMap<VerifyEmailConfirmationCodeRequest, VerifyEmailConfirmationCodeRequestDto>().ReverseMap();
            //CreateMap<VerifyEmailConfirmationCodeRequestDto, VerifyEmailConfirmationCodeRequest>();
            CreateMap<CreateIndividualCurrentAccountRequest, CreateIndividualCurrentAccountRequestDto>().ReverseMap();
            //CreateMap<CreateIndividualCurrentAccountRequestDto, CreateIndividualCurrentAccountRequest>().ReverseMap();
            CreateMap<RequestTicketRequest, RequestTicketDto>().ReverseMap();
            //CreateMap<RequestTicketDto, RequestTicketRequest>();
            CreateMap<SendEmailVerificationCodeRequest, SendEmailVerificationCodeRequestDto>().ReverseMap();
            //CreateMap<SendEmailVerificationCodeRequestDto, SendEmailVerificationCodeRequest>();
            CreateMap<SendOtpRequest, SendOtpRequestDto>().ReverseMap();
           // CreateMap<SendOtpRequestDto, SendOtpRequest>();
            CreateMap<SendPhoneRequest, SendPhoneRequestDto>().ReverseMap();
            //CreateMap<SendPhoneRequestDto, SendPhoneRequest>();
            CreateMap<SendPinRequest, SendPinRequestDto>().ReverseMap();
            //CreateMap<SendPinRequestDto, SendPinRequest>();
            CreateMap<SetDisbursementAccountRequest, SetDisbursementAccountRequestDto>().ReverseMap();
            //CreateMap<SetDisbursementAccountRequestDto, SetDisbursementAccountRequest>();
            CreateMap<ValidateChargeRequest, ValidateChargeRequestDto>().ReverseMap();
            //CreateMap<ValidateChargeRequestDto, ValidateChargeRequest>();
            CreateMap<VerifyBvnPhoneConfirmationCodeRequest, VerifyBvnPhoneConfirmationCodeRequestDto>().ReverseMap();
            //CreateMap<VerifyBvnPhoneConfirmationCodeRequestDto, VerifyBvnPhoneConfirmationCodeRequest>();
            CreateMap<VerifyEmailConfirmationCodeRequest, VerifyEmailConfirmationCodeRequestDto>().ReverseMap();
            //CreateMap<VerifyEmailConfirmationCodeRequestDto, VerifyEmailConfirmationCodeRequest>();
            CreateMap<ProductInventory, ProductInventoryDto>().ReverseMap();
            //CreateMap<ProductInventoryDto, ProductInventory>();
            CreateMap<PurchasedProduct, PurchasedProductDto>().ReverseMap();
           // CreateMap<PurchasedProductDto, PurchasedProduct>();
            CreateMap<InventoryHistory, InventoryHistoryDto>().ReverseMap();
            //CreateMap<InventoryHistoryDto, InventoryHistory>();
            CreateMap<MerchantWallet, MerchantWalletRequestDto>().ReverseMap();
            //CreateMap<MerchantWalletRequestDto, MerchantWallet>();
           

        }


    }
}
