using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Messaging;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.OnboardingNotification
{

    public class OnboardingNotificationRepository
    {

        private readonly AppSettings _appSettings;
        private readonly JobEmailService _emailService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(OnboardingNotificationRepository));
        public OnboardingNotificationRepository(IOptions<AppSettings> appSettings, JobEmailService emailService, IServiceProvider service)
        {
            _appSettings = appSettings.Value;
            Services = service;
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }
        public IServiceProvider Services { get; }

        public async Task<String> SendNotification(NotificationToProcessOnboardingViewModel item)
        {
            try
            {

                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    //Send email
                    var user = await context.ClientAuthentication
                              .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);

                    var emailModal = new EmailRequestDto
                    {
                        Subject = "Complete Onboarding",
                        DestinationEmail = item.Email,
                        SourceEmail = _appSettings.senderEmailInfo
                    };

                    var mailBuilder = new StringBuilder();
                    mailBuilder.AppendLine("Dear" + " " + user.FullName + "," + "<br />");
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("Thank you for signing up on our Socialpay platform" + " .<br />");
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("We however observed that you are yet to complete your profile and this may affect your ability to create payment links to sell your goods and services via Socialpay.<br />");
                    mailBuilder.AppendLine("1. Sign into your account with your username (email) and password from the socialpay website https://mysocialpay.ng. You will be promoted to use the verification link if you are yet to validate your email. <br />");
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("2. Complete your business information details such as company/personal name and logo from your dashboard.     <br />");
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("3. Input your account details i.e active account number and name. While we encourage that you use your Sterling bank account if you already have one, note also that the platform accepts and can process payments to other banks outside Sterling as long as the account belongs to you. <br />");
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("4. Well done, now you can begin to create payment links and even update your store with details of your products and services to showcase them to your clients<br />");
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("Links can shared using various channels where your customers can find you.<br />");
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("Socialpay comes with so many amazing benefits. Why not get onboard immediately and grow your business.< br />");
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("If you require more information, please give us a call on 017004271 to have all your questions answered.< br />");
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("With love from Alex.<br />");
                    emailModal.EmailBody = mailBuilder.ToString();

                    var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

                    // if (sendMail != AppResponseCodes.Success) log it some where

                    var payloadOnboardingNotification = new OnboardingNotiification
                    {
                        ClientAuthenticationId = item.ClientAuthenticationId,
                        notificationType = user.StatusCode

                    };

                    await context.AddAsync(payloadOnboardingNotification);
                    await context.SaveChangesAsync();
                }

                return "True";

            }
            catch (Exception ex)
            {
                return "Faled";
            }
        }



    }

}
