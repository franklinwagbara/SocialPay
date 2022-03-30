using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Messaging;
using SocialPay.Core.Messaging.SendGrid;
using SocialPay.Core.Repositories.UserService;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.Account;
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
        private readonly SendGridEmailService _sendGridEmailService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AccountResetService));
        private readonly AccountLogger _accountLogger;

        public AccountResetService(UserRepoService userRepoService, EmailService emailService,
            IOptions<AppSettings> appSettings, SendGridEmailService sendGridEmailService, AccountLogger accountLogger)
        {
            _userRepoService = userRepoService;
            _emailService = emailService;
            _appSettings = appSettings.Value;
            _sendGridEmailService = sendGridEmailService;
            _accountLogger = accountLogger;
        }

        public async Task<WebApiResponse> GenerateResetToken(string email)
        {
            try
            {
                _accountLogger.LogRequest($"{"GenerateResetToken"}{" | "}{email}{" | " }{DateTime.Now}");

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

                //var sendMail = await _sendGridEmailService.SendMail(emailModal.EmailBody, emailModal.DestinationEmail, emailModal.Subject);

                 await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

                _accountLogger.LogRequest($"{"GenerateResetToken sent"}{ " | "}{email}{" | "}{DateTime.Now}");

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _accountLogger.LogRequest($"{"Error occured"}{ " | "}{"GenerateResetToken"}{ " | "}{email}{" | "}{ex.Message.ToString()}{" | "}{DateTime.Now}");

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> ResetGuestAccess(string email)
        {
            try
            {
                _accountLogger.LogRequest($"{"ResetGuestAccess"}{ " | "}{email}{" | "}{DateTime.Now}");

                var getUser = await _userRepoService.GetClientAuthenticationAsync(email);

                if (getUser == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                //if(getUser.RoleName != "Guest")
                //    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                var token = Guid.NewGuid().ToString().Substring(0, 10);

                var logRequest = await _userRepoService.LogAccountReset(getUser.ClientAuthenticationId, token);

                if (logRequest.ResponseCode != AppResponseCodes.Success)
                    return logRequest;

                var emailModal = new EmailRequestDto
                {
                    Subject = "Account details request",
                    SourceEmail = "info@sterling.ng",
                    DestinationEmail = email,
                    //  EmailBody = "Your onboarding was successfully created. Kindly use your email as username and" + "   " + "" + "   " + "as password to login"
                };

                var mailBuilder = new StringBuilder();
                mailBuilder.AppendLine("Dear" + " " + email + "," + "<br />");
                mailBuilder.AppendLine("<br />");
                mailBuilder.AppendLine("You have successfully requested for account reset. Please complete this request by clicking the link below.<br />");
                mailBuilder.AppendLine("Kindly use this token" + "  " + token + "  " + "and" + " " + _appSettings.newpasswordUrl + "<br />");
                // mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                mailBuilder.AppendLine("Best Regards,");
                emailModal.EmailBody = mailBuilder.ToString();

               // var sendMail = await _sendGridEmailService.SendMail(emailModal.EmailBody, emailModal.DestinationEmail, emailModal.Subject);

                 await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

                _accountLogger.LogRequest($"{"GenerateResetToken sent"}{" | "}{ email}{" | "}{DateTime.Now}");

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _accountLogger.LogRequest($"{"Error occured"}{ " | "}{"ResetGuestAccess"}{" | "}{email}{" | "}{ex.Message.ToString()}{ " | "}{ DateTime.Now}",true);

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
            catch (Exception ex)
            {
                _accountLogger.LogRequest($"{"Error occured"}{ " | "}{ "ChangePassword"}{" | "}{passwordResetDto.Token}{" | "}{ ex.Message.ToString()}{" | "}{DateTime.Now}");

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> PasswordReset(ResetExistingPasswordDto model, long clientId)
        {
            try
            {
               // clientId = 203;
                return await _userRepoService.ResetPassword(model, clientId);               
            }
            catch (Exception ex)
            {
                _accountLogger.LogRequest($"{"Error occured"}{" | "}{"PasswordReset"}{" | "}{clientId}{" | "}{ ex.Message.ToString()}{ " | "}{ DateTime.Now}",true);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
