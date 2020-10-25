using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Helper.Dto.Request;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Messaging
{
    public class TransactionReceipt
    {
        private readonly EmailService _emailService;
        private readonly AppSettings _appSettings;
        private readonly IHostingEnvironment env;
        public TransactionReceipt(EmailService emailService,
            IOptions<AppSettings> appSettings, IHostingEnvironment env)
        {
            _emailService = emailService;
            _appSettings = appSettings.Value;
            this.env = env;
        }
        public async Task<bool> ReceiptTemplate(string destinationEmail,
            decimal amount, DateTime tranDate, string tranreference, string businessname)
        {
			try
			{
                var emailModal = new EmailRequestDto
                {
                    Subject = "Customer receipt" + " " + Guid.NewGuid().ToString() + " ",
                    SourceEmail = "info@sterling.ng",
                    DestinationEmail = destinationEmail,
                     //DestinationEmail = "festypat9@gmail.com",
                    //  EmailBody = "Your onboarding was successfully created. Kindly use your email as username and" + "   " + "" + "   " + "as password to login"
                };
                var mailBuilder = new StringBuilder();
                //mailBuilder.AppendLine("Dear" + " " + getMerchant.Email + "," + "<br />");
                //mailBuilder.AppendLine("<br />");
                //mailBuilder.AppendLine("An order has been rejected by" + "" + response.CustomerEmail + " " + ".<br />");
                ////mailBuilder.AppendLine("Kindly use this token" + "  " + newPin + "  " + "and" + " " + urlPath + "<br />");
                //// mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                //mailBuilder.AppendLine("Best Regards,");
                emailModal.EmailBody = mailBuilder.ToString();
                var path = env.WebRootFileProvider.GetFileInfo("socialpay-receipt.html")?.PhysicalPath;
                //Logo = getMerchantInfo == null ? string.Empty : _appSettings.BaseApiUrl + getMerchantInfo.FileLocation + "/" + getMerchantInfo.Logo
                using (StreamReader reader = new StreamReader(path))
               // using (StreamReader reader = new StreamReader("C:/stmt/socialpay-receipt.html"))
                //using (StreamReader reader = new StreamReader("C:/stmt/intro.html"))
                {
                    emailModal.EmailBody = reader.ReadToEnd();
                    StringBuilder b = new StringBuilder(emailModal.EmailBody);
                    b.Replace("%amount%", Convert.ToString(amount));
                    b.Replace("%trandate%", Convert.ToString(tranDate));
                    b.Replace("%tranreference%", tranreference);
                    b.Replace("%businessname%", businessname);
                    b.Replace("%currentyear%", Convert.ToString(DateTime.Now.Year));
                    emailModal.EmailBody = b.ToString();
                }
               
                try
                {
                    var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);
                    //Email sent
                }
                catch (Exception ex)
                {
                    //Email Not Sent
                }

                return true;
            }
			catch (Exception ex)
			{

                return false;
			}
        }
    }
}
