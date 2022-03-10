using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Messaging;
using SocialPay.Core.Messaging.SendGrid;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Repositories.Invoice
{
    public class InvoiceService
    {
        private readonly SocialPayDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AppSettings _appSettings;
        private readonly EmailService _emailService;
        private readonly SendGridEmailService _sendGridEmailService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(InvoiceService));


        public InvoiceService(SocialPayDbContext context,
            IHostingEnvironment environment, IOptions<AppSettings> appSettings,
            EmailService emailService, SendGridEmailService sendGridEmailService)
        {
            _context = context;
            _hostingEnvironment = environment;
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _sendGridEmailService = sendGridEmailService;
        }

        public async Task<List<InvoicePaymentInfo>> GetInvoicePaymentInfosAsync()
        {
            return await _context.InvoicePaymentInfo.ToListAsync();
        }


        public async Task<List<InvoicePaymentLink>> GetInvoicePaymentLinksAsync(long clientId)
        {
            return await _context.InvoicePaymentLink.Where(x=>x.ClientAuthenticationId == clientId).ToListAsync();
        }

        public async Task<WebApiResponse> GetInvoiceByClientId(long clientId)
        {
            var invoiceView = new List<MerchantInvoiceViewModel>();
            _log4net.Info("GetInvoiceByClientId request" + " | " + clientId + " | " + DateTime.Now);

            try
            {
                var getInvoice = await _context.InvoicePaymentLink
                    .Where(x => x.ClientAuthenticationId == clientId).ToListAsync();

                if (getInvoice.Count == 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Data = invoiceView };

                var config = new MapperConfiguration(cfg => cfg.CreateMap<InvoicePaymentLink, MerchantInvoiceViewModel>());
                var mapper = config.CreateMapper();
                invoiceView = mapper.Map<List<MerchantInvoiceViewModel>>(getInvoice);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = invoiceView };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetInvoiceByClientId" + " | " + clientId + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = invoiceView };
            }
        }

        public async Task<WebApiResponse> SendInvoiceAsync(string destinationEmail, decimal amount, decimal totalAmount,
           DateTime tranDate, string invoicename, string transactionReference, decimal discount = 0, decimal VAT = 0, decimal shippingFee = 0)
        {
            try
            {
                
                _log4net.Info("SendInvoiceAsync request" + " | " + transactionReference + " | " + destinationEmail + " | "+ DateTime.Now);

                var emailModal = new EmailRequestDto
                {
                    Subject = "Invoice" + " " + Guid.NewGuid().ToString() + " ",
                    SourceEmail = "info@sterling.ng",
                    DestinationEmail = destinationEmail,
                    //DestinationEmail = "festypat9@gmail.com",
                };
                var mailBuilder = new StringBuilder();
                string folderPath = Path.Combine(this._hostingEnvironment.WebRootPath, _appSettings.EmailTemplatesPath);

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                emailModal.EmailBody = mailBuilder.ToString();
                
                var path = _hostingEnvironment.WebRootFileProvider.GetFileInfo(_appSettings.EmailTemplatesPath + "/" + _appSettings.InvoiceTemplateDescription)?.PhysicalPath;
               
                using (StreamReader reader = new StreamReader(path))
                {
                        emailModal.EmailBody = reader.ReadToEnd();

                        var builder = new StringBuilder(emailModal.EmailBody);

                        builder.Replace("{VAT}", Convert.ToString(VAT));
                        builder.Replace("{Discount}", Convert.ToString(discount));
                        builder.Replace("{amount}", Convert.ToString(amount));
                        builder.Replace("{totalamount}", Convert.ToString(totalAmount));
                        builder.Replace("{trandate}", Convert.ToString(tranDate));
                        builder.Replace("{invoicename}", invoicename);
                        builder.Replace("{paymentLink}", $"{_appSettings.invoicePaymentlinkUrl}{transactionReference}");
                        builder.Replace("{currentyear}", Convert.ToString(DateTime.Now.Year));
                        builder.Replace("{transactionReference}", transactionReference);
                       // builder.Replace("{tax}", "0.00");
                        builder.Replace("{shippingfee}" , Convert.ToString(shippingFee));
                        emailModal.EmailBody = builder.ToString();
                    }

                try
                {
                    //var sendMail = await _sendGridEmailService.SendMail(emailModal.EmailBody, emailModal.DestinationEmail, emailModal.Subject);

                    await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success" };
                }
                catch (Exception ex)
                {
                    _log4net.Error("Error occured" + " | " + "SendInvoiceAsync" + " | " + transactionReference + " | " + destinationEmail +" | "+ ex.Message.ToString() + " | " + DateTime.Now);

                    return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError};
                }

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "SendInvoiceAsync" + " | " + transactionReference + " | " + destinationEmail + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetInvoiceTransactionDetails(long clientId)
        {
            var response = new List<InvoicePaymentInfoViewModel>();
            //clientId = 30043;
            try
            {
                var getInvoice = await GetInvoicePaymentLinksAsync(clientId);

                var result = (from a in getInvoice
                              join b in _context.InvoicePaymentInfo on a.InvoicePaymentLinkId
                              equals b.InvoicePaymentLinkId
                              select new InvoicePaymentInfoViewModel
                              {
                                  Email = b.Email,
                                  Channel = b.Channel,
                                  CustomerTransactionReference = b.CustomerTransactionReference,
                                  Fullname = b.Fullname,
                                  Message = b.Message,
                                  PhoneNumber = b.PhoneNumber,
                                  TransactionReference = b.TransactionReference,
                                  DateEntered = b.DateEntered, Qty = a.Qty, TotalAmount = a.TotalAmount,
                                  UnitPrice = a.UnitPrice, TransactionStatus = b.TransactionStatus
                              }).OrderByDescending(x=>x.DateEntered).ToList();

                response = result;

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = response };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetInvoiceTransactionDetails" + " | " + clientId + " | " +  ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = response };
            }
        }

    }
}
