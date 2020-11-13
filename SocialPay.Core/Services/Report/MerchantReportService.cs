﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Core.Messaging;
using SocialPay.Core.Repositories.Customer;
using SocialPay.Core.Repositories.Invoice;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Report
{
    public class MerchantReportService
    {
        private readonly SocialPayDbContext _context;
        private readonly ICustomerService _customerService;
        private readonly TransactionReceipt _transactionReceipt;
        private readonly InvoiceService _invoiceService;
        private readonly IDistributedCache _distributedCache;
        private readonly AppSettings _appSettings;
        public MerchantReportService(SocialPayDbContext context,
            ICustomerService customerService, TransactionReceipt transactionReceipt,
            InvoiceService invoiceService, IDistributedCache distributedCache, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _customerService = customerService;
            _transactionReceipt = transactionReceipt;
            _invoiceService = invoiceService;
            _distributedCache = distributedCache;
            _appSettings = appSettings.Value;
        }

        public async Task<WebApiResponse> GetMerchants()
        {
            //var result = new List<MerchantsViewModel>();
            var result = new List<MerchantBusinessInfoViewModel>();
            try
            {

                var getMerchantInfo = await _context.ClientAuthentication
                    .Where(x=>x.RoleName == RoleDetails.Merchant).ToListAsync();

                var response = (from c in getMerchantInfo
                                join b in _context.MerchantBusinessInfo on c.ClientAuthenticationId equals b.ClientAuthenticationId
                                join m in _context.MerchantBankInfo on c.ClientAuthenticationId equals m.ClientAuthenticationId
                                select new MerchantBusinessInfoViewModel
                                {
                                    BankInfo = new BankInfoViewModel 
                                    { 
                                        AccountName = m.AccountName, BankName = m.BankName,
                                        BVN = m.BVN, Country = m.Country, Currency = m.Currency,
                                        Nuban = m.Nuban
                                    },
                                    BusinessEmail = b.BusinessEmail, Country = b.Country,
                                    BusinessPhoneNumber = b.BusinessPhoneNumber, BusinessName = b.BusinessName,
                                    Chargebackemail = b.Chargebackemail,
                                    Logo = _appSettings.BaseApiUrl + b.FileLocation + "/" + b.Logo
                                }).ToList();
                result = response;
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
                //var clients = await _context.ClientAuthentication
                //    .Include(x => x.MerchantBankInfo)
                //    .Include(x => x.MerchantBusinessInfo)
                //    .Include(x => x.MerchantActivitySetup)
                //    .Include(x => x.MerchantPaymentSetup)
                //    .Where(x => x.RoleName == RoleDetails.Merchant).ToListAsync();

                //foreach (var item in clients)
                //{
                //    result1.Add(new MerchantBusinessInfoViewModel {  BusinessEmail = item.MerchantBusinessInfo.Select(x=>x.BusinessEmail).First() })
                //}


                //var bankInfo = new List<BankInfoViewModel>();
                //var businessInfo = new List<BusinessInfoViewModel>();
                //foreach (var item in clients)
                //{
                //    bankInfo.Add(new BankInfoViewModel { AccountName = item.MerchantBankInfo.Select(x => x.AccountName).FirstOrDefault() });
                //    bankInfo.Add(new BankInfoViewModel { BankName = item.MerchantBankInfo.Select(x => x.BankName).FirstOrDefault() });
                //    businessInfo.Add(new BusinessInfoViewModel { BusinessEmail = item.MerchantBusinessInfo.Select(x => x.BusinessEmail).FirstOrDefault() });
                //    businessInfo.Add(new BusinessInfoViewModel { BusinessName = item.MerchantBusinessInfo.Select(x => x.BusinessName).FirstOrDefault() });
                //}
                //result.Add(new MerchantsViewModel { bankInfo = bankInfo });
                //result.Add(new MerchantsViewModel { businessInfo = businessInfo });
                //return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
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

                var validateCustomer = await _customerService.GetTransactionLogsByReference(model.CustomerTransactionReference);

                //var validateCustomer = validateTransaction.CustomerTransaction
                //    .SingleOrDefault(x => x.CustomerTransactionId == model.CustomerTransactionId);
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


        public async Task<WebApiResponse> GetAllLoggedDisputes(long clientId, bool IsAdmin)
        {
            var result = new List<ItemDisputeViewModel>();
            try
            {
                //clientId = 30032;
                if(IsAdmin)
                {
                    var getallDisputes = await _context.DisputeRequestLog.ToListAsync();

                    var dataResponse = (from d in getallDisputes
                                       join t in _context.TransactionLog on d.TransactionReference equals t.TransactionReference
                                        select new ItemDisputeViewModel
                                        {
                                            Comment = d.DisputeComment,
                                            TransactionReference = t.TransactionReference,
                                            CustomerTransactionReference = t.CustomerTransactionReference,
                                            DateEntered = d.DateEntered,
                                            Document = _appSettings.BaseApiUrl + d.FileLocation + "/" + d.DisputeFile
                                        }).ToList();

                    result = dataResponse;
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };

                }
                var getTransactions = await _context.TransactionLog
                    .Where(x => x.MerchantInfo == clientId || x.ClientAuthenticationId == clientId).ToListAsync();
              
                var response = (from t in getTransactions
                                join d in _context.DisputeRequestLog on t.TransactionReference equals d.TransactionReference
                              select new ItemDisputeViewModel
                              {
                                  Comment = d.DisputeComment, TransactionReference = t.TransactionReference,
                                  CustomerTransactionReference = t.CustomerTransactionReference,
                                  DateEntered = d.DateEntered, Document = _appSettings.BaseApiUrl + d.FileLocation + "/" + d.DisputeFile
                              }).ToList();

                result = response;
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }



        public async Task<WebApiResponse> GetAllInvoiceByMerchantId(long clientId)
        {
            try
            {
                //clientId = 30032;
                var result = await _invoiceService.GetInvoiceByClientId(clientId);
                return result;
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetAllEscrowTransactions(long clientId, string status)
        {
            var result = new List<EscrowViewModel>();
            try
            {
                //clientId = 30032;
                var getTransactions = await _context.MerchantPaymentSetup
                    .Include(c => c.CustomerTransaction)
                    .Include(c => c.CustomerOtherPaymentsInfo)
                    .Where(x => x.ClientAuthenticationId == clientId 
                    && x.PaymentCategory == MerchantPaymentLinkCategory.Escrow
                    || x.PaymentCategory == MerchantPaymentLinkCategory.OneOffEscrowLink).ToListAsync();
                if (getTransactions.Count == 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                var response = (from m in getTransactions
                              //  join i in _context.ItemAcceptedOrRejected on m.TransactionReference equals i.TransactionReference
                                join t in  _context.TransactionLog on m.TransactionReference equals t.TransactionReference
                                where t.OrderStatus == status select new EscrowViewModel {
                                ShippingFee = m.ShippingFee, PaymentCategory = m.PaymentCategory, PaymentLinkName = m.PaymentLinkName,
                                MerchantAmount = m.MerchantAmount, DeliveryMethod = m.DeliveryMethod,
                                MerchantDescription = m.MerchantDescription, TotalAmount = m.TotalAmount,
                                Channel = t.Category, ClientId = t.ClientAuthenticationId, CustomerTransactionReference = t.CustomerTransactionReference,
                                }).ToList();

                result = response;
                //result = getTransactions.Select(p => new EscrowViewModel
                //{
                //    PaymentLinkName = p.PaymentLinkName, MerchantAmount = p.MerchantAmount,
                //    PaymentCategory = p.PaymentCategory, DeliveryMethod = p.DeliveryMethod, 
                //    PaymentLinkUrl = p.PaymentLinkUrl, MerchantDescription = p.MerchantDescription,
                //    ShippingFee = p.ShippingFee, TotalAmount = p.TotalAmount,
                //    Channel = p.CustomerTransaction.Count == 0 ? string.Empty : p.CustomerTransaction.Select(x=>x.Channel).First(),
                //}).ToList();

                return new WebApiResponse {ResponseCode = AppResponseCodes.Success, Data = result };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<UserInfoViewModel> RedisCacheTest()
        {
            try
            {
                var cacheKey = "40068";
                //var cacheKey = "festypat";
                string serializedCustomerList;
                var userInfo = new UserInfoViewModel{ };
                var redisCustomerList = await _distributedCache.GetAsync(cacheKey);
                if(redisCustomerList != null)
                {
                    serializedCustomerList = Encoding.UTF8.GetString(redisCustomerList);
                    return JsonConvert.DeserializeObject<UserInfoViewModel>(serializedCustomerList);
                }
                await _distributedCache.RemoveAsync(cacheKey);
                userInfo.Email = "debby";
                userInfo.StatusCode = "076";
                serializedCustomerList = JsonConvert.SerializeObject(userInfo);
                redisCustomerList = Encoding.UTF8.GetBytes(serializedCustomerList);
                var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(3))
                .SetSlidingExpiration(TimeSpan.FromMinutes(1));
                await _distributedCache.SetAsync(cacheKey, redisCustomerList, options);
                
                return new UserInfoViewModel();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
