using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Messaging;
using SocialPay.Core.Repositories.UserService;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Account
{
    public class AccountResetService
    {
        private readonly UserRepoService _userRepoService;
        private readonly EmailService _emailService;
        private readonly AppSettings _appSettings;
        
        public AccountResetService(UserRepoService userRepoService, EmailService emailService,
            IOptions<AppSettings> appSettings)
        {
            _userRepoService = userRepoService;
            _emailService = emailService;
            _appSettings = appSettings.Value;
        }

        public async Task<WebApiResponse> GenerateResetToken(string email)
        {
            try
            {
                var getUser = await _userRepoService.GetClientAuthenticationAsync(email);
                if(getUser == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                var token = Guid.NewGuid().ToString().Substring(0, 10);
                var logRequest = await _userRepoService.LogAccountReset(getUser.ClientAuthenticationId, token);
                if (logRequest.ResponseCode != AppResponseCodes.Success)
                    return logRequest;
                var emailModal = new EmailRequestDto
                {
                    Subject = "Account Reset",
                    SourceEmail = "info@sterling.ng",
                    DestinationEmail = email,
                    // DestinationEmail = "festypat9@gmail.com",
                    //  EmailBody = "Your onboarding was successfully created. Kindly use your email as username and" + "   " + "" + "   " + "as password to login"
                };
                
                var mailBuilder = new StringBuilder();
                mailBuilder.AppendLine("Dear" + " " + email + "," + "<br />");
                mailBuilder.AppendLine("<br />");
                mailBuilder.AppendLine("You have successfully requested a password reset. Please complete this request by clicking the link below.<br />");
                mailBuilder.AppendLine("Kindly use this token" + "  " + token + "  " + "and" + " " + _appSettings.newpasswordUrl + "<br />");
                // mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                mailBuilder.AppendLine("Best Regards,");
                emailModal.EmailBody = mailBuilder.ToString();
                var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> ChangePassword(PasswordResetDto passwordResetDto)
        {
            try
            {
                return await _userRepoService.ChangeUserPassword(passwordResetDto,
                    Convert.ToInt32(_appSettings.accountResetToken), _appSettings.appKey);
            }
            catch (Exception)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
