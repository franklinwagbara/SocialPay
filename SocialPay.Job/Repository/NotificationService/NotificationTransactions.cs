using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SocialPay.Helper;
using SocialPay.Core.Messaging;
using SocialPay.Helper.Dto.Request;
using SocialPay.Core.Configurations;
using Microsoft.Extensions.Options;

namespace SocialPay.Job.Repository.NotificationService
{
    public class NotificationTransactions
    {
        private readonly JobEmailService _emailService;
        private readonly AppSettings _appSettings;
        public NotificationTransactions(JobEmailService emailService, IOptions<AppSettings> appSettings,
            IServiceProvider service)
        {
            _emailService = emailService;
            _appSettings = appSettings.Value;
            Services = service;
        }
        public IServiceProvider Services { get; }
        public async Task<WebApiResponse> InitiatePendingNotifications(List<TransactionLog> pendingRequest)
        {
            try
            {
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    foreach (var item in pendingRequest)
                    {
                        var getTransInfo = await context.TransactionLog
                            .SingleOrDefaultAsync(x => x.TransactionLogId == item.TransactionLogId);
                        getTransInfo.IsNotified = true;
                        getTransInfo.LastDateModified = DateTime.Now;
                        getTransInfo.DateNotified = DateTime.Now;
                        context.Update(getTransInfo);                       

                        var getMerchantInf = await context.ClientAuthentication
                            .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);

                        var emailModal = new EmailRequestDto
                        {
                            Subject = "Accept/Reject order" + " - " + item.TransactionReference,
                            SourceEmail = "info@sterling.ng",
                            DestinationEmail = getMerchantInf.Email,
                            // DestinationEmail = "festypat9@gmail.com",
                            //  EmailBody = "Your onboarding was successfully created. Kindly use your email as username and" + "   " + "" + "   " + "as password to login"
                        };
                        var mailBuilder = new StringBuilder();
                        mailBuilder.AppendLine("Dear" + " " + getMerchantInf.Email + "," + "<br />");
                        mailBuilder.AppendLine();
                        mailBuilder.AppendLine("Your order with transaction reference " + " "+ item.TransactionReference + " "+ "is about to expire. Please login to accept/reject item");
                        mailBuilder.AppendLine();
                        //mailBuilder.AppendLine("Kindly use this token" + "  " + newPin + "  " + "and" + " " + urlPath + "<br />");
                        // mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                        mailBuilder.AppendLine("Best Regards,");
                        emailModal.EmailBody = mailBuilder.ToString();
                        var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);
                        if(sendMail == "00")
                        {
                            await context.SaveChangesAsync();
                        }

                    }
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }

            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
