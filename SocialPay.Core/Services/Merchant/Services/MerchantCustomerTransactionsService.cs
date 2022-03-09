using SocialPay.Core.Services.Merchant.Interfaces;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace SocialPay.Core.Services.Merchant.Services
{
    public class MerchantCustomerTransactionsService : IMerchantCustomerTransactions
    {
        private readonly SocialPayDbContext _context;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(MerchantCustomerTransactionsService));

        public MerchantCustomerTransactionsService(SocialPayDbContext context)
        {
            _context = context;
        }

        public async Task<WebApiResponse> CustomerTransactions()
        {
            try
            {
                var customertransactions = await (from t in _context.TransactionLog
                                                  join c in _context.ClientAuthentication on t.ClientAuthenticationId equals c.ClientAuthenticationId
                                                  select new CustomerTransactionResponseDto()
                                                  {
                                                      Email = c.Email,
                                                      FullName = c.FullName,
                                                      PhoneNumber = c.PhoneNumber,
                                                      TransactionLogId = t.TransactionLogId,
                                                      ClientAuthenticationId = c.ClientAuthenticationId,
                                                      TransactionReference = t.TransactionReference,
                                                      PaymentChannel = t.PaymentChannel,
                                                      TransactionStatus = t.TransactionStatus,
                                                      TotalAmount = t.TotalAmount,
                                                      Message = t.Message,
                                                      TransactionType = t.TransactionType,
                                                      transactionDate = t.TransactionDate
                                                  }).OrderByDescending(x=> x.transactionDate).ToListAsync();

                if (customertransactions.Count == 0)
                {
                    _log4net.Info("No Record Found" + " | " + "Customer Transaction Details");

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "No Recond Found", Data = customertransactions, StatusCode = ResponseCodes.RecordNotFound };
                }
                _log4net.Info("Successful" + " | " + "Customer Transaction Details");

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = customertransactions, StatusCode = ResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured " + ex.Message + " | " + "Customer Transaction Details");
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Data = null, Message = ex.Message, StatusCode = ResponseCodes.InternalError };

            }
        }
    }

}
