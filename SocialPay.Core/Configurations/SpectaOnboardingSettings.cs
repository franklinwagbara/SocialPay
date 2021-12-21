using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Core.Configurations
{
    public class SpectaOnboardingSettings
    {
        public string SpectaRegistrationCustomerUrlExtension { get; set; }
        public string SpectaRegistrationAuthenticaUrlExtension { get; set; }
        public string SendEmailVerificationCodeUrlExtension { get; set; }
        public string VerifyEmailConfirmationCodeUrlExtension { get; set; }
        public string RegisterCustomerUrlExtension { get; set; }
        public string LoggedInCustomerProfileUrlExtension { get; set; }
        public string AddOrrInformationUrlExtension { get; set; }
        public string BusinessSegmentAllListUrlExtension { get; set; }
        public string RequestTicketUrlExtension { get; set; }
        public string ConfirmTicketUrlExtension { get; set; }
        public string CreateIndividualCurrentAccountUrlExtension { get; set; }
        public string DisbursementAccountUrlExtension { get; set; }
        public string ChargeCardUrlExtension { get; set; }
        public string SendPhoneUrlExtension { get; set; }
        public string ValidateChargeUrlExtension { get; set; }
        public string SendOtpUrlExtension { get; set; }
    }
}
