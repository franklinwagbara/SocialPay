using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Interface
{
    public interface ISpectaOnBoarding
    {
        Task<WebApiResponse> RegisterCustomer(RegisterCustomerRequestDto model);
        Task<WebApiResponse> SendEmailVerificationCode(SendEmailVerificationCodeRequestDto model);
        Task<WebApiResponse> VerifyEmailConfirmationCode(VerifyEmailConfirmationCodeRequestDto model);
        Task<WebApiResponse> SendBvnPhoneVerificationCode(string emailaddress);
        Task<WebApiResponse> VerifyBvnPhoneConfirmationCode(VerifyBvnPhoneConfirmationCodeRequestDto model);
        Task<WebApiResponse> LoggedInCustomerProfile(string email);
        Task<WebApiResponse> AddOrrInformation(AddOrrInformationRequestDto model, string email);
        Task<WebApiResponse> Authenticate(AuthenticateRequestDto model);
        Task<WebApiResponse> BusinessSegmentAllList(string email);
        Task<WebApiResponse> RequestTicket(RequestTicketDto model, string email);
        Task<WebApiResponse> ConfirmTicket(ConfirmTicketRequestDto model, string email);
        Task<WebApiResponse> CreateIndividualCurrentAccount(CreateIndividualCurrentAccountRequestDto model, string email);
        Task<WebApiResponse> DisbursementAccount(SetDisbursementAccountRequestDto model, string email);
        public Task<WebApiResponse> BankBranchList(string email);
        public Task<WebApiResponse> AvailableBanksList(string email);
        //Task<WebApiResponse> ChargeCard(ChargeCardRequestDto model, string email);
        //Task<WebApiResponse> SendPhone(SendPhoneRequestDto model, string email);
        //Task<WebApiResponse> SendOtp(SendOtpRequestDto model, string email);
        //Task<WebApiResponse> SendPin(SendPinRequestDto model, string email);
        //Task<WebApiResponse> ValidateCharge(ValidateChargeRequestDto model, string email);
    }

}
