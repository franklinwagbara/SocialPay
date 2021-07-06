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
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(TransactionService));

        public TransactionService(SocialPayDbContext context)
        {
            _context = context;
        }

        public async Task<WebApiResponse> GetCustomerOrders(string category)
        {
            _log4net.Info("Task start to GetCustomerOrders" + " - " + category + " - " + DateTime.Now);
            var request = new List<OrdersViewModel>();
            try
            {
                var getCustomerOrders = await _context.TransactionLog.ToListAsync();
                if (getCustomerOrders == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                if (category == MerchantPaymentLinkCategory.InvoiceLink)
                {
                    var invoiceResponse = (from c in getCustomerOrders
                                           join m in _context.InvoicePaymentLink on c.TransactionReference equals m.TransactionReference
                                           select new OrdersViewModel
                                           {
                                               MerchantAmount = m.UnitPrice,
                                               DeliveryTime = c.DeliveryDate,
                                               ShippingFee = m.ShippingFee,
                                               TransactionReference = m.TransactionReference,
                                               MerchantDescription = m.Description,
                                               ClientId = c.ClientAuthenticationId,
                                               CustomerTransactionReference = c.CustomerTransactionReference,
                                               TotalAmount = c.TotalAmount,
                                               PaymentCategory = category,
                                               TransactionDate = Convert.ToString(c.TransactionDate),
                                               PaymentMethod = c.PaymentChannel,
                                               PaymentReference = c.PaymentReference,
                                               OrderStatus = c.OrderStatus,
                                               RequestId = c.TransactionLogId
                                           })
                               .OrderByDescending(x => x.TransactionDate).ToList();

                    request = invoiceResponse;
                   
                    _log4net.Info("Response for GetCustomerOrders" + " - " + category + " - " + request.Count + " - " + DateTime.Now);
                    
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request };
                }

                var otherLinksresponse = (from c in getCustomerOrders
                                          join m in _context.MerchantPaymentSetup on c.TransactionReference equals m.TransactionReference
                                          join a in _context.MerchantBusinessInfo on m.ClientAuthenticationId equals a.ClientAuthenticationId
                                          join b in _context.CustomerOtherPaymentsInfo on c.PaymentReference equals b.PaymentReference
                                          select new OrdersViewModel
                                          {
                                              MerchantAmount = m.MerchantAmount,
                                              DeliveryTime = c.DeliveryDate,
                                              ShippingFee = m.ShippingFee,
                                              TransactionReference = m.TransactionReference,
                                              DeliveryMethod = m.DeliveryMethod,
                                              MerchantDescription = m.MerchantDescription,
                                              TotalAmount = c.TotalAmount,
                                              PaymentCategory = m.PaymentCategory,
                                              ClientId = c.ClientAuthenticationId,
                                              CustomerTransactionReference = c.CustomerTransactionReference,
                                              MerchantName = a.BusinessName,
                                              CustomerName = b.Fullname,
                                              PaymentReference = c.PaymentReference,
                                              TransactionStatus = c.TransactionJourney,
                                              TransactionDate = Convert.ToString(c.TransactionDate),
                                              PaymentMethod = c.PaymentChannel,
                                              OrderStatus = c.OrderStatus,
                                              RequestId = c.TransactionLogId
                                          })
                                .OrderByDescending(x => x.TransactionDate).ToList();

                request = otherLinksresponse;

                _log4net.Info("Response for GetCustomerOrders" + " - " + category + " - " + request.Count + " - " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request };
            }
            catch (Exception ex)
            {
                _log4net.Error("An error occured while trying to initiateGetCustomerOrders" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }

        public async Task<WebApiResponse> GetOnboardingJourney()
        {
            var request = new List<UserJourneyViewModel>();
            try
            {
                var getUsers = await _context.ClientAuthentication.Where(x => x.RoleName != "Guest").ToListAsync();

                var response = (from c in getUsers
                                    //join b in _context.MerchantBusinessInfo on c.ClientAuthenticationId equals b.ClientAuthenticationId
                                select new UserJourneyViewModel
                                {
                                    Email = c.Email,
                                    Status = c.StatusCode,
                                    PhoneNumber = c.PhoneNumber,
                                    FullName = c.FullName,
                                    DateEntered = c.DateEntered,
                                    LastDateModified = c.LastDateModified
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
                if (validateUser != null)
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
