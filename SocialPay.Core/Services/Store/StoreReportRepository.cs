using SocialPay.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using SocialPay.Helper;
using SocialPay.Helper.ViewModel;
using SocialPay.Helper.Dto.Response;
using Microsoft.EntityFrameworkCore;
using SocialPay.Helper.SerilogService.Store;

namespace SocialPay.Core.Services.Store
{
    public class StoreReportRepository
    {
        private readonly SocialPayDbContext _context;
        private readonly StoreLogger _storeLogger;
        public StoreReportRepository(SocialPayDbContext context, StoreLogger storeLogger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _storeLogger = storeLogger ?? throw new ArgumentNullException(nameof(storeLogger));
        }
        public async Task<WebApiResponse> GetStoreTransactionsAsync()
        {
            try
            {
                var query = await (from t in _context.TransactionLog
                                   where t.TransactionType == TransactionType.StorePayment
                                   join s in _context.StoreTransactionLog on t.PaymentReference equals s.PaymentReference

                                   join sd in _context.StoreTransactionLogDetails on s.StoreTransactionLogId equals sd.StoreTransactionLogId
                                   join p in _context.Products on sd.ProductId equals p.ProductId
                                   select new StoreTransactionViewModel
                                   {
                                       PaymentChannel = t.PaymentChannel,
                                       TransactionReference = t.TransactionReference,
                                       CustomerEmail = t.CustomerEmail,
                                       Message = t.Message,
                                       TransactionJourney = t.TransactionJourney,
                                       TransactionType = t.TransactionType,
                                       TransactionDate = t.TransactionDate,
                                       TransactionDetails = new List<StoreTransactionDetailsViewModel>()
                                    {
                                     new StoreTransactionDetailsViewModel
                                     {
                                         Color = sd.Color,
                                         Quantity = sd.Quantity,
                                         ProductName = p.ProductName,
                                         Size = sd.Size,
                                         TotalAmount = sd.TotalAmount,
                                         DateEntered = sd.DateEntered
                                     }
                                   
                                 }}).ToListAsync();


                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = query };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Internal error occured" };
            }

        }

        public async Task<WebApiResponse> GetMerchantWithStoreInfoAsync(UserDetailsViewModel userModel)
        {
            _storeLogger.LogRequest($"{"Generating report for merchant with stores"}{" "}{userModel.ClientId}{" - "}", false);

            try
            {
                var query = await (from m in _context.MerchantStore
                                 join c in _context.ClientAuthentication on m.ClientAuthenticationId equals c.ClientAuthenticationId

                                 select new MerchantStoreUsersViewModel
                                 {
                                     StoreName = m.StoreName,
                                     StoreLink = m.StoreLink,
                                     Description = m.Description,
                                     Fullname = c.FullName,
                                     PhoneNumber = c.PhoneNumber,
                                     DateEntered = m.DateEntered,
                                     Email = c.Email
                                 }).OrderByDescending(x=>x.DateEntered).ToListAsync();              

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = query, Message = "Success" };
            }
            catch (Exception ex)
            {
                _storeLogger.LogRequest($"{"An error occured while trying to get store merchants"}{" "}{userModel.ClientId}{" - "}{" - "}{ex}{" - "}{DateTime.Now}", true);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Internal error occured" };
            }

        }
    }
}
