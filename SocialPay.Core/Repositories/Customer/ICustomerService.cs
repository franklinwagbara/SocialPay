using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Messaging;
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
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Repositories.Customer
{
    public class ICustomerService : BaseService<MerchantPaymentSetup>
    {
        private readonly SocialPayDbContext _context;
        private readonly AuthRepoService _authRepoService;
        private readonly AppSettings _appSettings;
        private readonly EmailService _emailService;
        public ICustomerService(SocialPayDbContext context, AuthRepoService authRepoService,
            IOptions<AppSettings> appSettings, EmailService emailService) : base(context)
        {
            _context = context;
            _authRepoService = authRepoService;
            _appSettings = appSettings.Value;
            _emailService = emailService;
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
                         select new CustomerPaymentViewModel { MerchantAmount = c.MerchantAmount, CustomerEmail = a.Email,
                         TotalAmount = c.TotalAmount, CustomerPhoneNumber = a.PhoneNumber, TransactionDate = p.TransactionDate,
                         ShippingFee = c.ShippingFee, DeliveryMethod = c.DeliveryMethod, CustomerAmount = c.CustomerAmount, 
                         DeliveryTime = c.DeliveryTime, Description = c.MerchantDescription, CustomerDescription = c.CustomerDescription,
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

        public async Task<WebApiResponse> CreateNewCustomer(string email, string password, string fullname, string phoneNumber)
        {
            var createCustomer = await _authRepoService.CreateAccount(email, password, fullname,  phoneNumber);
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
                                select new OrdersViewModel { MerchantAmount = m.MerchantAmount, DeliveryTime = c.DeliveryDate, 
                                ShippingFee = m.ShippingFee, TransactionReference = m.TransactionReference,
                                DeliveryMethod = m.DeliveryMethod, Description = m.MerchantDescription,
                                TotalAmount = m.TotalAmount, PaymentCategory = m.PaymentCategory,
                                OrderStatus = c.OrderStatus, RequestId = c.CustomerTransactionId}).ToList();

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
                if (model.Message.Contains("Approve"))
                {
                    logRequest.Status = true;
                }
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

                var logRequest = new ItemAcceptedOrRejected
                {
                    ClientAuthenticationId = clientId,
                    Status = model.Status,
                    Comment = model.Comment,
                    TransactionReference = model.TransactionReference,
                    CustomerTransactionId = model.RequestId
                };
                if (model.Status == OrderStatusCode.Decline)
                {
                    var getMerchant = await _context.ClientAuthentication
                        .SingleOrDefaultAsync(x => x.ClientAuthenticationId == validateOrder.ClientAuthenticationId);
                    if (response.DeliveryDate.AddDays(sla) < DateTime.Now)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.CancelHasExpired };

                   
                    await _context.ItemAcceptedOrRejected.AddAsync(logRequest);
                    await _context.SaveChangesAsync();
                    var emailModal = new EmailRequestDto
                    {
                        Subject = "Order" + " "+ model.TransactionReference + " "+ "was Rejected",
                        SourceEmail = "info@sterling.ng",
                        DestinationEmail = getMerchant.Email,
                        // DestinationEmail = "festypat9@gmail.com",
                        //  EmailBody = "Your onboarding was successfully created. Kindly use your email as username and" + "   " + "" + "   " + "as password to login"
                    };
                    var mailBuilder = new StringBuilder();
                    mailBuilder.AppendLine("Dear" + " " + getMerchant.Email + "," + "<br />");
                    mailBuilder.AppendLine("<br />");
                    mailBuilder.AppendLine("An order has been rejected by" + ""+ response.CustomerEmail +" "+ ".<br />");
                    //mailBuilder.AppendLine("Kindly use this token" + "  " + newPin + "  " + "and" + " " + urlPath + "<br />");
                    // mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                    mailBuilder.AppendLine("Best Regards,");
                    emailModal.EmailBody = mailBuilder.ToString();

                    var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }

                await _context.ItemAcceptedOrRejected.AddAsync(logRequest);
                await _context.SaveChangesAsync();
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
