using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Messaging;
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

        public InvoiceService(SocialPayDbContext context,
            IHostingEnvironment environment, IOptions<AppSettings> appSettings,
            EmailService emailService)
        {
            _context = context;
            _hostingEnvironment = environment;
            _appSettings = appSettings.Value;
            _emailService = emailService;
        }

        public async Task<WebApiResponse> GetInvoiceByClientId(long clientId)
        {
            var invoiceView = new List<MerchantInvoiceViewModel>();

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

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = invoiceView };
            }
        }

        public async Task<WebApiResponse> SendInvoiceAsync(string destinationEmail, decimal amount, decimal totalAmount,
            DateTime tranDate, string businessname)
        {
            try
            {
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
                {
                    Directory.CreateDirectory(folderPath);
                }
                emailModal.EmailBody = mailBuilder.ToString();
                var path = _hostingEnvironment.WebRootFileProvider.GetFileInfo(_appSettings.EmailTemplatesPath + "/" + _appSettings.InvoiceTemplateDescription)?.PhysicalPath;
                using (StreamReader reader = new StreamReader(path))
                {
                    emailModal.EmailBody = reader.ReadToEnd();
                    var builder = new StringBuilder(emailModal.EmailBody);
                    builder.Replace("%amount%", Convert.ToString(amount));
                    builder.Replace("%totalamount%", Convert.ToString(totalAmount));
                    builder.Replace("%trandate%", Convert.ToString(tranDate));
                    builder.Replace("%businessname%", businessname);
                    builder.Replace("%currentyear%", Convert.ToString(DateTime.Now.Year));
                    emailModal.EmailBody = builder.ToString();
                }

                try
                {
                    var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }
                catch (Exception ex)
                {
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError};
                }

            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
