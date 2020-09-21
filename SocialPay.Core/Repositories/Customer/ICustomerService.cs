﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Services;
using SocialPay.Core.Services.Authentication;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Core.Repositories.Customer
{
    public class ICustomerService : BaseService<MerchantPaymentSetup>
    {
        private readonly SocialPayDbContext _context;
        private readonly AuthRepoService _authRepoService;
        private readonly AppSettings _appSettings;
        public ICustomerService(SocialPayDbContext context, AuthRepoService authRepoService,
            IOptions<AppSettings> appSettings) : base(context)
        {
            _context = context;
            _authRepoService = authRepoService;
            _appSettings = appSettings.Value;
        }

        public async Task<MerchantPaymentSetup> GetTransactionReference(string refId)
        {
            return await _context.MerchantPaymentSetup.Include(x => x.CustomerTransaction).SingleOrDefaultAsync(p => p.TransactionReference
              == refId
            );
        }

        public async Task<List<CustomerTransaction>> GetTransactionByClientId(long clientId)
        {
            return await _context.CustomerTransaction
                .Where(x => x.ClientAuthenticationId == clientId).ToListAsync();
        }

        public async Task<WebApiResponse> GetCustomerPaymentsByMerchantPayRef(long clientId)
        {
            var result = new List<CustomerPaymentViewModel>();
            ////var getPaymentSetupInfo = await _context.MerchantPaymentSetup.Include(x=>x.CustomerTransaction)
            ////    .SingleOrDefaultAsync(x => x.TransactionReference == tranId);

            var getPaymentSetupInfo = await _context.MerchantPaymentSetup
                .Where(x => x.ClientAuthenticationId == clientId).ToListAsync();

            if (getPaymentSetupInfo.Count == 0)
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };

            var response =  (from c in getPaymentSetupInfo
                             join p in _context.CustomerTransaction on c.MerchantPaymentSetupId  equals p.MerchantPaymentSetupId
                         join a in _context.ClientAuthentication on c.ClientAuthenticationId equals a.ClientAuthenticationId
                         select new CustomerPaymentViewModel { Amount = c.Amount, CustomerEmail = a.Email,
                         TotalAmount = c.TotalAmount, CustomerPhoneNumber = a.PhoneNumber, TransactionDate = p.TransactionDate,
                         ShippingFee = c.ShippingFee, DeliveryMethod = c.DeliveryMethod,
                         DeliveryTime = c.DeliveryTime, Description = c.Description,
                         TransactionReference = c.TransactionReference}).ToList();
            result = response;
            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
        }


        public async Task <List<CustomerTransaction>> GetCustomerPaymentsByMerchantId(long merchantId)
        {
            return await _context.CustomerTransaction.Where(p => p.MerchantPaymentSetupId
            == merchantId).ToListAsync();
        }

        public async Task<WebApiResponse> GetClientDetails(string email)
        {
            var getClientInfo = await _authRepoService.GetClientDetails(email);
            if (getClientInfo == null)
                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };
            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = getClientInfo.ClientAuthenticationId };
        }

        public async Task<WebApiResponse> CreateNewCustomer(string email, string fullname, string phoneNumber)
        {
            var createCustomer = await _authRepoService.CreateAccount(email, null, fullname,  phoneNumber);
            if (createCustomer == null)
                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };
            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = createCustomer.Data };
        }


        public async Task<WebApiResponse> PaymentValidation(long clientId, string transactionReference, string message, string status)
        {
            try
            {
                var validateClient = await _authRepoService.GetClientDetailsByClientId(clientId);
                if (validateClient == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                var getPaymentLinkInfo = await GetTransactionReference(transactionReference);
                if (getPaymentLinkInfo == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                var logCustomerPayment = new CustomerTransaction
                {
                    CustomerEmail = validateClient.Email,
                    MerchantPaymentSetupId = getPaymentLinkInfo.MerchantPaymentSetupId,
                    Message = message,
                    OrderStatus = status
                };
                await _context.CustomerTransaction.AddAsync(logCustomerPayment);
                await _context.SaveChangesAsync();
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
           
        }

        public async Task<PaymentLinkViewModel> GetTransactionDetails(string refId)
        {
            var paymentview = new PaymentLinkViewModel();
            var validateReference = await GetTransactionReference(refId);
            if (validateReference == null)
                return new PaymentLinkViewModel { };
            var config = new MapperConfiguration(cfg => cfg.CreateMap<MerchantPaymentSetup, PaymentLinkViewModel>());
            var mapper = config.CreateMapper();
            paymentview = mapper.Map<PaymentLinkViewModel>(validateReference);
            return paymentview;
        }

        public async Task<WebApiResponse> GetCustomerOrders(long clientId)
        {
            try
            {
                var getCustomerOrders = await GetTransactionByClientId(clientId);
                if (getCustomerOrders == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                var response = (from c in getCustomerOrders
                                join m in _context.MerchantPaymentSetup on c.MerchantPaymentSetupId equals m.MerchantPaymentSetupId
                                select new OrdersViewModel { Amount = m.Amount, DeliveryTime = c.DeliveryDate, 
                                ShippingFee = m.ShippingFee, TransactionReference = m.TransactionReference,
                                DeliveryMethod = m.DeliveryMethod, Description = m.Description,
                                TotalAmount = m.TotalAmount, PaymentCategory = m.PaymentCategory,
                                OrderStatus = c.OrderStatus}).ToList();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = response };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
          
        }

        public async Task<List<MerchantPaymentSetup>> GetAllPaymentLinksByClientId(long clientId)
        {
            return await _context.MerchantPaymentSetup.Where(x => x.IsDeleted
            == false && x.ClientAuthenticationId == clientId).ToListAsync();
        }

        public async Task <List<PaymentLinkViewModel>> GetPaymentLinks(long clientId)
        {
            var paymentview = new List<PaymentLinkViewModel>();
            var validateReference = await GetAllPaymentLinksByClientId(clientId);
            if (validateReference == null)
                return paymentview;
            var config = new MapperConfiguration(cfg => cfg.CreateMap<MerchantPaymentSetup, PaymentLinkViewModel>());
            var mapper = config.CreateMapper();
            paymentview = mapper.Map<List<PaymentLinkViewModel>>(validateReference);
            return paymentview;
        }

        public async Task<WebApiResponse> LogPaymentResponse(PaymentValidationRequestDto model)
        {
            try
            {
                var customerInfo = await _context.ClientAuthentication
                    .SingleOrDefaultAsync(x => x.ClientAuthenticationId == model.CustomerId);

                var paymentSetupInfo = await _context.MerchantPaymentSetup
                   .SingleOrDefaultAsync(x => x.TransactionReference == model.TransactionReference);
                var logRequest = new CustomerTransaction
                {
                    ClientAuthenticationId = model.CustomerId, CustomerEmail = customerInfo.Email,
                    Message = model.Message, OrderStatus = OrderStatusCode.Pending, MerchantPaymentSetupId = paymentSetupInfo.MerchantPaymentSetupId,
                    DeliveryDate = DateTime.Now.AddDays(paymentSetupInfo.DeliveryTime)
                };

                await _context.CustomerTransaction.AddAsync(logRequest);
                await _context.SaveChangesAsync();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> ValidateShippingRequest(AcceptRejectRequestDto model, long clientId)
        {
            try
            {
                var validateOrder = await _context.MerchantPaymentSetup
                    .SingleOrDefaultAsync(x => x.TransactionReference == model.TransactionReference);
                if(validateOrder == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                var response = await _context.CustomerTransaction
                    .SingleOrDefaultAsync(x => x.CustomerTransactionId == model.RequestId);

                int sla = Convert.ToInt32(_appSettings.deliverySLA);

                if(response.DeliveryDate.AddDays(sla) < DateTime.Now && model.Status == OrderStatusCode.Decline)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.CancelHasExpired };

                var logRequest = new ItemAcceptedOrRejected
                {
                    ClientAuthenticationId = clientId, Status = model.Status, Comment = model.Comment,
                    TransactionReference = model.TransactionReference, CustomerTransactionId = model.RequestId
                };
                await _context.ItemAcceptedOrRejected.AddAsync(logRequest);
                await _context.SaveChangesAsync();
                //var respose = from c in _context.CustomerTransaction
                //              join m in _context.MerchantPaymentSetup on c.MerchantPaymentSetupId equals m.MerchantPaymentSetupId
                //              where c.CustomerTransactionId == model.RequestId
                //where m.TransactionReference == model.TransactionReference


                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
