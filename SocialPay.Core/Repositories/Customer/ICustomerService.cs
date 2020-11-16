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
        private readonly TransactionReceipt _transactionReceipt;
        public ICustomerService(SocialPayDbContext context, AuthRepoService authRepoService,
            IOptions<AppSettings> appSettings, EmailService emailService,
            TransactionReceipt transactionReceipt) : base(context)
        {
            _context = context;
            _authRepoService = authRepoService;
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _transactionReceipt = transactionReceipt;
        }

        public async Task<MerchantPaymentSetup> GetTransactionReference(string refId)
        {
            return await _context.MerchantPaymentSetup
                .Include(x => x.CustomerTransaction)
                .SingleOrDefaultAsync(p => p.TransactionReference
              == refId
            );
        }


        public async Task<InvoicePaymentLink> GetInvoicePaymentAsync(string refId)
        {
            return await _context.InvoicePaymentLink
                .SingleOrDefaultAsync(p => p.TransactionReference
              == refId
            );
        }

        public async Task<InvoicePaymentInfo> GetInvoicePaymentInfo(string refId, string paymentReference)
        {
            return await _context.InvoicePaymentInfo
                .SingleOrDefaultAsync(p => p.TransactionReference
              == refId && p.PaymentReference == paymentReference
            );
        }

        public async Task<TransactionLog> GetTransactionLogsByReference(string refId)
        {
            return await _context.TransactionLog
                .SingleOrDefaultAsync(p => p.CustomerTransactionReference
              == refId
            );
        }

        public async Task<WebApiResponse> GetMerchantPaymentInfo(string transactionReference)
        {
            var validateReference = await GetTransactionReference(transactionReference);
            if(validateReference == null)
                return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidPaymentReference};

            var getMerchantInfo = await GetMerchantInfo(validateReference.ClientAuthenticationId);
            if(getMerchantInfo == null)
                return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidPaymentReference };




            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = validateReference };
        }

        public async Task<List<TransactionLog>> GetTransactionByClientId(long clientId)
        {
            return await _context.TransactionLog
                .Where(x => x.CustomerInfo == clientId
               ).ToListAsync();
        }


        public async Task<MerchantBusinessInfo> GetMerchantInfo(long clientId)
        {
            return await _context.MerchantBusinessInfo.SingleOrDefaultAsync(p => p.ClientAuthenticationId
             == clientId
           );
        }

        public async Task<LinkCategory> GetLinkCategorybyTranref(string tranRef)
        {
            return await _context.LinkCategory.SingleOrDefaultAsync(p => p.TransactionReference
             == tranRef
           );
        }

        public async Task<TransactionLog> GetTransactionLogAsync(string tranRef, string customerRef)
        {
            return await _context.TransactionLog.SingleOrDefaultAsync(p => p.TransactionReference
             == tranRef && p.CustomerTransactionReference == customerRef
           );
        }
        public async Task<WebApiResponse> GetCustomerPaymentsByMerchantPayRef(long clientId)
        {
            try
            {
                 var result = new List<CustomerPaymentViewModel>();
           
            var getPaymentSetupInfo = await _context.MerchantPaymentSetup
                .Where(x => x.ClientAuthenticationId == clientId).ToListAsync();

            if (getPaymentSetupInfo.Count == 0)
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };

            var response =  (from c in getPaymentSetupInfo
                             join p in _context.TransactionLog on c.TransactionReference  equals p.TransactionReference
                         join a in _context.CustomerOtherPaymentsInfo on p.PaymentReference equals a.PaymentReference
                         select new CustomerPaymentViewModel { MerchantAmount = c.MerchantAmount, CustomerEmail = a.Email,
                         TotalAmount = a.Amount, CustomerPhoneNumber = a.PhoneNumber, TransactionDate = p.TransactionDate,
                         ShippingFee = c.ShippingFee, DeliveryMethod = c.DeliveryMethod, CustomerAmount = c.CustomerAmount, 
                         DeliveryTime = c.DeliveryTime, MerchantDescription = c.MerchantDescription, CustomerDescription = a.CustomerDescription,
                         TransactionReference = c.TransactionReference, Fullname = a.Fullname,
                         Document = a.Document == null ? string.Empty : _appSettings.BaseApiUrl + a.FileLocation + "/" + a.Document,
                         CustomerTransactionReference = p.CustomerTransactionReference}).OrderByDescending(x=>x.TransactionDate).ToList();
            result = response;
            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }



        public async Task<WebApiResponse> GetCustomerByMerchantId(long clientId)
        {
            var result = new List<CustomerInfoViewModel>();
          
            var getPaymentSetupInfo = await _context.CustomerOtherPaymentsInfo
                .Where(x => x.ClientAuthenticationId == clientId).ToListAsync();

            if (getPaymentSetupInfo.Count == 0)
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };

            var response =  (from a in getPaymentSetupInfo                             
                         join p in _context.TransactionLog on a.PaymentReference equals p.PaymentReference
                         select new CustomerInfoViewModel { CustomerEmail = a.Email, Fullname = a.Fullname,
                             CustomerPhoneNumber = a.PhoneNumber, DateRegistered = a.DateEntered, 
                             ClientAuthenticationId = p.CustomerInfo}).OrderByDescending(x=>x.DateRegistered).ToList();
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

        public async Task<dynamic> GetTransactionDetails(string refId)
        {
            var paymentview = new PaymentLinkViewModel();

            var validateLink = await GetLinkCategorybyTranref(refId);
            if (validateLink == null)
                return new PaymentLinkViewModel { };
            var getMerchantInfo = await GetMerchantInfo(validateLink.ClientAuthenticationId);
            if (getMerchantInfo == null)
                return new PaymentLinkViewModel { };
            if (validateLink.Channel == MerchantPaymentLinkCategory.InvoiceLink)
            {
                return await GetInvoiceTransactionDetails(refId);
            }
            var validateReference = await GetTransactionReference(refId);
            if (validateReference == null)
                return new PaymentLinkViewModel { };          
            var config = new MapperConfiguration(cfg => cfg.CreateMap<MerchantPaymentSetup, PaymentLinkViewModel>());
            var mapper = config.CreateMapper();
            paymentview = mapper.Map<PaymentLinkViewModel>(validateReference);
            paymentview.MerchantDocument = validateReference == null ? string.Empty : _appSettings.BaseApiUrl + validateReference.FileLocation + "/" + validateReference.Document;
            paymentview.MerchantInfo = new MerchantInfoViewModel
            {
                BusinessEmail = getMerchantInfo.BusinessEmail,
                BusinessPhoneNumber = getMerchantInfo.BusinessPhoneNumber,
                BusinessName = getMerchantInfo.BusinessName,
                Chargebackemail = getMerchantInfo.Chargebackemail,
                Country = getMerchantInfo.Country,
                Logo = getMerchantInfo == null ? string.Empty : _appSettings.BaseApiUrl + getMerchantInfo.FileLocation + "/" + getMerchantInfo.Logo
            };
            //ProfilePhoto = getInvestors == null ? string.Empty : _appSettings.BaseApiUrl + getInvestors.IndividualInvestor.Select(x => x.FileLocation).First() + "/" + getInvestors.IndividualInvestor.Select(x => x.ProfilePhoto).First(),

            return paymentview;
        }


        public async Task<InvoiceViewModel> GetInvoiceTransactionDetails(string refId)
        {
            var paymentview = new InvoiceViewModel();

            var validateLink = await GetLinkCategorybyTranref(refId);
            if (validateLink == null)
                return new InvoiceViewModel { };
            var getMerchantInfo = await GetMerchantInfo(validateLink.ClientAuthenticationId);           
            var validateReference = await GetInvoicePaymentAsync(refId);
            if (validateReference == null)
                return new InvoiceViewModel { };         
            var config = new MapperConfiguration(cfg => cfg.CreateMap<InvoicePaymentLink, InvoiceViewModel>());
            var mapper = config.CreateMapper();
            paymentview = mapper.Map<InvoiceViewModel>(validateReference);
            //paymentview.MerchantDocument = validateReference == null ? string.Empty : _appSettings.BaseApiUrl + validateReference.FileLocation + "/" + validateReference.Document;
            paymentview.MerchantInfo = new MerchantInfoViewModel
            {
                BusinessEmail = getMerchantInfo.BusinessEmail,
                BusinessPhoneNumber = getMerchantInfo.BusinessPhoneNumber,
                BusinessName = getMerchantInfo.BusinessName,
                Chargebackemail = getMerchantInfo.Chargebackemail,
                Country = getMerchantInfo.Country,
                Logo = getMerchantInfo == null ? string.Empty : _appSettings.BaseApiUrl + getMerchantInfo.FileLocation + "/" + getMerchantInfo.Logo
            };

            return paymentview;
        }


        public async Task<WebApiResponse> GetCustomerOrders(long clientId, string category)
        {
            var request = new List<OrdersViewModel>();
            try
            {
                var getCustomerOrders = await GetTransactionByClientId(clientId);
                if (getCustomerOrders == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                if(category == MerchantPaymentLinkCategory.InvoiceLink )
                {
                     var invoiceResponse = (from c in getCustomerOrders
                                join m in _context.InvoicePaymentLink on c.TransactionReference equals m.TransactionReference
                                select new OrdersViewModel { MerchantAmount = m.UnitPrice, DeliveryTime = c.DeliveryDate, 
                                ShippingFee = m.ShippingFee, TransactionReference = m.TransactionReference,
                                 MerchantDescription = m.Description, ClientId = clientId, CustomerTransactionReference = c.CustomerTransactionReference,
                                TotalAmount = m.TotalAmount, PaymentCategory = category,
                                OrderStatus = c.OrderStatus, RequestId = c.TransactionLogId}).ToList();
                    request = invoiceResponse;
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request };
                }

                var otherLinksresponse = (from c in getCustomerOrders
                                join m in _context.MerchantPaymentSetup on c.TransactionReference equals m.TransactionReference
                                join v in _context.CustomerOtherPaymentsInfo on c.PaymentReference equals v.PaymentReference
                                select new OrdersViewModel { MerchantAmount = m.MerchantAmount, DeliveryTime = c.DeliveryDate, 
                                ShippingFee = m.ShippingFee, TransactionReference = m.TransactionReference,
                                DeliveryMethod = m.DeliveryMethod, MerchantDescription = m.MerchantDescription,
                                TransactionStatus = c.TransactionStatus, CustomerDescription = v.CustomerDescription,
                                TotalAmount = v.Amount, PaymentCategory = m.PaymentCategory, ClientId = clientId,
                                CustomerTransactionReference = c.CustomerTransactionReference, PaymentReference = v.PaymentReference,
                                OrderStatus = c.OrderStatus, RequestId = c.TransactionLogId}).ToList();
                request = otherLinksresponse;
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request };
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



        public async Task<WebApiResponse> LogInvoicePaymentResponse(PaymentValidationRequestDto model)
        {
            try
            {
                var linkInfo = await GetLinkCategorybyTranref(model.TransactionReference);
                var logFailedResponse = new FailedTransactions();
                if (linkInfo != null & linkInfo.Channel == MerchantPaymentLinkCategory.InvoiceLink)
                {
                    var getpaymentInfo = await GetInvoicePaymentInfo(model.TransactionReference, model.InvoiceReference);
                    if (getpaymentInfo == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidTransactionReference };
                    if(getpaymentInfo.TransactionStatus == OrderStatusCode.Approved)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                    if (model.Message.Contains("approve") || model.Message.Contains("success") || model.Message.Contains("Approve"))
                    {
                        using(var transaction = await _context.Database.BeginTransactionAsync())
                        {
                            try
                            {
                                var logconfirmation = new TransactionLog { };
                                logconfirmation.Category = linkInfo.Channel;
                                logconfirmation.CustomerEmail = getpaymentInfo.Email;
                                logconfirmation.CustomerTransactionReference = getpaymentInfo.CustomerTransactionReference;
                                logconfirmation.TransactionReference = model.TransactionReference;
                                logconfirmation.OrderStatus = OrderStatusCode.Pending;
                                logconfirmation.Message = model.Message;
                                logconfirmation.LastDateModified = DateTime.Now;
                                logconfirmation.CustomerInfo = model.CustomerId;
                                logconfirmation.Status = true;                               
                                logconfirmation.PaymentReference = model.InvoiceReference;
                                logconfirmation.PaymentChannel = model.Channel;
                                logconfirmation.TransactionStatus = OrderStatusCode.Approved;

                                var merchantInfo = await GetMerchantInfo(linkInfo.ClientAuthenticationId);
                                var invoiceInfo = await GetInvoicePaymentAsync(model.TransactionReference);
                                logconfirmation.TotalAmount = invoiceInfo.TotalAmount;
                                logconfirmation.ClientAuthenticationId = invoiceInfo.ClientAuthenticationId;
                                getpaymentInfo.TransactionStatus = OrderStatusCode.Approved;
                                getpaymentInfo.Status = true;
                                getpaymentInfo.Message = model.Message;
                                getpaymentInfo.LastDateModified = DateTime.Now;
                                logconfirmation.DeliveryDayTransferStatus = OrderStatusCode.Pending;
                                _context.Update(getpaymentInfo);
                                await _context.SaveChangesAsync();
                                await _context.TransactionLog.AddAsync(logconfirmation);
                                await _context.SaveChangesAsync();
                                await transaction.CommitAsync();
                                await _transactionReceipt.ReceiptTemplate(getpaymentInfo.Email, invoiceInfo.TotalAmount,
                                  DateTime.Now, model.TransactionReference, merchantInfo == null ? string.Empty : merchantInfo.BusinessName);
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                            }
                            catch (Exception ex)
                            {
                                await transaction.RollbackAsync();
                                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                            }
                        }
                      
                     }
                    using(var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {

                            logFailedResponse.CustomerTransactionReference = getpaymentInfo.CustomerTransactionReference;
                            logFailedResponse.TransactionReference = getpaymentInfo.TransactionReference;
                            logFailedResponse.Message = model.Message;                            
                            await _context.FailedTransactions.AddAsync(logFailedResponse);
                            await _context.SaveChangesAsync();
                            getpaymentInfo.TransactionStatus = OrderStatusCode.Failed;
                            getpaymentInfo.Status = false;
                            getpaymentInfo.Message = model.Message;
                            getpaymentInfo.LastDateModified = DateTime.Now;
                            _context.Update(getpaymentInfo);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                            return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionFailed };
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                        }
                    }
                 
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionFailed };
                ////var customerInfo = await _context.ClientAuthentication
                ////    .SingleOrDefaultAsync(x => x.ClientAuthenticationId == model.CustomerId);

                ////var merhantInfo = await GetMerchantInfo(customerInfo.ClientAuthenticationId);

                //var getUserInfo = await GetInvoicePaymentAsync(model.TransactionReference);


                //var logRequest = new CustomerTransaction { };
                //// var logconfirmation = new TransactionLog { };
                //// logconfirmation.Category = linkInfo.Channel;
                //// logconfirmation.ClientAuthenticationId = model.CustomerId;
                //// logconfirmation.CustomerEmail = customerInfo.Email;
                //// logconfirmation.CustomerTransactionReference = Guid.NewGuid().ToString();
                //// logconfirmation.TransactionReference = model.TransactionReference;
                //// logconfirmation.OrderStatus = OrderStatusCode.Pending;
                //// logconfirmation.Message = model.Message;

                //// var paymentSetupInfo = await _context.MerchantPaymentSetup
                ////.SingleOrDefaultAsync(x => x.TransactionReference == model.TransactionReference);


                //// if (model.Message.Contains("Approve") || model.Message.Contains("Success"))
                //// {
                ////     logconfirmation.Status = true;
                ////     using (var transaction = await _context.Database.BeginTransactionAsync())
                ////     {
                ////         try
                ////         {
                ////             logconfirmation.DeliveryDate = DateTime.Now.AddDays(paymentSetupInfo.DeliveryTime);
                ////             await _context.TransactionLog.AddAsync(logconfirmation);
                ////             await _context.SaveChangesAsync();
                ////             await transaction.CommitAsync();
                ////             //Send mail
                ////             await _transactionReceipt.ReceiptTemplate(logconfirmation.CustomerEmail, paymentSetupInfo.TotalAmount,
                ////                 logconfirmation.TransactionDate, model.TransactionReference, merhantInfo == null ? string.Empty : merhantInfo.BusinessName);
                ////             return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                ////         }
                ////         catch (Exception ex)
                ////         {
                ////             await transaction.RollbackAsync();
                ////             return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                ////         }
                ////     }
                //// }

                //// logFailedResponse.CustomerTransactionReference = "";
                //// logFailedResponse.TransactionReference = paymentSetupInfo.TransactionReference;
                //// logFailedResponse.Message = model.Message;
                //// await _context.FailedTransactions.AddAsync(logFailedResponse);
                //// await _context.SaveChangesAsync();
                //// return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionFailed };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> LogPaymentResponse(PaymentValidationRequestDto model)
        {
            try
            {
               

                var logconfirmation = new TransactionLog { };
                var linkInfo = await GetLinkCategorybyTranref(model.TransactionReference);
                var paymentSetupInfo = await _context.MerchantPaymentSetup
               .SingleOrDefaultAsync(x => x.TransactionReference == model.TransactionReference);
                if (paymentSetupInfo == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };
                var merchantInfo = await GetMerchantInfo(paymentSetupInfo.ClientAuthenticationId);
                if (linkInfo != null && linkInfo.Channel == MerchantPaymentLinkCategory.Escrow || linkInfo.Channel == MerchantPaymentLinkCategory.OneOffEscrowLink)
                {
                    var customerInfo = await _context.ClientAuthentication
                    .SingleOrDefaultAsync(x => x.ClientAuthenticationId == model.CustomerId);
                    logconfirmation.Category = linkInfo.Channel;
                    logconfirmation.PaymentChannel = model.Channel;
                    logconfirmation.ClientAuthenticationId = paymentSetupInfo.ClientAuthenticationId;
                    logconfirmation.CustomerInfo = model.CustomerId;
                    logconfirmation.CustomerEmail = customerInfo.Email;
                    logconfirmation.CustomerTransactionReference = Guid.NewGuid().ToString();
                    logconfirmation.TransactionReference = model.TransactionReference;
                    logconfirmation.OrderStatus = OrderStatusCode.Pending;
                    logconfirmation.Message = model.Message;
                    logconfirmation.LastDateModified = DateTime.Now;
                    logconfirmation.TotalAmount = paymentSetupInfo.TotalAmount;
                    logconfirmation.DeliveryDayTransferStatus = OrderStatusCode.Pending;
                    logconfirmation.PaymentReference = model.PaymentReference;
                    logconfirmation.TransactionStatus = OrderStatusCode.Pending;

                    if (model.Message.Contains("approve") || model.Message.Contains("success") || model.Message.Contains("Approve"))
                    {
                        logconfirmation.Status = true;
                        logconfirmation.LastDateModified = DateTime.Now;
                        using (var transaction = await _context.Database.BeginTransactionAsync())
                        {
                            try
                            {
                                logconfirmation.DeliveryDate = DateTime.Now.AddDays(paymentSetupInfo.DeliveryTime);
                                logconfirmation.DeliveryFinalDate = logconfirmation.DeliveryDate.AddDays(2);
                                await _context.TransactionLog.AddAsync(logconfirmation);
                                await _context.SaveChangesAsync();
                                await transaction.CommitAsync();
                                //Send mail
                                await _transactionReceipt.ReceiptTemplate(logconfirmation.CustomerEmail, paymentSetupInfo.TotalAmount,
                                    logconfirmation.TransactionDate, model.TransactionReference, merchantInfo == null ? string.Empty : merchantInfo.BusinessName);
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                            }
                            catch (Exception ex)
                            {
                                await transaction.RollbackAsync();
                                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                            }
                        }
                    }

                    return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionFailed };
                }
              

                if (model.Message.Contains("approve") || model.Message.Contains("success") || model.Message.Contains("Approve"))
                {
                    logconfirmation.Category = linkInfo.Channel;
                    logconfirmation.PaymentChannel = model.Channel;
                    logconfirmation.ClientAuthenticationId = paymentSetupInfo.ClientAuthenticationId;
                    logconfirmation.CustomerInfo = model.CustomerId;
                    logconfirmation.CustomerTransactionReference = Guid.NewGuid().ToString();
                    logconfirmation.TransactionReference = model.TransactionReference;
                    logconfirmation.OrderStatus = OrderStatusCode.Pending;
                    logconfirmation.Message = model.Message;
                    logconfirmation.LastDateModified = DateTime.Now;
                    logconfirmation.TotalAmount = paymentSetupInfo.TotalAmount;
                    logconfirmation.DeliveryDayTransferStatus = OrderStatusCode.Pending;
                    logconfirmation.PaymentReference = model.PaymentReference;
                    logconfirmation.TransactionStatus = OrderStatusCode.Approved;
                   // logconfirmation.CustomerEmail = model.e;
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            logconfirmation.Status = true;
                            logconfirmation.DeliveryDate = DateTime.Now.AddDays(paymentSetupInfo.DeliveryTime);
                            logconfirmation.DeliveryFinalDate = logconfirmation.DeliveryDate.AddDays(2);
                            await _context.TransactionLog.AddAsync(logconfirmation);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                            //Send mail
                            await _transactionReceipt.ReceiptTemplate(logconfirmation.CustomerEmail, paymentSetupInfo.TotalAmount,
                                logconfirmation.TransactionDate, model.TransactionReference, merchantInfo == null ? string.Empty : merchantInfo.BusinessName);
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                        }
                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionFailed };
              
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> AcceptRejectOrderRequest(AcceptRejectRequestDto model, long clientId)
        {
            try
            {

                var getTransactionLogs = await _context.TransactionLog
                    .SingleOrDefaultAsync(x => x.PaymentReference == 
                    model.PaymentReference);
                var logRequest = new ItemAcceptedOrRejected();
                logRequest.ClientAuthenticationId = clientId;
                logRequest.Comment = model.Comment;
                logRequest.CustomerTransactionReference = getTransactionLogs.CustomerTransactionReference;
                logRequest.PaymentReference = model.PaymentReference;
                logRequest.ProcessedBy = model.ProcessedBy;
                logRequest.OrderStatus = model.Status;
                logRequest.TransactionReference = model.TransactionReference;
                if (model.ProcessedBy == AcceptRejectRequest.Merchant)
                {
                    using(var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            if (model.Status == OrderStatusCode.Decline)
                            {
                                if (getTransactionLogs.TransactionStatus == OrderStatusCode.Decline)
                                {
                                    logRequest.OrderStatus = OrderStatusCode.Dispute;
                                    await _context.ItemAcceptedOrRejected.AddAsync(logRequest);
                                    await _context.SaveChangesAsync();
                                    getTransactionLogs.TransactionStatus = OrderStatusCode.Dispute;
                                    getTransactionLogs.Status = true;
                                    getTransactionLogs.IsAccepted = true;
                                    getTransactionLogs.AcceptRejectLastDateModified = DateTime.Now;
                                    _context.Update(getTransactionLogs);
                                    await _context.SaveChangesAsync();
                                    await transaction.CommitAsync();
                                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                                }
                                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };
                            }

                            if (getTransactionLogs.TransactionStatus == OrderStatusCode.Decline)
                            {
                                logRequest.OrderStatus = OrderStatusCode.ItemAccepted;
                                await _context.ItemAcceptedOrRejected.AddAsync(logRequest);
                                await _context.SaveChangesAsync();
                                getTransactionLogs.TransactionStatus = OrderStatusCode.ItemAccepted;
                                getTransactionLogs.Status = true;
                                getTransactionLogs.IsAccepted = true;
                                getTransactionLogs.AcceptRejectLastDateModified = DateTime.Now;
                                _context.Update(getTransactionLogs);
                                await _context.SaveChangesAsync();
                                await transaction.CommitAsync();
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                            }
                            return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                        }
                    }
                  
                }

                if (getTransactionLogs != null && getTransactionLogs.TransactionStatus != OrderStatusCode.Pending)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionProcessed};

                if(getTransactionLogs.DeliveryDate < DateTime.Now)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.OrderHasExpired };                                

                //if (await _context.ItemAcceptedOrRejected
                //    .AnyAsync(x => x.CustomerTransactionReference == model.CustomerTransactionReference))
                //return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionAlreadyexit };
                if (model.Status == OrderStatusCode.Decline || model.Status == OrderStatusCode.Approved)
                {
                    // var validateOrder = await _context.MerchantPaymentSetup
                    //.SingleOrDefaultAsync(x => x.TransactionReference == model.TransactionReference);
                    // if (validateOrder == null)
                    //     return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                    // var response = await _context.CustomerTransaction
                    //     .SingleOrDefaultAsync(x => x.CustomerTransactionId == model.RequestId);

                    // int sla = Convert.ToInt32(_appSettings.deliverySLA);
                    logRequest.ClientAuthenticationId = clientId;
                    logRequest.Comment = model.Comment;
                    logRequest.CustomerTransactionReference = getTransactionLogs.CustomerTransactionReference;
                    logRequest.PaymentReference = model.PaymentReference;
                    logRequest.ProcessedBy = model.ProcessedBy;
                    logRequest.OrderStatus = model.Status;
                    logRequest.TransactionReference = model.TransactionReference;
                  
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            if (model.Status == OrderStatusCode.Decline)
                            {
                                var getMerchant = await _context.ClientAuthentication
                                    .SingleOrDefaultAsync(x => x.ClientAuthenticationId == getTransactionLogs.ClientAuthenticationId);
                                //if (response.DeliveryDate.AddDays(sla) < DateTime.Now)
                                //    return new WebApiResponse { ResponseCode = AppResponseCodes.OrderHasExpired };

                                logRequest.OrderStatus = OrderStatusCode.Decline;
                                logRequest.LastDateModified = DateTime.Now;
                                logRequest.ReturnedDate = DateTime.Now.AddDays(Convert.ToInt32(_appSettings.returnedDateSLA));
                                await _context.ItemAcceptedOrRejected.AddAsync(logRequest);
                                await _context.SaveChangesAsync();
                                getTransactionLogs.TransactionStatus = OrderStatusCode.Decline;
                                getTransactionLogs.Status = true;
                                getTransactionLogs.IsAccepted = false;
                                getTransactionLogs.AcceptRejectLastDateModified = DateTime.Now;
                                await _context.SaveChangesAsync();
                                ////var disputeModel = new DisputeRequestLog
                                ////{
                                ////    ItemAcceptedOrRejectedId = logRequest.ItemAcceptedOrRejectedId,
                                ////    DisputeComment = model.Comment, 
                                ////    ClientAuthenticationId = validateOrder.ClientAuthenticationId
                                ////};
                                ////await _context.ItemDispute.AddAsync(disputeModel);
                                ////await _context.SaveChangesAsync();
                                await transaction.CommitAsync();
                                var emailModal = new EmailRequestDto
                                {
                                    Subject = "Order" + " " + model.TransactionReference + " " + "was Rejected",
                                    SourceEmail = "info@sterling.ng",
                                    DestinationEmail = getMerchant.Email,
                                    // DestinationEmail = "festypat9@gmail.com",
                                    //  EmailBody = "Your onboarding was successfully created. Kindly use your email as username and" + "   " + "" + "   " + "as password to login"
                                };
                                var mailBuilder = new StringBuilder();
                                mailBuilder.AppendLine("Dear" + " " + getMerchant.Email + "," + "<br />");
                                mailBuilder.AppendLine("<br />");
                                mailBuilder.AppendLine("An order has been rejected by" + "" + getTransactionLogs.CustomerEmail + " " + ".<br />");
                                //mailBuilder.AppendLine("Kindly use this token" + "  " + newPin + "  " + "and" + " " + urlPath + "<br />");
                                // mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                                mailBuilder.AppendLine("Best Regards,");
                                emailModal.EmailBody = mailBuilder.ToString();

                                var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                            }
                            logRequest.OrderStatus = OrderStatusCode.Approved;
                            logRequest.LastDateModified = DateTime.Now;
                            await _context.ItemAcceptedOrRejected.AddAsync(logRequest);
                            await _context.SaveChangesAsync();
                            getTransactionLogs.TransactionStatus = OrderStatusCode.Approved;
                            getTransactionLogs.Status = true;
                            getTransactionLogs.IsAccepted = true;
                            getTransactionLogs.AcceptRejectLastDateModified = DateTime.Now;
                            _context.Update(getTransactionLogs);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                        }
                    }
                   
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidConfirmation };

            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
