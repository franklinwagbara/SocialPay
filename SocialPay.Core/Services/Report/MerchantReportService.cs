using Microsoft.EntityFrameworkCore;
using SocialPay.Core.Messaging;
using SocialPay.Core.Repositories.Customer;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Report
{
    public class MerchantReportService
    {
        private readonly SocialPayDbContext _context;
        private readonly ICustomerService _customerService;
        private readonly TransactionReceipt _transactionReceipt;

        public MerchantReportService(SocialPayDbContext context,
            ICustomerService customerService, TransactionReceipt transactionReceipt)
        {
            _context = context;
            _customerService = customerService;
            _transactionReceipt = transactionReceipt;
        }

        public async Task<WebApiResponse> GetMerchants()
        {
            var result = new List<MerchantsViewModel>();
            try
            {
                var clients = await _context.ClientAuthentication
                    .Include(x => x.MerchantBankInfo)
                    .Include(x => x.MerchantBusinessInfo)
                    .Include(x => x.MerchantActivitySetup)
                    .Include(x => x.MerchantPaymentSetup)
                    .Where(x => x.RoleName == RoleDetails.Merchant).ToListAsync();
                var bankInfo = new List<BankInfoViewModel>();
                var businessInfo = new List<BusinessInfoViewModel>();
                foreach (var item in clients)
                {
                    bankInfo.Add(new BankInfoViewModel { AccountName = item.MerchantBankInfo.Select(x => x.AccountName).FirstOrDefault() });
                    bankInfo.Add(new BankInfoViewModel { BankName = item.MerchantBankInfo.Select(x => x.BankName).FirstOrDefault() });
                    businessInfo.Add(new BusinessInfoViewModel { BusinessEmail = item.MerchantBusinessInfo.Select(x => x.BusinessEmail).FirstOrDefault() });
                    businessInfo.Add(new BusinessInfoViewModel { BusinessName = item.MerchantBusinessInfo.Select(x => x.BusinessName).FirstOrDefault() });
                }
                result.Add(new MerchantsViewModel { bankInfo = bankInfo });
                result.Add(new MerchantsViewModel { businessInfo = businessInfo });
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = result };
            }
        }

        public async Task<WebApiResponse> GenerateCustomerReceipt(CustomerReceiptRequestDto model)
        {
            try
            {

                //await _transactionReceipt.ReceiptTemplate("festypat9@gmail.com");
                var validateTransaction = await _customerService.GetTransactionReference(model.TransactionReference);

                if(validateTransaction == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidTransactionReference };

                var getMerchantInfo = await _customerService.GetMerchantInfo(validateTransaction.ClientAuthenticationId);

                var validateCustomer = validateTransaction.CustomerTransaction
                    .SingleOrDefault(x => x.CustomerTransactionId == model.CustomerTransactionId);
                await _transactionReceipt.ReceiptTemplate(validateCustomer.CustomerEmail,
                    validateTransaction.TotalAmount, validateCustomer.TransactionDate,
                    model.TransactionReference, getMerchantInfo == null ? string.Empty : getMerchantInfo.BusinessName );
                //Send Mail here

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

       
    }
}
