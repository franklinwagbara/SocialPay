using EwService;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Helper.Dto.Request;
using System.Threading.Tasks;

namespace SocialPay.Core.Messaging
{
    public class EmailService
    {
        private readonly AppSettings _appSettings;

        public EmailService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public async Task<string> SendMail(EmailRequestDto model, string endpoint)
        {
            var emailNotifier = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap, endpoint);

            var result = await emailNotifier.SendMailAsync(model.DestinationEmail,
                _appSettings.senderEmailInfo, model.EmailBody, model.Subject);

            ////var result = await emailNotifier.SendMailAsync("festypat9@gmail.com",
            ////    model.SourceEmail, model.EmailBody, model.Subject);

            return result;
        }
    }
}
