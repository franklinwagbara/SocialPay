using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Messaging;
using SocialPay.Core.Messaging.SendGrid;
using SocialPay.Core.Services;
using SocialPay.Core.Services.Authentication;
using SocialPay.Core.Store;
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
        private readonly SendGridEmailService _sendGridEmailService;
        private readonly StoreRepository _storeRepository;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(ICustomerService));

        public ICustomerService(SocialPayDbContext context, AuthRepoService authRepoService,
            IOptions<AppSettings> appSettings, EmailService emailService,
            TransactionReceipt transactionReceipt, SendGridEmailService sendGridEmailService,
            StoreRepository storeRepository) : base(context)
        {
            _context = context;
            _authRepoService = authRepoService;
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _transactionReceipt = transactionReceipt;
            _sendGridEmailService = sendGridEmailService;
            _storeRepository = storeRepository ?? throw new ArgumentNullException(nameof(storeRepository));
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

        public async Task<MerchantPaymentSetup> GetPaymentLinkByCustomUrl(string customUrl)
        {
            return await _context.MerchantPaymentSetup
                .SingleOrDefaultAsync(p => p.CustomUrl
              == customUrl
            );
        }

        public async Task<bool> ValidateCustomerUrl(string customUrl)
        {
            return await _context.MerchantPaymentSetup
                .AnyAsync(p => p.CustomUrl
              == customUrl
            );
        }

        public async Task<WebApiResponse> GetMerchantPaymentInfo(string transactionReference)
        {
            var validateReference = await GetTransactionReference(transactionReference);

            if (validateReference == null)
                return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidPaymentReference };

            var getMerchantInfo = await GetMerchantInfo(validateReference.ClientAuthenticationId);

            if (getMerchantInfo == null)
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

                var response = (from c in getPaymentSetupInfo
                                join p in _context.TransactionLog on c.TransactionReference equals p.TransactionReference
                                join a in _context.CustomerOtherPaymentsInfo on p.PaymentReference equals a.PaymentReference
                                
                                where p.TransactionType != TransactionType.StorePayment
                                
                                select new CustomerPaymentViewModel
                                {
                                    MerchantAmount = c.MerchantAmount,
                                    CustomerEmail = a.Email,
                                    TotalAmount = a.Amount,
                                    CustomerPhoneNumber = a.PhoneNumber,
                                    TransactionDate = p.TransactionDate,
                                    ShippingFee = c.ShippingFee,
                                    DeliveryMethod = c.DeliveryMethod,
                                    CustomerAmount = c.CustomerAmount,
                                    DeliveryTime = c.DeliveryTime,
                                    MerchantDescription = c.MerchantDescription,
                                    CustomerDescription = a.CustomerDescription,
                                    TransactionReference = c.TransactionReference,
                                    Fullname = a.Fullname,
                                    Document = a.Document == null ? string.Empty : _appSettings.BaseApiUrl + a.FileLocation + "/" + a.Document,
                                    CustomerTransactionReference = p.CustomerTransactionReference
                                }).OrderByDescending(x => x.TransactionDate).ToList();

                result = response;

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetCustomerPaymentsByMerchantPayRef" + " | " + clientId + " | " + ex.Message.ToString() + " | " + DateTime.Now);

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

            var response = (from a in getPaymentSetupInfo
                            join p in _context.TransactionLog on a.PaymentReference equals p.PaymentReference
                            select new CustomerInfoViewModel
                            {
                                CustomerEmail = a.Email,
                                Fullname = a.Fullname,
                                CustomerPhoneNumber = a.PhoneNumber,
                                DateRegistered = a.DateEntered,
                                ClientAuthenticationId = p.CustomerInfo
                            }).OrderByDescending(x => x.DateRegistered).ToList();


            var uniquePersons = response.GroupBy(p => p.CustomerEmail)
                           .Select(grp => grp.First())
                           .ToArray();

            List<CustomerInfoViewModel> distinctPeople = response
  .GroupBy(p => new { p.CustomerEmail, p.CustomerPhoneNumber })
  .Select(g => g.First())
  .ToList();

            result = response
                      .GroupBy(p => new { p.CustomerEmail, p.CustomerPhoneNumber, p.Fullname, p.DateRegistered })
                      .Select(g => g.First())
                      .ToList();
            result = distinctPeople;

            // response.GroupBy(item => (item.CustomerEmail, item.CustomerPhoneNumber, item.Fullname,
            // item.DateRegistered)).Select(group => group.First()).ToList();
            //// var distinctList = response.Distinct(x => x.).ToList();

            // result = response;

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
        }

        public async Task<List<CustomerTransaction>> GetCustomerPaymentsByMerchantId(long merchantId)
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
            var createCustomer = await _authRepoService.CreateAccount(email, password, fullname, phoneNumber);
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

        public async Task<dynamic> GetTransactionDetails(string customUrl)
        {
            try
            {
                var paymentview = new PaymentLinkViewModel();

                var validateReference = await GetPaymentLinkByCustomUrl(customUrl);

                if (validateReference == null)
                    return new PaymentLinkViewModel { };

                var validateLink = await GetLinkCategorybyTranref(validateReference.TransactionReference);

                if (validateLink == null)
                    return new PaymentLinkViewModel { };

                if (validateLink.Channel == MerchantPaymentLinkCategory.InvoiceLink)
                    return await GetInvoiceTransactionDetails(validateReference.TransactionReference);

                if (validateReference.MerchantStoreId > 0)
                    return await _storeRepository.GetStoreInfobyStoreIdAsync(validateReference.MerchantStoreId, validateReference.TransactionReference);

               // var getMerchantInfo = await GetMerchantInfo(validateLink.ClientAuthenticationId);
                var getMerchantInfo = await _context.ClientAuthentication
                    .Include(x=>x.MerchantBusinessInfo).SingleOrDefaultAsync(x=>x.ClientAuthenticationId == validateLink.ClientAuthenticationId);

                if (getMerchantInfo == null)
                    return new PaymentLinkViewModel { };               

                var config = new MapperConfiguration(cfg => cfg.CreateMap<MerchantPaymentSetup, PaymentLinkViewModel>());
                var mapper = config.CreateMapper();

                paymentview = mapper.Map<PaymentLinkViewModel>(validateReference);

                paymentview.MerchantDocument = validateReference == null ? string.Empty : _appSettings.BaseApiUrl + validateReference.FileLocation + "/" + validateReference.Document;

                //paymentview.MerchantInfo = new MerchantInfoViewModel
                //{
                //    BusinessEmail = getMerchantInfo.BusinessEmail,
                //    BusinessPhoneNumber = getMerchantInfo.BusinessPhoneNumber,
                //    BusinessName = getMerchantInfo.BusinessName,
                //    Chargebackemail = getMerchantInfo.Chargebackemail,
                //    Country = getMerchantInfo.Country,
                //    HasSpectaMerchantID = getMerchantInfo == null ? false : getMerchantInfo.HasSpectaMerchantID,
                //    Logo = getMerchantInfo == null ? string.Empty : _appSettings.BaseApiUrl + getMerchantInfo.FileLocation + "/" + getMerchantInfo.Logo
                //};


                paymentview.MerchantInfo = new MerchantInfoViewModel
                {
                    BusinessEmail = getMerchantInfo == null ? "Invalid Email" : getMerchantInfo.MerchantBusinessInfo == null ? getMerchantInfo.Email : getMerchantInfo.MerchantBusinessInfo.Select(x=>x.BusinessEmail).FirstOrDefault(),
                    BusinessPhoneNumber = getMerchantInfo == null ? "Invalid Phone number" : getMerchantInfo.MerchantBusinessInfo == null ? getMerchantInfo.PhoneNumber : getMerchantInfo.MerchantBusinessInfo.Select(x => x.BusinessPhoneNumber).FirstOrDefault(),
                    BusinessName = getMerchantInfo == null ? "Invalid business email" : getMerchantInfo.MerchantBusinessInfo == null ? getMerchantInfo.FullName : getMerchantInfo.MerchantBusinessInfo.Select(x => x.BusinessName).FirstOrDefault(),
                    Chargebackemail = getMerchantInfo == null ? "Invalid charge back email" : getMerchantInfo.MerchantBusinessInfo == null ? "Invalid charge back email" : getMerchantInfo.MerchantBusinessInfo.Select(x => x.Chargebackemail).FirstOrDefault(),
                    Country = getMerchantInfo == null ? "Invalid country" : getMerchantInfo.MerchantBusinessInfo == null ? "Invalid charge back email" : getMerchantInfo.MerchantBusinessInfo.Select(x => x.Country).FirstOrDefault(),
                    HasSpectaMerchantID = getMerchantInfo == null ? false : getMerchantInfo.MerchantBusinessInfo == null ? false : getMerchantInfo.MerchantBusinessInfo.Select(x => x.HasSpectaMerchantID).FirstOrDefault(),
                    Logo = getMerchantInfo == null ? "Invalid business logo" : getMerchantInfo.MerchantBusinessInfo == null ? getMerchantInfo.FullName : _appSettings.BaseApiUrl + getMerchantInfo.MerchantBusinessInfo.Select(x => x.FileLocation).FirstOrDefault() + "/" + getMerchantInfo.MerchantBusinessInfo.Select(x => x.Logo).FirstOrDefault(),
                    //  Logo = getMerchantInfo == null ? string.Empty : _appSettings.BaseApiUrl + getMerchantInfo.FileLocation + "/" + getMerchantInfo.Logo
                };

                return paymentview;
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetTransactionDetails" + " | " + customUrl + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<InvoiceViewModel> GetInvoiceTransactionDetails(string refId)
        {
            try
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
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetInvoiceTransactionDetails" + " | " + refId + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return null;
            }
        }


        public async Task<WebApiResponse> GetCustomerOrders(long clientId, string category)
        {
            var request = new List<OrdersViewModel>();
            try
            {
                var getCustomerOrders = await GetTransactionByClientId(clientId);
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
                                               ClientId = clientId,
                                               CustomerTransactionReference = c.CustomerTransactionReference,
                                               TotalAmount = m.TotalAmount,
                                               PaymentCategory = category,
                                               OrderStatus = c.OrderStatus,
                                               RequestId = c.TransactionLogId
                                           }).ToList();
                    request = invoiceResponse;
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request };
                }

                var otherLinksresponse = (from c in getCustomerOrders
                                          join m in _context.MerchantPaymentSetup on c.TransactionReference equals m.TransactionReference
                                          join v in _context.CustomerOtherPaymentsInfo on c.PaymentReference equals v.PaymentReference
                                          select new OrdersViewModel
                                          {
                                              MerchantAmount = m.MerchantAmount,
                                              DeliveryTime = c.DeliveryDate,
                                              ShippingFee = m.ShippingFee,
                                              TransactionReference = m.TransactionReference,
                                              DeliveryMethod = m.DeliveryMethod,
                                              MerchantDescription = m.MerchantDescription,
                                              TransactionStatus = c.TransactionStatus,
                                              CustomerDescription = v.CustomerDescription,
                                              TotalAmount = v.Amount,
                                              PaymentCategory = m.PaymentCategory,
                                              ClientId = clientId,
                                              CustomerTransactionReference = c.CustomerTransactionReference,
                                              PaymentReference = v.PaymentReference,
                                              OrderStatus = c.OrderStatus,
                                              RequestId = c.TransactionLogId
                                          }).ToList();

                request = otherLinksresponse;

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetCustomerOrders" + " | " + clientId + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }

        public async Task<List<MerchantPaymentSetup>> GetAllPaymentLinksByClientId(long clientId)
        {
            return await _context.MerchantPaymentSetup.Where(x => x.IsDeleted
            == false && x.ClientAuthenticationId == clientId && x.IsDeleted == false).ToListAsync();
        }

        public async Task<List<PaymentLinkViewModel>> GetPaymentLinks(long clientId)
        {
            var paymentview = new List<PaymentLinkViewModel>();
            var validateReference = await GetAllPaymentLinksByClientId(clientId);

            if (validateReference == null)
                return paymentview;

            var config = new MapperConfiguration(cfg => cfg.CreateMap<MerchantPaymentSetup, PaymentLinkViewModel>());
            var mapper = config.CreateMapper();
            paymentview = mapper.Map<List<PaymentLinkViewModel>>(validateReference);
            //getCreatedCart.ForEach(s => s.Status = true);
            //paymentview.ForEach(x => x.MerchantPaymentLinkId = validateReference.Select(x => x.MerchantPaymentSetupId).FirstOrDefault());
            return paymentview;
        }

        public async Task<WebApiResponse> LogInvoicePaymentResponse(PaymentValidationRequestDto model, string reference)
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

                    if (getpaymentInfo.TransactionStatus == TransactionJourneyStatusCodes.Approved)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };

                    if (model.Message.Contains("approve") || model.Message.Contains("success") || model.Message.Contains("Approve"))
                    {
                        using (var transaction = await _context.Database.BeginTransactionAsync())
                        {
                            try
                            {
                                var logconfirmation = new TransactionLog { };
                                logconfirmation.Category = linkInfo.Channel;
                                logconfirmation.CustomerEmail = getpaymentInfo.Email;
                                logconfirmation.CustomerTransactionReference = getpaymentInfo.CustomerTransactionReference;
                                logconfirmation.TransactionReference = model.TransactionReference;
                                logconfirmation.OrderStatus = TransactionJourneyStatusCodes.Pending;
                                logconfirmation.Message = model.Message;
                                logconfirmation.LastDateModified = DateTime.Now;
                                logconfirmation.CustomerInfo = model.CustomerId;
                                logconfirmation.Status = true;
                                logconfirmation.PaymentReference = model.InvoiceReference;
                                logconfirmation.PaymentChannel = model.Channel;
                                logconfirmation.TransactionJourney = TransactionJourneyStatusCodes.Approved;
                                logconfirmation.TransactionStatus = TransactionJourneyStatusCodes.Approved;
                                logconfirmation.ActivityStatus = TransactionJourneyStatusCodes.Approved;
                                logconfirmation.OtherPaymentReference = reference;

                                var merchantInfo = await GetMerchantInfo(linkInfo.ClientAuthenticationId);
                                var invoiceInfo = await GetInvoicePaymentAsync(model.TransactionReference);
                                logconfirmation.TotalAmount = invoiceInfo.TotalAmount;
                                logconfirmation.ClientAuthenticationId = invoiceInfo.ClientAuthenticationId;
                                getpaymentInfo.TransactionStatus = TransactionJourneyStatusCodes.Approved;
                                logconfirmation.TransactionJourney = TransactionJourneyStatusCodes.Approved;
                                getpaymentInfo.Status = true;
                                getpaymentInfo.Message = model.Message;
                                getpaymentInfo.LastDateModified = DateTime.Now;
                                logconfirmation.DeliveryDayTransferStatus = TransactionJourneyStatusCodes.Pending;

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
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {

                            logFailedResponse.CustomerTransactionReference = getpaymentInfo.CustomerTransactionReference;
                            logFailedResponse.TransactionReference = getpaymentInfo.TransactionReference;
                            logFailedResponse.Message = model.Message;

                            await _context.FailedTransactions.AddAsync(logFailedResponse);
                            await _context.SaveChangesAsync();

                            getpaymentInfo.TransactionStatus = TransactionJourneyStatusCodes.TransactionFailed;
                            getpaymentInfo.Status = false;
                            getpaymentInfo.Message = model.Message;
                            getpaymentInfo.LastDateModified = DateTime.Now;
                            _context.Update(getpaymentInfo);

                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();

                            if (model.Message.Contains("Incorrect PIN"))
                                return new WebApiResponse { ResponseCode = AppResponseCodes.IncorrectTransactionPin, Data = "Incorrect Transaction Pin" };

                            if (model.Message.Contains("Insufficient"))
                                return new WebApiResponse { ResponseCode = AppResponseCodes.InsufficientFunds, Data = "Insufficient Funds" };

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


        public async Task<WebApiResponse> LogPaymentResponse(PaymentValidationRequestDto model, string reference)
        {
            try
            {

                var validateDuplicateTransaction = await _context.TransactionLog
                    .SingleOrDefaultAsync(x => x.Message == model.Message);

                if (validateDuplicateTransaction != null)
                {
                    _log4net.Info("LogPaymentResponse" + " - " + model.PaymentReference + " - " + "validate Duplicate Transaction. DuplicateTransaction" + " - " + DateTime.Now);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                }

                var logconfirmation = new TransactionLog { };
                var linkInfo = await GetLinkCategorybyTranref(model.TransactionReference);

                var paymentSetupInfo = await _context.MerchantPaymentSetup
               .SingleOrDefaultAsync(x => x.TransactionReference == model.TransactionReference);

                if (paymentSetupInfo == null)
                {
                    _log4net.Info("LogPaymentResponse" + " - " + model.PaymentReference + " - " + "paymentSetupInfo. RecordNotFound" + " - " + DateTime.Now);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };
                }

                // var merchantInfo = await GetMerchantInfo(paymentSetupInfo.ClientAuthenticationId);

                var merchantInfo = await _context.ClientAuthentication.Include(x => x.MerchantBusinessInfo)
                   .SingleOrDefaultAsync(x => x.ClientAuthenticationId == paymentSetupInfo.ClientAuthenticationId);

                var getCustomerInfo = await _context.CustomerOtherPaymentsInfo
                    .SingleOrDefaultAsync(x => x.PaymentReference == model.PaymentReference);

                //for escrow if need to activate
                ////if (linkInfo != null && linkInfo.Channel == MerchantPaymentLinkCategory.Escrow || linkInfo.Channel == MerchantPaymentLinkCategory.OneOffEscrowLink)
                ////{
                ////    var customerInfo = await _context.ClientAuthentication
                ////    .SingleOrDefaultAsync(x => x.ClientAuthenticationId == model.CustomerId);
                ////    logconfirmation.Category = linkInfo.Channel;
                ////    logconfirmation.LinkCategory = paymentSetupInfo.PaymentCategory;
                ////    logconfirmation.PaymentChannel = model.Channel;
                ////    logconfirmation.ClientAuthenticationId = paymentSetupInfo.ClientAuthenticationId;
                ////    logconfirmation.CustomerInfo = model.CustomerId;
                ////    logconfirmation.CustomerEmail = customerInfo.Email;
                ////    logconfirmation.CustomerTransactionReference = Guid.NewGuid().ToString();
                ////    logconfirmation.TransactionReference = model.TransactionReference;
                ////    logconfirmation.OrderStatus = TransactionJourneyStatusCodes.Pending;
                ////    logconfirmation.Message = model.Message;
                ////    logconfirmation.LastDateModified = DateTime.Now;
                ////    logconfirmation.TotalAmount = getCustomerInfo.Amount;
                ////    logconfirmation.DeliveryDayTransferStatus = TransactionJourneyStatusCodes.Pending;
                ////    logconfirmation.PaymentReference = model.PaymentReference;
                ////    logconfirmation.TransactionStatus = TransactionJourneyStatusCodes.Pending;
                ////    logconfirmation.TransactionJourney = TransactionJourneyStatusCodes.Pending;
                ////    logconfirmation.ActivityStatus = TransactionJourneyStatusCodes.Pending;
                ////    logconfirmation.OtherPaymentReference = reference;

                ////    if (model.Message.Contains("approve") || model.Message.Contains("success") || model.Message.Contains("Approve"))
                ////    {
                ////        logconfirmation.Status = true;
                ////        logconfirmation.LastDateModified = DateTime.Now;

                ////        using (var transaction = await _context.Database.BeginTransactionAsync())
                ////        {
                ////            try
                ////            {
                ////                logconfirmation.DeliveryDate = DateTime.Now.AddDays(paymentSetupInfo.DeliveryTime);
                ////                logconfirmation.DeliveryFinalDate = logconfirmation.DeliveryDate.AddDays(2);
                ////                await _context.TransactionLog.AddAsync(logconfirmation);
                ////                await _context.SaveChangesAsync();
                ////                await transaction.CommitAsync();
                ////                //Send mail
                ////                await _transactionReceipt.ReceiptTemplate(logconfirmation.CustomerEmail, paymentSetupInfo.TotalAmount,
                ////                    logconfirmation.TransactionDate, model.TransactionReference, merchantInfo == null ? string.Empty : merchantInfo.BusinessName);

                ////                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                ////            }
                ////            catch (Exception ex)
                ////            {
                ////                await transaction.RollbackAsync();
                ////                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                ////            }
                ////        }
                ////    }

                ////    return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionFailed };
                ////}


                if (model.Message.Contains("approve") || model.Message.Contains("success") || model.Message.Contains("Approve"))
                {
                    logconfirmation.Category = linkInfo.Channel;
                    logconfirmation.LinkCategory = paymentSetupInfo.PaymentCategory;
                    logconfirmation.PaymentChannel = model.Channel;
                    logconfirmation.ClientAuthenticationId = paymentSetupInfo.ClientAuthenticationId;
                    logconfirmation.CustomerInfo = model.CustomerId;
                    logconfirmation.CustomerTransactionReference = Guid.NewGuid().ToString();
                    logconfirmation.TransactionReference = model.TransactionReference;
                    logconfirmation.OrderStatus = TransactionJourneyStatusCodes.Pending;
                    logconfirmation.Message = model.Message;
                    logconfirmation.LastDateModified = DateTime.Now;
                    logconfirmation.TotalAmount = getCustomerInfo.Amount;
                    logconfirmation.DeliveryDayTransferStatus = TransactionJourneyStatusCodes.Pending;
                    logconfirmation.PaymentReference = model.PaymentReference;
                    logconfirmation.TransactionStatus = TransactionJourneyStatusCodes.Approved;
                    logconfirmation.TransactionJourney = TransactionJourneyStatusCodes.Approved;
                    logconfirmation.ActivityStatus = TransactionJourneyStatusCodes.Approved;
                    logconfirmation.TransactionType = TransactionType.OtherPayment;
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

                            //Send mail
                            await _transactionReceipt.ReceiptTemplate(logconfirmation.CustomerEmail, paymentSetupInfo.TotalAmount,
                            logconfirmation.TransactionDate, model.TransactionReference, merchantInfo == null ? string.Empty : merchantInfo.FullName);


                            var emailModal = new EmailRequestDto
                            {
                                Subject = $"{_appSettings.successfulTransactionEmailSubject}{"-"}{model.TransactionReference}{"-"}",
                                DestinationEmail = merchantInfo.MerchantBusinessInfo.Count() == 0 ? merchantInfo.MerchantBusinessInfo.Select(x => x.BusinessEmail).FirstOrDefault() : merchantInfo.Email,
                            };

                          //  emailModal.DestinationEmail = "festypat9@gmail.com";

                            var mailBuilder = new StringBuilder();

                            mailBuilder.AppendLine("Dear" + " " + merchantInfo.Email + "," + "<br />");
                            mailBuilder.AppendLine("<br />");
                            mailBuilder.AppendLine("Customer was able to make payment successfully. See details below.<br />");
                            mailBuilder.AppendLine("<br />");
                            mailBuilder.AppendLine("Customer Name:" + "  " + getCustomerInfo.Fullname + "<br />");
                            mailBuilder.AppendLine("<br />");
                            mailBuilder.AppendLine("Customer Phone number:" + "  " + getCustomerInfo.PhoneNumber + "<br />");
                            mailBuilder.AppendLine("<br />");
                            mailBuilder.AppendLine("Transaction Amount:" + "  " + logconfirmation.TotalAmount + "<br />");
                            mailBuilder.AppendLine("<br />");
                            mailBuilder.AppendLine("Transaction Reference:" + "  " + logconfirmation.TransactionReference + "<br />");
                            mailBuilder.AppendLine("<br />");
                            mailBuilder.AppendLine("Payment Reference:" + "  " + logconfirmation.PaymentReference + "<br />");
                            mailBuilder.AppendLine("<br />");
                            // mailBuilder.AppendLine("Best Regards,");
                            emailModal.EmailBody = mailBuilder.ToString();

                            //var sendMail = await _sendGridEmailService.SendMail(mailBuilder.ToString(), emailModal.DestinationEmail, emailModal.Subject);

                            //if (sendMail.ResponseCode != AppResponseCodes.Success)
                            //    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Data = "Request Failed" };

                            await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

                            var customerEmailModal = new EmailRequestDto
                            {
                                Subject = $"{_appSettings.successfulTransactionEmailSubject}{"-"}{model.TransactionReference}{"-"}",
                                DestinationEmail = getCustomerInfo.Email,
                            };

                           // customerEmailModal.DestinationEmail = getCustomerInfo.Email;

                            var customermailBuilder = new StringBuilder();

                            customermailBuilder.AppendLine("Dear" + " " + getCustomerInfo.Fullname + "," + "<br />");
                            customermailBuilder.AppendLine("<br />");
                            customermailBuilder.AppendLine("Your payment was successful. See details below.<br />");
                            customermailBuilder.AppendLine("<br />");
                            customermailBuilder.AppendLine("Transaction Amount:" + "  " + logconfirmation.TotalAmount + "<br />");
                            customermailBuilder.AppendLine("<br />");
                            customermailBuilder.AppendLine("Transaction Reference:" + "  " + logconfirmation.TransactionReference + "<br />");
                            customermailBuilder.AppendLine("<br />");
                            customermailBuilder.AppendLine("Payment Reference:" + "  " + logconfirmation.PaymentReference + "<br />");
                            customermailBuilder.AppendLine("<br />");
                            // mailBuilder.AppendLine("Best Regards,");
                            customerEmailModal.EmailBody = customermailBuilder.ToString();

                            await _emailService.SendMail(customerEmailModal, _appSettings.EwsServiceUrl);

                            //var sendCustomerMail = await _sendGridEmailService.SendMail(mailBuilder.ToString(), emailModal.DestinationEmail, emailModal.Subject);

                            //if (sendCustomerMail.ResponseCode != AppResponseCodes.Success)
                            //    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Data = "Request Failed" };

                            _log4net.Info("Emails was successfully sent" + " | " + "LogPaymentResponse" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + DateTime.Now);

                            await transaction.CommitAsync();

                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Request failed" };
                        }
                        catch (Exception ex)
                        {
                            _log4net.Error("Database Error occured" + " | " + "LogPaymentResponse" + " | " + model.PaymentReference + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                            await transaction.RollbackAsync();

                            return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                        }
                    }
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    _log4net.Info("Transaction failed" + " | " + "LogPaymentResponse" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + model.Message + " - " + DateTime.Now);

                    try
                    {
                        var logFailedResponse = new FailedTransactions();

                        //logFailedResponse.CustomerTransactionReference = model.CustomerTransactionReference;
                        logFailedResponse.TransactionReference = model.TransactionReference;
                        logFailedResponse.Message = model.Message;
                        await _context.FailedTransactions.AddAsync(logFailedResponse);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        if (model.Message.Contains("Incorrect PIN"))
                        {
                            _log4net.Info("Transaction failed" + " | " + "Incorrect PIN" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + model.Message + " - " + DateTime.Now);

                            return new WebApiResponse { ResponseCode = AppResponseCodes.IncorrectTransactionPin, Data = "Incorrect Transaction Pin" };
                        }

                        if (model.Message.Contains("Insufficient"))
                        {
                            _log4net.Info("Transaction failed" + " | " + "Insufficient" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + model.Message + " - " + DateTime.Now);
                            return new WebApiResponse { ResponseCode = AppResponseCodes.InsufficientFunds, Data = "Insufficient Funds" };
                        }

                        return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionFailed };
                    }
                    catch (Exception ex)
                    {
                        _log4net.Error("Error occured" + " | " + "LogPaymentResponse" + " | " + model.PaymentReference + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                    }
                }

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "LogPaymentResponse" + " | " + model.PaymentReference + " | " + ex.Message.ToString() + " | " + DateTime.Now);

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
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            if (model.Status == TransactionJourneyStatusCodes.Decline)
                            {
                                if (getTransactionLogs.TransactionStatus == TransactionJourneyStatusCodes.Decline)
                                {
                                    logRequest.OrderStatus = TransactionJourneyStatusCodes.Dispute;
                                    await _context.ItemAcceptedOrRejected.AddAsync(logRequest);
                                    await _context.SaveChangesAsync();
                                    getTransactionLogs.TransactionStatus = TransactionJourneyStatusCodes.Dispute;
                                    getTransactionLogs.ActivityStatus = TransactionJourneyStatusCodes.Dispute;
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

                            if (model.Status == TransactionJourneyStatusCodes.Approved)
                            {
                                logRequest.OrderStatus = TransactionJourneyStatusCodes.ItemAccepted;
                                await _context.ItemAcceptedOrRejected.AddAsync(logRequest);
                                await _context.SaveChangesAsync();
                                getTransactionLogs.TransactionStatus = TransactionJourneyStatusCodes.ItemAccepted;
                                getTransactionLogs.ActivityStatus = TransactionJourneyStatusCodes.ItemAccepted;
                                getTransactionLogs.TransactionJourney = TransactionJourneyStatusCodes.ItemAccepted;
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

                if (getTransactionLogs != null && getTransactionLogs.TransactionStatus != TransactionJourneyStatusCodes.Pending)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionProcessed };

                if (getTransactionLogs.DeliveryDate < DateTime.Now)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.OrderHasExpired };

                //if (await _context.ItemAcceptedOrRejected
                //    .AnyAsync(x => x.CustomerTransactionReference == model.CustomerTransactionReference))
                //return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionAlreadyexit };
                if (model.Status == TransactionJourneyStatusCodes.Decline || model.Status == TransactionJourneyStatusCodes.Approved)
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
                            var emailModal = new EmailRequestDto();
                            var mailBuilder = new StringBuilder();
                            var getMerchant = await _context.ClientAuthentication
                                  .SingleOrDefaultAsync(x => x.ClientAuthenticationId == getTransactionLogs.ClientAuthenticationId);
                            //{
                            //    Subject = "Order" + " " + model.TransactionReference + " " + "was Rejected",
                            //    SourceEmail = "info@sterling.ng",
                            //    DestinationEmail = getMerchant.Email,
                            //    // DestinationEmail = "festypat9@gmail.com",
                            //    //  EmailBody = "Your onboarding was successfully created. Kindly use your email as username and" + "   " + "" + "   " + "as password to login"
                            //};
                            if (model.Status == TransactionJourneyStatusCodes.Decline)
                            {

                                //if (response.DeliveryDate.AddDays(sla) < DateTime.Now)
                                //    return new WebApiResponse { ResponseCode = AppResponseCodes.OrderHasExpired };

                                logRequest.OrderStatus = TransactionJourneyStatusCodes.Decline;
                                logRequest.LastDateModified = DateTime.Now;
                                logRequest.ReturnedDate = DateTime.Now.AddDays(Convert.ToInt32(_appSettings.returnedDateSLA));
                                await _context.ItemAcceptedOrRejected.AddAsync(logRequest);
                                await _context.SaveChangesAsync();
                                getTransactionLogs.TransactionStatus = TransactionJourneyStatusCodes.Decline;
                                getTransactionLogs.ActivityStatus = TransactionJourneyStatusCodes.Decline;
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
                                emailModal.Subject = "Accept/Reject Request";
                                emailModal.DestinationEmail = getMerchant.Email;
                                emailModal.Subject = "Order" + " " + model.TransactionReference + " " + "was Rejected";
                                mailBuilder.AppendLine("Dear" + " " + getMerchant.Email + "," + "<br />");
                                mailBuilder.AppendLine("<br />");
                                mailBuilder.AppendLine("An order has been rejected by" + "" + getTransactionLogs.CustomerEmail + " " + ".<br />");
                                //mailBuilder.AppendLine("Kindly use this token" + "  " + newPin + "  " + "and" + " " + urlPath + "<br />");
                                // mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                                mailBuilder.AppendLine("Best Regards,");
                                emailModal.EmailBody = mailBuilder.ToString();

                                await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);


                                //var sendMail = await _sendGridEmailService.SendMail(mailBuilder.ToString(), emailModal.DestinationEmail, emailModal.Subject);

                                //if (sendMail.ResponseCode != AppResponseCodes.Success)
                                //    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Data = "Request Failed" };

                                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                            }

                            logRequest.OrderStatus = TransactionJourneyStatusCodes.Approved;
                            logRequest.LastDateModified = DateTime.Now;

                            await _context.ItemAcceptedOrRejected.AddAsync(logRequest);
                            await _context.SaveChangesAsync();

                            getTransactionLogs.TransactionStatus = TransactionJourneyStatusCodes.Approved;
                            getTransactionLogs.ActivityStatus = TransactionJourneyStatusCodes.Approved;
                            getTransactionLogs.Status = true;
                            getTransactionLogs.IsAccepted = true;
                            getTransactionLogs.AcceptRejectLastDateModified = DateTime.Now;

                            _context.Update(getTransactionLogs);

                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();

                            emailModal.Subject = "Accept/Reject Request";
                            emailModal.DestinationEmail = getMerchant.Email;
                            emailModal.Subject = "Order" + " " + model.TransactionReference + " " + "was Accepted";
                            mailBuilder.AppendLine("Dear" + " " + getMerchant.Email + "," + "<br />");
                            mailBuilder.AppendLine("<br />");
                            mailBuilder.AppendLine("An order has been rejected by" + "" + getTransactionLogs.CustomerEmail + " " + ".<br />");
                            //mailBuilder.AppendLine("Kindly use this token" + "  " + newPin + "  " + "and" + " " + urlPath + "<br />");
                            // mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                            mailBuilder.AppendLine("Best Regards,");
                            emailModal.EmailBody = mailBuilder.ToString();

                            await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

                            //var sendCustomerMail = await _sendGridEmailService.SendMail(mailBuilder.ToString(), emailModal.DestinationEmail, emailModal.Subject);

                            //if (sendCustomerMail.ResponseCode != AppResponseCodes.Success)
                            //    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Data = "Request Failed" };

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
