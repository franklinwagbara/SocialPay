using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.OnboardingNotification
{
    public interface IOnboardingNotificationService
    {
        Task<string> SendNotificationToCompleteOnboarding();
    }
}
