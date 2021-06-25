using EwService;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Helper.Dto.Request;
using System;
using System.Threading.Tasks;

namespace SocialPay.Core.Messaging
{
    public class EmailService
    {
        private readonly AppSettings _appSettings;

        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(EmailService));
        public EmailService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public async Task<string> SendMail(EmailRequestDto model, string endpoint)
        {
            try
            {
                var emailNotifier = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap, endpoint);

                var result = await emailNotifier.SendMailAsync(model.DestinationEmail,
                    _appSettings.senderEmailInfo, model.EmailBody, model.Subject);

                _log4net.Info("Email service response" + " | " + model.DestinationEmail + " | " + DateTime.Now);

                return result;
            }
            catch (Exception ex)
            {

                _log4net.Error("Email service response: Error" + " | " + model.DestinationEmail + " | " + ex + " | "+ DateTime.Now);

                return "99";
            }
        }
    }
}
