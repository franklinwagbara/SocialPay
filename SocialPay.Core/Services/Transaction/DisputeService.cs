using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Transaction
{
    public class DisputeRepoService
    {
        private readonly SocialPayDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AppSettings _appSettings;
        public DisputeRepoService(SocialPayDbContext context, IHostingEnvironment environment,
            IOptions<AppSettings> appSettings)
        {
            _context = context;
            _hostingEnvironment = environment;
            _appSettings = appSettings.Value;
        }

        public async Task<WebApiResponse> LogDisputeRequest(DisputeItemRequestDto model, long clientId)
        {
            try
            {

                var logDispute = new DisputeRequestLog
                {
                    DisputeComment = model.Comment,
                    ClientAuthenticationId = clientId,
                    PaymentReference = model.PaymentReference,
                    TransactionReference = model.TransactionReference,
                    ProcessedBy = model.ProcessedBy
                };

                string path = Path.Combine(this._hostingEnvironment.WebRootPath, _appSettings.DisputeDocument);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fileName = string.Empty;
                var newFileName = string.Empty;
                fileName = (model.Document.FileName);
                var documentId = Guid.NewGuid().ToString("N").Substring(18);
                var FileExtension = Path.GetExtension(fileName);
                fileName = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.DisputeDocument) + $@"\{newFileName}";

                // concating  FileName + FileExtension
                newFileName = documentId + FileExtension;
                var filePath = Path.Combine(fileName, newFileName);
                logDispute.DisputeFile = newFileName;
                logDispute.FileLocation = _appSettings.MerchantLinkPaymentDocument;
                model.Document.CopyTo(new FileStream(filePath, FileMode.Create));
                await _context.DisputeRequestLog.AddAsync(logDispute);
                await _context.SaveChangesAsync();
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
