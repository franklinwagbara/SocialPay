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

namespace SocialPay.Core.Services.Store
{
    public class StoreReportRepository
    {
        private readonly SocialPayDbContext _context;

        public StoreReportRepository(SocialPayDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
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
    }
}
