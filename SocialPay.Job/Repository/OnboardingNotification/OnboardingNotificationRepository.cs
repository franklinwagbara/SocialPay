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
                        Subject = "Complet Onboarding",
                        DestinationEmail = item.Email,
                        //DestinationEmail = "nchimdike@gmail.com",
                        SourceEmail = _appSettings.senderEmailInfo
                    };

                    var mailBuilder = new StringBuilder();

                    mailBuilder.AppendLine("Dear" + " " + user.FullName + "," + "<br />");
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("You have successfully sign up and your password is " + " .<br />");
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("Please confirm your sign up by clicking the link below.<br />");
                    mailBuilder.AppendLine("Best Regards,");
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
