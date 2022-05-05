using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.ISpectaOnboardingService
{
    public interface ISpectaOnBoarding
    {
        Task<WebApiResponse> RegisterCustomer(RegisterCustomerRequestDto model);
        Task<WebApiResponse> SendEmailVerificationCode(SendEmailVerificationCodeRequestDto model);
        Task<WebApiResponse> VerifyEmailConfirmationCode(VerifyEmailConfirmationCodeRequestDto model);
        Task<WebApiResponse> SendBvnPhoneVerificationCode(string emailaddress);
        Task<WebApiResponse> VerifyBvnPhoneConfirmationCode(VerifyBvnPhoneConfirmationCodeRequestDto model);
        Task<WebApiResponse> LoggedInCustomerProfile(string email);
        Task<WebApiResponse> AddOrrInformation(AddOrrInformationRequestDto model);
        Task<WebApiResponse> Authenticate(AuthenticateRequestDto model);
        Task<WebApiResponse> BusinessSegmentAllList();
        Task<WebApiResponse> RequestTicket(RequestTicketDto model);
        Task<WebApiResponse> ConfirmTicket(ConfirmTicketRequestDto model);
        Task<WebApiResponse> CreateIndividualCurrentAccount(CreateIndividualCurrentAccountRequestDto model);
        Task<WebApiResponse> DisbursementAccount(SetDisbursementAccountRequestDto model);
        Task<WebApiResponse> ChargeCard(ChargeCardRequestDto model);
        Task<WebApiResponse> SendPhone(SendPhoneRequestDto model);
        Task<WebApiResponse> SendOtp(SendOtpRequestDto model);
        Task<WebApiResponse> SendPin(SendPinRequestDto model);
        Task<WebApiResponse> ValidateCharge(ValidateChargeRequestDto model);
        public Task<WebApiResponse> BankBranchList();
        public Task<WebApiResponse> AvailableBanksList(string email);
    }
}
