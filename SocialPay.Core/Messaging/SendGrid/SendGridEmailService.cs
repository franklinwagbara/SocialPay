using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Messaging.SendGrid
{
    public class SendGridEmailService
    {
        private readonly HttpClient _client;
        private readonly AppSettings _appSettings;
        public SendGridEmailService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

            _client = new HttpClient
            {
                BaseAddress = new Uri(_appSettings.sendGridAPIBaseUrl),
            };

            _client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _appSettings.sendGridbearerToken);
        }

        public async Task<WebApiResponse> SendMail(string emailBodyContents, string destinationEmail, string subject)
        {
            try
            {
                var emailCopy = new List<CC>();

                string encodedFileContents = emailBodyContents;

                //var attachedFileList = new List<Attachments>
                //        {
                //            new Attachments { content = encodedFileContents, filename = $"{sendMailRequestDto.transactionReference}{".pdf"}",
                //            type = MediaTypeNames.Application.Pdf }
                //         };

                var tosender = new List<To>
                        {
                            new To { email = destinationEmail, name = destinationEmail }
                        };

                var fileContent = new List<Content>
                        {
                            new Content { type = "text/html", value = emailBodyContents }
                        };

                var emailBody = new SendGridEmailRequest
                {
                    from = new From
                    {
                        email = _appSettings.sendGridEmailSender,
                        name = _appSettings.senderEmailName,
                    },
                    personalizations = new List<Personalization>()
                            {
                                new Personalization{ to = tosender, subject = subject
                            }
                        },

                    content = fileContent,

                    //attachments = attachedFileList

                };

                //if (emailCopy.Count > 0)
                //{
                //    emailBody.cc = emailCopy;
                //}

                //var client = new HttpClient
                //{
                //    BaseAddress = new Uri(_appSettings.sendGridAPIBaseUrl)
                //};

                var request = JsonConvert.SerializeObject(emailBody);

                // MessageServiceLogger.LogApiRequest("Email json request" + " - " + sendMailRequestDto.transactionReference + " - " + request + " - " + DateTime.Now, false);

              //  client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _appSettings.sendGridbearerToken);

                var sendRequest = await _client.PostAsync(_appSettings.sendGridEmailUrlExtension,
                new StringContent(request, Encoding.UTF8, "application/json"));

                var result = await sendRequest.Content.ReadAsStringAsync();

               // MessageServiceLogger.LogApiRequest("send grid response" + " - " + sendMailRequestDto.transactionReference + " - " + sendRequest + " - " + result + " - " + DateTime.Now, false);

                if (sendRequest.IsSuccessStatusCode)
                {

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }

                // return "00";
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed };

            }
            catch (Exception ex)
            {
                //MessageServiceLogger.LogApiRequest("Error occured initiating send grid message" + " - " + sendMailRequestDto.transactionReference + " - " + sendMailRequestDto.nuban + " - " + ex.Message.ToString() + " - " + DateTime.Now, true);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };

            }

        }
    }
}
