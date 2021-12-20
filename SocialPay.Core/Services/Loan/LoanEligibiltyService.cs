using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Loan
{
    public class LoanEligibiltyService
    {
        private readonly SocialPayDbContext _context;
        private readonly AppSettings _appSettings;
        public LoanEligibiltyService(SocialPayDbContext context, IOptions<AppSettings> appSettings)

        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _appSettings = appSettings.Value;

        }

        public async Task<WebApiResponse> MerchantEligibilty(long clientId)
        {
            try
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = 500000, Message = "Success", StatusCode = ResponseCodes.Success };
                //if (await MerchantDuration(clientId) < 90) return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = 0, Message = "Merchant is not eligible for loan", StatusCode = ResponseCodes.Success };
                //if (await MerchantTransactionCount(clientId) < 15) return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = 0, Message = "Merchant is not eligible for loan becuase of last 7 days  transaction count", StatusCode = ResponseCodes.Success };
                //if (await MerchantTransactionVolume(clientId) < 500000) return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = 0, Message = "Merchant is not eligible for loan becuase of last 30 days  transaction volume", StatusCode = ResponseCodes.Success };
                //var calculateLoanWithThisMonth = await MerchantDuration(clientId);
                //var calculatedMonthlyTransactions = await MerchantTransactionVolume(clientId, calculateLoanWithThisMonth);

                //return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = await CalculateMaximumEligibleLoan(calculatedMonthlyTransactions, calculateLoanWithThisMonth), Message = "Success", StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Internal error occured", Data = 0, StatusCode = ResponseCodes.InternalError };
            }

        }


        private async Task<int> MerchantDuration(long clientId)
        {

            var query = await _context.ClientAuthentication
                    .SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);

            return ((int)(DateTime.Now - query.DateEntered).TotalDays);
        }

        private async Task<int> MerchantTransactionCount(long clientId)
        {

            return await _context.TransactionLog
                  .Where(x => x.ClientAuthenticationId == clientId && x.TransactionDate >= DateTime.Now.AddDays(-7))
                 // .Where(x => x.TransactionDate >= DateTime.Now.AddDays(-7))
                  .CountAsync();

           // return result.Count();
        }
        private async Task<decimal> MerchantTransactionVolume(long clientId)
        {
            var result = await _context.TransactionLog
               .Where(x => x.ClientAuthenticationId == clientId && x.TransactionDate >= DateTime.Now.AddDays(-30))
              // .Where(x => x.TransactionDate >= DateTime.Now.AddDays(-30))
               .Select(x => new
               {
                   TotalAmount = x.TotalAmount,
                   ActualAmount = x.ActualAmount

               })
               .ToListAsync();
            var transactionValue = result.Sum(x => x.TotalAmount);

            return transactionValue;
        }
        private async Task<decimal> MerchantTransactionVolume(long clientId, int days)
        {
            if (days > 180)
                days = 180;

            var result = await _context.TransactionLog
               .Where(x => x.ClientAuthenticationId == clientId && x.TransactionDate >= DateTime.Now.AddDays(-days))
              // .Where(x => x.TransactionDate >= DateTime.Now.AddDays(-days))
               .Select(x => new
               {
                   TotalAmount = x.TotalAmount,
                   ActualAmount = x.ActualAmount

               })
               .ToListAsync();
            var transactionValue = result.Sum(x => x.TotalAmount);

            return transactionValue;
        }
        private async Task<decimal> CalculateMaximumEligibleLoan(decimal amount, int days)
        {
            if (days > 180)
                days = 180;

            var actualMonth = days / 30;
            decimal eligibleLoan = decimal.Zero;
            actualMonth = Int32.Parse(String.Format("{0:0.##}", actualMonth));

            if (actualMonth == 3)
            {
                eligibleLoan = Convert.ToDecimal(0.1) * amount;
            }
            else if (actualMonth == 4)
            {
                eligibleLoan = Convert.ToDecimal(0.2) * amount;
            }
            else if (actualMonth == 5)
            {
                eligibleLoan = Convert.ToDecimal(0.3) * amount;
            }
            else if (actualMonth == 6)
            {
                eligibleLoan = Convert.ToDecimal(0.5) * amount;
            }
            else
            {
                return 0;
            }

            return eligibleLoan;
        }

    }

}
