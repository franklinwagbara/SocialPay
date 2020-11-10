using Microsoft.EntityFrameworkCore;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace SocialPay.Core.Services.Report
{
    public class TransactionService
    {
        private readonly SocialPayDbContext _context;
        public TransactionService(SocialPayDbContext context)
        {
            _context = context;
        }

        public async Task<WebApiResponse> GetCustomerOrders(string category)
        {
            var request = new List<OrdersViewModel>();
            try
            {
                var getCustomerOrders = await _context.TransactionLog.ToListAsync();
                if (getCustomerOrders == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                if(category == MerchantPaymentLinkCategory.InvoiceLink )
                {
                     var invoiceResponse = (from c in getCustomerOrders
                                join m in _context.InvoicePaymentLink on c.TransactionReference equals m.TransactionReference
                                select new OrdersViewModel { MerchantAmount = m.UnitPrice, DeliveryTime = c.DeliveryDate, 
                                ShippingFee = m.ShippingFee, TransactionReference = m.TransactionReference,
                                 Description = m.Description, ClientId = c.ClientAuthenticationId, CustomerTransactionReference = c.CustomerTransactionReference,
                                TotalAmount = m.TotalAmount, PaymentCategory = category,
                                OrderStatus = c.OrderStatus, RequestId = c.TransactionLogId}).ToList();
                    request = invoiceResponse;
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request };
                }

                var otherLinksresponse = (from c in getCustomerOrders
                                join m in _context.MerchantPaymentSetup on c.TransactionReference equals m.TransactionReference
                                select new OrdersViewModel { MerchantAmount = m.MerchantAmount, DeliveryTime = c.DeliveryDate, 
                                ShippingFee = m.ShippingFee, TransactionReference = m.TransactionReference,
                                DeliveryMethod = m.DeliveryMethod, Description = m.MerchantDescription,
                                TotalAmount = m.TotalAmount, PaymentCategory = m.PaymentCategory, ClientId = c.ClientAuthenticationId,
                                CustomerTransactionReference = c.CustomerTransactionReference,
                                OrderStatus = c.OrderStatus, RequestId = c.TransactionLogId}).ToList();
                request = otherLinksresponse;
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
          
        }


        public async Task<WebApiResponse> GetOnboardingJourney()
        {
            var request = new List<UserJourneyViewModel>();
            try
            {
                var getUsers = await _context.ClientAuthentication.Where(x=>x.RoleName !="Guest").ToListAsync();

                var response = (from c in getUsers
                                //join b in _context.MerchantBusinessInfo on c.ClientAuthenticationId equals b.ClientAuthenticationId
                                select new UserJourneyViewModel
                                {
                                    Email = c.Email, Status = c.StatusCode, PhoneNumber = c.PhoneNumber,
                                    FullName = c.FullName
                                }).ToList();
                request = response;
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
          
        }


        public async Task<WebApiResponse> ClearUserAccount(string email)
        {
            try
            {
                var validateUser = await _context.ClientAuthentication
                    .SingleOrDefaultAsync(x => x.Email == email);
                if(validateUser !=null)
                {
                    _context.Remove(validateUser);
                    await _context.SaveChangesAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
