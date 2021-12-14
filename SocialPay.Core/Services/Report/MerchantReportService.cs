using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Core.Messaging;
using SocialPay.Core.Repositories.Customer;
using SocialPay.Core.Repositories.Invoice;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
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
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(MerchantReportService));

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

        public async Task<WebApiResponse> GetMerchants(bool hasCompanyProfile)
        {
            var result = new List<MerchantBusinessInfoViewModel>();
            try
            {
                _log4net.Info("Initiating GetMerchants request" + " | " + DateTime.Now);


                var getMerchantInfo = await _context.ClientAuthentication
               .Where(x => x.RoleName == RoleDetails.Merchant && x.HasRegisteredCompany == hasCompanyProfile).ToListAsync();

                if (hasCompanyProfile)
                {
                    var response = (from c in getMerchantInfo
                                    join b in _context.MerchantBusinessInfo on c.ClientAuthenticationId equals b.ClientAuthenticationId
                                    join m in _context.MerchantBankInfo on c.ClientAuthenticationId equals m.ClientAuthenticationId
                                    select new MerchantBusinessInfoViewModel
                                    {
                                        BankInfo = new BankInfoViewModel
                                        {
                                            AccountName = m.AccountName,
                                            BankName = m.BankName,
                                            BVN = m.BVN,
                                            Country = m.Country,
                                            Currency = m.Currency,
                                            Nuban = m.Nuban
                                        },
                                        BusinessEmail = b.BusinessEmail,
                                        Country = b.Country,
                                        BusinessPhoneNumber = b.BusinessPhoneNumber,
                                        BusinessName = b.BusinessName,
                                        Chargebackemail = b.Chargebackemail,
                                        Logo = _appSettings.BaseApiUrl + b.FileLocation + "/" + b.Logo,
                                        ReferralCode = c.ReferralCode,
                                        ReferCode = c.ReferCode,
                                        Date = c.DateEntered,
                                        HasRegisteredCompany = c.HasRegisteredCompany,
                                        //AccountStatus = c.IsLocked == true ? "Locked/Disabled" : "Active"
                                    }).OrderByDescending(x => x.Date).ToList();


                    result = response;

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
                }

                else
                {
                    var response = (from c in getMerchantInfo
                                    join m in _context.MerchantBankInfo on c.ClientAuthenticationId equals m.ClientAuthenticationId
                                    select new MerchantBusinessInfoViewModel
                                    {
                                        BankInfo = new BankInfoViewModel
                                        {
                                            AccountName = m.AccountName,
                                            BankName = m.BankName,
                                            BVN = m.BVN,
                                            Country = m.Country,
                                            Currency = m.Currency,
                                            Nuban = m.Nuban
                                        },
                                        BusinessEmail = c.Email,
                                        BusinessPhoneNumber = c.PhoneNumber,
                                        BusinessName = c.FullName,
                                        Date = c.DateEntered,
                                        ReferralCode = c.ReferralCode,
                                        ReferCode = c.ReferCode,
                                        HasRegisteredCompany = c.HasRegisteredCompany,
                                        Logo = $"{_appSettings.BaseApiUrl}{"MerchantLogo/defaultLogo.jpg"}",
                                        //AccountStatus = c.IsLocked == true ? "Locked/Disabled" : "Active"
                                    }).OrderByDescending(x => x.Date).ToList();


                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = response };
                }

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetMerchants" + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = result };
            }
        }

        public async Task<WebApiResponse> GenerateCustomerReceipt(CustomerReceiptRequestDto model)
        {
            try
            {
                _log4net.Info("Initiating GenerateCustomerReceipt request" + " | " + DateTime.Now);

                //await _transactionReceipt.ReceiptTemplate("festypat9@gmail.com");
                var validateTransaction = await _customerService.GetTransactionReference(model.TransactionReference);

                if (validateTransaction == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidTransactionReference };

                var getMerchantInfo = await _customerService.GetMerchantInfo(validateTransaction.ClientAuthenticationId);

                var validateCustomer = await _customerService.GetTransactionLogsByReference(model.CustomerTransactionReference);

                //var validateCustomer = validateTransaction.CustomerTransaction
                //    .SingleOrDefault(x => x.CustomerTransactionId == model.CustomerTransactionId);
                await _transactionReceipt.ReceiptTemplate(validateCustomer.CustomerEmail,
                    validateTransaction.TotalAmount, validateCustomer.TransactionDate,
                    model.TransactionReference, getMerchantInfo == null ? string.Empty : getMerchantInfo.BusinessName);
                //Send Mail here

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GenerateCustomerReceipt" + " | " + model.TransactionReference + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetAllLoggedDisputes(long clientId, bool IsAdmin)
        {
            var result = new List<ItemDisputeViewModel>();
            try
            {
                _log4net.Info("Initiating GetAllLoggedDisputes request" + " | " + clientId + " | " + DateTime.Now);

                //clientId = 30032;
                if (IsAdmin)
                {
                    var getallDisputes = await _context.DisputeRequestLog.ToListAsync();

                    var dataResponse = (from d in getallDisputes
                                        join t in _context.TransactionLog on d.PaymentReference equals t.PaymentReference
                                        select new ItemDisputeViewModel
                                        {
                                            Comment = d.DisputeComment,
                                            ProcessedBy = d.ProcessedBy,
                                            TransactionReference = t.TransactionReference,
                                            CustomerTransactionReference = t.CustomerTransactionReference,
                                            DateEntered = d.DateEntered,
                                            PaymentReference = d.PaymentReference,
                                            Document = _appSettings.BaseApiUrl + d.FileLocation + "/" + d.DisputeFile
                                        }).ToList();

                    result = dataResponse;
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };

                }
                var getTransactions = await _context.TransactionLog
                    .Where(x => x.CustomerInfo == clientId || x.ClientAuthenticationId == clientId).ToListAsync();

                var response = (from t in getTransactions
                                join d in _context.DisputeRequestLog on t.PaymentReference equals d.PaymentReference
                                select new ItemDisputeViewModel
                                {
                                    Comment = d.DisputeComment,
                                    TransactionReference = t.TransactionReference,
                                    CustomerTransactionReference = t.CustomerTransactionReference,
                                    PaymentReference = d.PaymentReference,
                                    ProcessedBy = d.ProcessedBy,
                                    DateEntered = d.DateEntered,
                                    Document = _appSettings.BaseApiUrl + d.FileLocation + "/" + d.DisputeFile
                                }).ToList();

                result = response;
                _log4net.Info("Initiating GetAllLoggedDisputes response" + " | " + clientId + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllLoggedDisputes" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetAllUsers()
        {
            var result = new List<ItemDisputeViewModel>();
            try
            {
                _log4net.Info("Initiating GetAllUsers request" + " | " + DateTime.Now);

                //clientId = 30032;
                var response = await (from c in _context.ClientAuthentication
                                      select new
                                      {
                                          Email = c.Email,
                                          ClientAuthenticationId = c.ClientAuthenticationId,
                                          DateRegistered = c.DateEntered,
                                          StatusCode = c.StatusCode,
                                          UserRole = c.RoleName,
                                          UserName = c.UserName,
                                          PhoneNumber = c.PhoneNumber
                                      }).ToListAsync();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = response };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllLoggedDisputes" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

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
                _log4net.Error("Error occured" + " | " + "GetAllInvoiceByMerchantId" + " | " + clientId + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }



        public async Task<WebApiResponse> GetAllTransactions()
        {
            try
            {
                //clientId = 30032;
                var result = await _context.TransactionLog.ToListAsync();

                return new WebApiResponse { ResponseCode = "00", Data = result };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetAllFailedTransactions()
        {
            try
            {
                //clientId = 30032;
                var result = await _context.FailedTransactions.ToListAsync();

                return new WebApiResponse { ResponseCode = "00", Data = result };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
        //FioranoT24CardCreditRequest

        public async Task<WebApiResponse> FioranoCardRequest()
        {
            try
            {
                //clientId = 30032;
                var result = await _context.FioranoT24CardCreditRequest.OrderByDescending(x => x.TransactionDate).ToListAsync();

                return new WebApiResponse { ResponseCode = "00", Data = result };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> ClearFioranoCardRequest(string reference)
        {
            try
            {
                var validateMerchant = await _context.FioranoT24CardCreditRequest
                    .SingleOrDefaultAsync(x => x.PaymentReference == reference);

                if (validateMerchant != null)
                {
                    _context.Remove(validateMerchant);
                    await _context.SaveChangesAsync();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Successful" };
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Data = "Record Not found" };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetAllPaymentResponseLogs()
        {
            try
            {
                //clientId = 30032;
                var result = await _context.PaymentResponse.OrderByDescending(x => x.TransactionDate).ToListAsync();

                return new WebApiResponse { ResponseCode = "00", Data = result };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetAllDefaultWalletLogsAsync()
        {
            try
            {
                return new WebApiResponse { ResponseCode = "00", Data = await _context.DebitMerchantWalletTransferRequestLog.OrderByDescending(x => x.DateEntered).ToListAsync() };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetAllDefaultWalletTransferRequestLogsAsync()
        {
            try
            {
                return new WebApiResponse { ResponseCode = "00", Data = await _context.DefaultWalletTransferRequestLog.OrderByDescending(x => x.DateEntered).ToListAsync() };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetAllEscrowTransactions(long clientId, string status)
        {
            var result = new List<EscrowViewModel>();
            try
            {
                //clientId = 30032;
                _log4net.Info("Initiating GetAllEscrowTransactions request" + " | " + clientId + " | " + DateTime.Now);

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
                                join t in _context.TransactionLog on m.TransactionReference equals t.TransactionReference
                                join c in _context.CustomerOtherPaymentsInfo on t.PaymentReference equals c.PaymentReference
                                where t.TransactionStatus == status
                                select new EscrowViewModel
                                {
                                    ShippingFee = m.ShippingFee,
                                    PaymentCategory = m.PaymentCategory,
                                    PaymentLinkName = m.PaymentLinkName,
                                    MerchantAmount = m.MerchantAmount,
                                    DeliveryMethod = m.DeliveryMethod,
                                    MerchantDescription = m.MerchantDescription,
                                    TotalAmount = c.Amount,
                                    CustomerDescription = c.CustomerDescription,
                                    Channel = t.Category,
                                    ClientId = t.ClientAuthenticationId,
                                    DeliveryDay = t.DeliveryFinalDate,
                                    CustomerTransactionReference = t.CustomerTransactionReference,
                                    CustomerEmail = c.Email,
                                    Fullname = c.Fullname,
                                    PhoneNumber = c.PhoneNumber,
                                    TransactionReference = c.TransactionReference,
                                    PaymentReference = c.PaymentReference,
                                    ActivityStatus = t.ActivityStatus
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

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllEscrowTransactions" + " | " + clientId + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> InsertData()
        {
            try
            {
                var getallClients = await _context.ClientAuthentication.ToListAsync();
                var loginstat = new List<ClientLoginStatus>();
                foreach (var item in getallClients)
                {
                    loginstat.Add(new ClientLoginStatus
                    {
                        ClientAuthenticationId = item.ClientAuthenticationId,
                        IsSuccessful = true,
                        LoginAttempt = 0
                    });
                }

                await _context.ClientLoginStatus.AddRangeAsync(loginstat);
                await _context.SaveChangesAsync();
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<WebApiResponse> GetMerchantBankInfoAsync()
        {
            try
            {
                return new WebApiResponse { ResponseCode = "00", Data = await _context.MerchantBankInfo.OrderByDescending(x => x.DateEntered).ToListAsync() };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetMerchantBusinessInfoAsync()
        {
            try
            {
                return new WebApiResponse { ResponseCode = "00", Data = await _context.MerchantBusinessInfo.OrderByDescending(x => x.DateEntered).ToListAsync() };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetInterBankRequestAsync()
        {
            try
            {
                return new WebApiResponse { ResponseCode = "00", Data = await _context.InterBankTransactionRequest.OrderByDescending(x => x.DateEntered).ToListAsync() };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetNonEscrowBankTransactions()
        {
            try
            {
                return new WebApiResponse { ResponseCode = "00", Data = await _context.NonEscrowFioranoT24Request.OrderByDescending(x => x.TransactionDate).ToListAsync() };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetFioranoTransactions()
        {
            try
            {
                return new WebApiResponse { ResponseCode = "00", Data = await _context.FioranoT24TransactionResponse.OrderByDescending(x => x.TransactionDate).ToListAsync() };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetPaymentLinks()
        {
            try
            {
                return new WebApiResponse { ResponseCode = "00", Data = await _context.MerchantPaymentSetup.OrderByDescending(x => x.DateEntered).ToListAsync() };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> UpdateLink()
        {
            try
            {
                // var validateMerchant = await _context.MerchantPaymentSetup


                return new WebApiResponse { ResponseCode = "00", Data = await _context.MerchantPaymentSetup.OrderByDescending(x => x.DateEntered).ToListAsync() };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetCustomerOtherTransactionInfo()
        {
            try
            {
                return new WebApiResponse { ResponseCode = "00", Data = await _context.CustomerOtherPaymentsInfo.OrderByDescending(x => x.DateEntered).ToListAsync() };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetWalletInfo()
        {
            try
            {
                return new WebApiResponse { ResponseCode = "00", Data = await _context.MerchantWallet.OrderByDescending(x => x.DateEntered).ToListAsync() };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> ValidateMerchantInfo(string reference)
        {
            try
            {
                var validateMerchant = await _context.DebitMerchantWalletTransferRequestLog
                    .SingleOrDefaultAsync(x => x.PaymentReference == reference);

                if (validateMerchant != null)
                {
                    _context.Remove(validateMerchant);
                    await _context.SaveChangesAsync();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Successful" };
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Data = "Record Not found" };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }



        public async Task<WebApiResponse> ModifyFioranoRequestInfo(string reference)
        {
            try
            {
                var validateMerchant = await _context.NonEscrowFioranoT24Request
                    .SingleOrDefaultAsync(x => x.PaymentReference == reference);

                if (validateMerchant != null)
                {
                    _context.Remove(validateMerchant);
                    await _context.SaveChangesAsync();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Successful" };
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Data = "Record Not found" };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> RemoveInterbankRequestInfo(string reference)
        {
            try
            {
                var validateMerchant = await _context.InterBankTransactionRequest
                    .SingleOrDefaultAsync(x => x.PaymentReference == reference);

                if (validateMerchant != null)
                {
                    _context.Remove(validateMerchant);
                    await _context.SaveChangesAsync();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Successful" };
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Data = "Record Not found" };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> ClearDefaultLogs(string reference)
        {
            try
            {
                var validateMerchant = await _context.DefaultWalletTransferRequestLog
                    .SingleOrDefaultAsync(x => x.PaymentReference == reference);

                if (validateMerchant != null)
                {
                    _context.Remove(validateMerchant);
                    await _context.SaveChangesAsync();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Successful" };
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Data = "Record Not found" };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> UpdateTransLog(string paymentReference, string code)
        {
            try
            {
                var validateMerchant = await _context.TransactionLog
                    .SingleOrDefaultAsync(x => x.PaymentReference == paymentReference);

                if (validateMerchant != null)
                {
                    validateMerchant.TransactionJourney = code;
                    _context.Update(validateMerchant);
                    await _context.SaveChangesAsync();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Successful" };
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Data = "Record Not found" };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> UpdateCustomerInfo2(string paymentReference, string code)
        {

            var sql = "UPDATE transactionLog SET TransactionJourney = @TransactionJourney where PaymentReference = @PaymentReference";
            try
            {
                using (var connection = new SqlConnection("----------"))
                {
                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@TransactionJourney", SqlDbType.NVarChar).Value = code;
                        command.Parameters.AddWithValue("@PaymentReference", SqlDbType.NVarChar).Value = paymentReference;
                        //  command.Parameters.Add("@PaymentReference", SqlDbType.NVarChar).Value = Fnamestring;
                        // repeat for all variables....
                        connection.Open();
                        command.ExecuteNonQuery();

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Successful" };

                    }
                }
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };

            }
        }


        public async Task<WebApiResponse> ValidateInfo(string reference)
        {
            try
            {
                var validateMerchant = await _context.TransactionLog
                    .SingleOrDefaultAsync(x => x.PaymentReference == reference);

                if (validateMerchant != null)
                {
                    validateMerchant.TransactionJourney = TransactionJourneyStatusCodes.FioranoFirstFundingCompleted;
                    _context.Update(validateMerchant);
                    await _context.SaveChangesAsync();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Successful" };
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Data = "Record Not found" };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> ValidateWalletInfo(string reference)
        {
            try
            {
                var validateMerchant = await _context.TransactionLog
                    .SingleOrDefaultAsync(x => x.PaymentReference == reference);

                if (validateMerchant != null)
                {
                    validateMerchant.TransactionJourney = TransactionJourneyStatusCodes.WalletTranferCompleted;
                    _context.Update(validateMerchant);
                    await _context.SaveChangesAsync();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Successful" };
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Data = "Record Not found" };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> UpdateWalletInfo(string reference)
        {
            try
            {
                var validateMerchant = await _context.TransactionLog
                    .SingleOrDefaultAsync(x => x.PaymentReference == reference);

                if (validateMerchant != null)
                {
                    validateMerchant.OrderStatus = TransactionJourneyStatusCodes.Pending;
                    _context.Update(validateMerchant);
                    await _context.SaveChangesAsync();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Successful" };
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Data = "Record Not found" };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> InterRequestAsync(string reference)
        {
            try
            {
                var validateMerchant = await _context.TransactionLog
                    .SingleOrDefaultAsync(x => x.PaymentReference == reference);

                if (validateMerchant != null)
                {
                    validateMerchant.TransactionJourney = TransactionJourneyStatusCodes.WalletTranferCompleted;
                    _context.Update(validateMerchant);
                    await _context.SaveChangesAsync();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Successful" };
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Data = "Record Not found" };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllTransactions" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetCustomersTransactionCount(long clientId)
        {
            try
            {
                //  clientId = 238;
                _log4net.Info("GetCustomersTransactionCount" + " | " + clientId + " | " + DateTime.Now);

                var result = await _context.TransactionLog
                    .Where(x => x.ClientAuthenticationId == clientId)
                    .ToListAsync();

                var transactionCount = result.Count();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = transactionCount };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetCustomersTransactionCount" + " | " + clientId + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetCustomersTransactionValue(long clientId)
        {
            try
            {
                _log4net.Info("GetCustomersTransactionValue" + " | " + clientId + " | " + DateTime.Now);

                var result = await _context.TransactionLog
                    .Where(x => x.ClientAuthenticationId == clientId)
                    .ToListAsync();
                var transactionValue = result.Sum(x => x.TotalAmount);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = transactionValue };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetCustomersTransactionValue" + " | " + clientId + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> MostUsedPaymentChannel(long clientId)
        {
            try
            {
                _log4net.Info("GetCustomersTransactionValue" + " | " + clientId + " | " + DateTime.Now);

                var result = await _context.TransactionLog
                    .Where(x => x.ClientAuthenticationId == clientId)
                    .ToListAsync();
                var transactionValue = result.GroupBy(x => x.PaymentChannel, (key, value) =>
                new UsedPaymentChannel
                {
                    paymentChannel = key,
                    Amount = value.Sum(x => x.TotalAmount)


                }).OrderByDescending(o => o.Amount).Take(5).ToList();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = transactionValue };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetCustomersTransactionValue" + " | " + clientId + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> AdminGetCustomersTransactionCount(string category)
        {

            _log4net.Info("AdminGetCustomersTransactionCount" + " | " + DateTime.Now);

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
                                               TotalAmount = c.TotalAmount

                                           })
                               .OrderByDescending(x => x.CustomerTransactionId).ToList();

                    request = invoiceResponse;

                    _log4net.Info("Response for GetCustomerOrders" + " - " + category + " - " + request.Count + " - " + DateTime.Now);

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request.Count() };
                }

                var otherLinksresponse = (from c in getCustomerOrders
                                          join m in _context.MerchantPaymentSetup on c.TransactionReference equals m.TransactionReference
                                          join a in _context.MerchantBusinessInfo on m.ClientAuthenticationId equals a.ClientAuthenticationId
                                          join b in _context.CustomerOtherPaymentsInfo on c.PaymentReference equals b.PaymentReference
                                          select new OrdersViewModel
                                          {
                                              TotalAmount = c.TotalAmount,
                                              MerchantAmount = m.MerchantAmount,
                                          })
                                .OrderByDescending(x => x.CustomerTransactionId).ToList();

                request = otherLinksresponse;

                // _log4net.Info("Response for GetCustomerOrders" + " - " + category + " - " + request.Count + " - " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request.Count() };

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "AdminGetCustomersTransactionCount" + " | " + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }



        }


        public async Task<WebApiResponse> AdminGetCustomersTransactionValue(string category)
        {




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
                                               TotalAmount = c.TotalAmount

                                           })
                               .OrderByDescending(x => x.CustomerTransactionId).ToList();

                    request = invoiceResponse;

                    _log4net.Info("Response for GetCustomerOrders" + " - " + category + " - " + request.Count + " - " + DateTime.Now);

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request.Sum(x => x.TotalAmount) };
                }

                var otherLinksresponse = (from c in getCustomerOrders
                                          join m in _context.MerchantPaymentSetup on c.TransactionReference equals m.TransactionReference
                                          join a in _context.MerchantBusinessInfo on m.ClientAuthenticationId equals a.ClientAuthenticationId
                                          join b in _context.CustomerOtherPaymentsInfo on c.PaymentReference equals b.PaymentReference
                                          select new OrdersViewModel
                                          {
                                              TotalAmount = c.TotalAmount,
                                              MerchantAmount = m.MerchantAmount,
                                          })
                                .OrderByDescending(x => x.CustomerTransactionId).ToList();

                request = otherLinksresponse;

                _log4net.Info("Response for GetCustomerOrders" + " - " + category + " - " + request.Count + " - " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request.Sum(x => x.TotalAmount) };

            }

            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "AdminGetCustomersTransactionCount" + " | " + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }


        }


        public async Task<WebApiResponse> AdminMostUsedPaymentChannel()
        {
            try
            {
                _log4net.Info("AdminMostUsedPaymentChannel" + " | " + DateTime.Now);

                var result = await _context.TransactionLog
                    .ToListAsync();
                var transactionValue = result.GroupBy(x => x.PaymentChannel, (key, value) =>
                new UsedPaymentChannel
                {
                    paymentChannel = key,
                    Amount = value.Sum(x => x.TotalAmount)


                }).OrderByDescending(o => o.Amount).Take(5).ToList();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = transactionValue };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "AdminMostUsedPaymentChannel" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }



        public async Task<UserInfoViewModel> RedisCacheTest()
        {
            try
            {
                var cacheKey = "pat";
                //var cacheKey = "festypat";
                string serializedCustomerList;
                var userInfo = new UserInfoViewModel { };
                var redisCustomerList = await _distributedCache.GetAsync(cacheKey);
                // await _distributedCache.RemoveAsync(cacheKey);
                if (redisCustomerList != null)
                {
                    serializedCustomerList = Encoding.UTF8.GetString(redisCustomerList);
                    return JsonConvert.DeserializeObject<UserInfoViewModel>(serializedCustomerList);
                }
                await _distributedCache.RemoveAsync(cacheKey);
                userInfo.Email = "gbemsy@gmail.com";
                userInfo.StatusCode = "00";
                serializedCustomerList = JsonConvert.SerializeObject(userInfo);
                redisCustomerList = Encoding.UTF8.GetBytes(serializedCustomerList);
                var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(30))
                .SetSlidingExpiration(TimeSpan.FromMinutes(12));
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
