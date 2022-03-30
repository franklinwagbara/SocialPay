using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialPay.Domain;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.OnboardingNotification
{
    public class OnboardingNotificationService : IOnboardingNotificationService
    {
        private readonly OnboardingNotificationRepository _onboardingNotificationRepository;
        public OnboardingNotificationService(IServiceProvider services, OnboardingNotificationRepository onboardingNotificationRepository)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
            _onboardingNotificationRepository = onboardingNotificationRepository;
        }

        public IServiceProvider Services { get; }

        public async Task<string> SendNotificationToCompleteOnboarding()
        {
            try
            {
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    var query = await context.ClientAuthentication.Where(x => x.StatusCode != "00" && x.LastDateModified < DateTime.Now.AddMinutes(-10)).Take(5).ToListAsync();
                    //var query = await context.ClientAuthentication.Where(x => x.StatusCode != "00").ToListAsync();
                    foreach (var item in query)
                    {
                        var getUser = await context.OnboardingNotiification.SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId
                        && x.notificationType == item.StatusCode);
                        if (getUser == default)
                        {
                            var notificationPayload = new NotificationToProcessOnboardingViewModel
                            {
                                Email = item.Email,
                                ClientAuthenticationId = item.ClientAuthenticationId
                            };

                            await _onboardingNotificationRepository.SendNotification(notificationPayload);
                        }

                    }
                    //var data = (from c in context.ClientAuthentication
                    //            join n in context.OnboardingNotiification on c.ClientAuthenticationId equals n.ClientAuthenticationId
                    //            where c.StatusCode != n.notificationType && c.ClientAuthenticationId == n.ClientAuthenticationId
                    //            select c
                    //            )
                    //            .ToList();




                    //var notificationPayload = new List<NotificationToProcessOnboardingViewModel>();

                    //foreach (var item in query)
                    //{
                    //    notificationPayload.Add(new NotificationToProcessOnboardingViewModel
                    //    {
                    //        Email = item.Email,
                    //        ClientAuthenticationId = item.ClientAuthenticationId
                    //    });
                    //}
                    //await _onboardingNotificationRepository.SendNotification(notificationPayload);
                    return "Task Completed";

                }
            }
            catch (Exception ex)
            {
                return "Error";
            }

        }
    }

}
