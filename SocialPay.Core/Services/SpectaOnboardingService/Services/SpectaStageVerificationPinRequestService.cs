using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Messaging;
using SocialPay.Core.Services.SpectaOnboardingService.Interface;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaStageVerificationPinRequestService : ISpectaStageVerificationPinRequest
    {
        private readonly SocialPayDbContext _context;
        private readonly EmailService _emailService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaStageVerificationPinRequestService));
        private readonly AppSettings _appSettings;
        private readonly SpectaOnboardingSettings _spectaOnboardingSettings;
        public SpectaStageVerificationPinRequestService(SocialPayDbContext context, IOptions<AppSettings> appSettings,
            IOptions<SpectaOnboardingSettings> spectaOnboardingSettings,
            EmailService emailService)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _spectaOnboardingSettings = spectaOnboardingSettings.Value;
            _emailService = emailService;
        }
        public async Task<WebApiResponse> SpectaStageVerificationPinRequest(SpectaStageVerificationPinRequestDto model)
        {
            var token = $"{DateTime.Now.ToString()}{Guid.NewGuid().ToString()}{DateTime.Now.AddMilliseconds(120)}{Utilities.GeneratePin()}";
            var encryptedToken = token.Encrypt(_appSettings.appKey);
            var newPin = Utilities.GeneratePin();
            var encryptedPin = newPin.Encrypt(_appSettings.appKey);

            if (await _context.SpectaStageVerificationPinRequest.AnyAsync(x => x.Pin == encryptedPin))
            {
                newPin = string.Empty;
                newPin = Utilities.GeneratePin();
                encryptedPin = newPin.Encrypt(_appSettings.appKey);
            }
            var spectastage = new SpectaStageVerificationPinRequest()
            {
                Email = model.Email,
                Pin = encryptedPin,
                Status = false,
            };
            _context.SpectaStageVerificationPinRequest.Add(spectastage);
            await _context.SaveChangesAsync();

            var emailModal = new EmailRequestDto
            {
                Subject = "Specta Onboarding",
                DestinationEmail = model.Email,
            };
            var resetUrl = _spectaOnboardingSettings.SpectaPinVerificationWebportalUrl + encryptedToken;

            string urlPath = "<a href=\"" + resetUrl + "\">Click to continue with your Specta onboarding processes</a>";

            var mailBuilder = new StringBuilder();
            mailBuilder.AppendLine("Welcome Back" + " " + model.Email + "," + "<br />");
            mailBuilder.AppendLine("<br />");
            mailBuilder.AppendLine("Please continue with your onboarding journey by clicking the link below.<br />");
            mailBuilder.AppendLine("Kindly use this token" + "  " + newPin + "  " + "and" + " " + urlPath + "<br />");
            mailBuilder.AppendLine("<br />");
            mailBuilder.AppendLine("Best Regards,");
            emailModal.EmailBody = mailBuilder.ToString();

            var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

            if (sendMail != AppResponseCodes.Success)
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error occured while sending email", StatusCode = ResponseCodes.InternalError };
           
            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Email was sent successfully", StatusCode = ResponseCodes.Success };

        }

        public async Task<WebApiResponse> SpectaPinConfirmationRequest(SpectaPinConfirmationRequestDto model)
        {
            var encryptedPin = model.Pin.Encrypt(_appSettings.appKey);
            var details = await _context.SpectaStageVerificationPinRequest.SingleOrDefaultAsync(x => x.Pin == encryptedPin);
            if (details == null)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record Not Fount", StatusCode = ResponseCodes.RecordNotFound };
            }
            else
            {
                if (details.LastDateModified.AddMinutes(Convert.ToInt32(_appSettings.otpSession)) < DateTime.Now)
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.TokenExpired, Message = "Token has expired", StatusCode = ResponseCodes.InternalError };
                }
                else
                {
                    details.LastDateModified = DateTime.Now;
                    details.Status = true;
                    _context.SaveChanges();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", StatusCode = ResponseCodes.Success };
                }
            }
        }
    }
}
