using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Repositories.Customer;
using SocialPay.Core.Repositories.Invoice;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Transaction
{
    public class MerchantPaymentLinkService
    {
        private readonly SocialPayDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly Utilities _utilities;
        private readonly ICustomerService _customerService;
        private readonly InvoiceService _invoiceService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IDistributedCache _distributedCache;
        public MerchantPaymentLinkService(SocialPayDbContext context, IOptions<AppSettings> appSettings,
            Utilities utilities, ICustomerService customerService, IHostingEnvironment environment,
            InvoiceService invoiceService, IDistributedCache distributedCache)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _utilities = utilities;
            _customerService = customerService;
            _hostingEnvironment = environment;
            _invoiceService = invoiceService;
            _distributedCache = distributedCache;
        }

        public async Task<WebApiResponse> GeneratePaymentLink(MerchantpaymentLinkRequestDto paymentModel,
            long clientId)
        {
            try
            {
                //clientId = 30032;
                //userStatus = "00";
                decimal addtionalAmount = 0;
                var cacheKey = Convert.ToString(clientId);
                string serializedCustomerList;
                string userStatus = string.Empty;
                var userInfo = new UserInfoViewModel { };
                var redisCustomerList = await _distributedCache.GetAsync(cacheKey);
                if (redisCustomerList != null)
                {
                    serializedCustomerList = Encoding.UTF8.GetString(redisCustomerList);
                    var result = JsonConvert.DeserializeObject<UserInfoViewModel>(serializedCustomerList);
                    userStatus = result.StatusCode;
                }
                if (userStatus != AppResponseCodes.Success)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.IncompleteMerchantProfile };
                if(paymentModel.PaymentCategory == MerchantPaymentLinkCategory.Basic 
                    || paymentModel.PaymentCategory == MerchantPaymentLinkCategory.Escrow
                    || paymentModel.PaymentCategory == MerchantPaymentLinkCategory.OneOffBasicLink
                    || paymentModel.PaymentCategory == MerchantPaymentLinkCategory.OneOffEscrowLink)
                {
                    var model = new MerchantPaymentSetup { };
                   
                    model.PaymentLinkName = paymentModel.PaymentLinkName == null ? string.Empty : paymentModel.PaymentLinkName;
                    model.AdditionalDetails = paymentModel.AdditionalDetails == null ? string.Empty : paymentModel.AdditionalDetails;
                    model.MerchantDescription = paymentModel.MerchantDescription == null ? string.Empty : paymentModel.MerchantDescription;
                    model.MerchantAmount = paymentModel.MerchantAmount < 1 ? 0 : paymentModel.MerchantAmount;
                    model.CustomUrl = paymentModel.CustomUrl == null ? string.Empty : paymentModel.CustomUrl;
                    model.RedirectAfterPayment = paymentModel.RedirectAfterPayment == false ? false : paymentModel.RedirectAfterPayment;
                    model.DeliveryMethod = paymentModel.DeliveryMethod == null ? string.Empty : paymentModel.DeliveryMethod;
                    model.ClientAuthenticationId = clientId;
                    model.PaymentCategory = paymentModel.PaymentCategory == null ? string.Empty : paymentModel.PaymentCategory;
                    model.ShippingFee = paymentModel.ShippingFee < 1 ? 0 : paymentModel.ShippingFee;
                    model.TotalAmount = model.MerchantAmount + model.ShippingFee;
                    model.DeliveryTime = paymentModel.DeliveryTime < 1 ? 0 : paymentModel.DeliveryTime;
                    model.PaymentMethod = paymentModel.PaymentMethod == null ? string.Empty : paymentModel.PaymentMethod;
                    var newGuid = Guid.NewGuid().ToString("N");
                    var token = model.MerchantAmount + "," + model.PaymentCategory + "," + model.PaymentLinkName + "," + newGuid;
                    var encryptedToken = token.Encrypt(_appSettings.appKey);
                    //var newPin = Utilities.GeneratePin();// + DateTime.Now.Day;
                    //var encryptedPin = newPin.Encrypt(_appSettings.appKey);
                    //byte[] HashReference, SaltReference;
                    //_utilities.CreatePasswordHash(encryptedPin, out HashReference, out SaltReference);
                    if (await _context.MerchantPaymentSetup.AnyAsync(x => x.TransactionReference == newGuid))
                    {
                        newGuid = Guid.NewGuid().ToString("N");
                        token = string.Empty;
                        encryptedToken = string.Empty;
                        token = model.MerchantAmount + model.PaymentCategory + model.PaymentLinkName + newGuid;
                        encryptedToken = token.Encrypt(_appSettings.appKey);
                        //newPin = string.Empty;
                        //newPin = Utilities.GeneratePin();
                        //encryptedPin = newPin.Encrypt(_appSettings.appKey);
                    }
                   
                    model.TransactionReference = newGuid;
                    model.PaymentLinkUrl = _appSettings.paymentlinkUrl + model.TransactionReference;
                    if(paymentModel.PaymentCategory == MerchantPaymentLinkCategory.Escrow ||
                        paymentModel.PaymentCategory == MerchantPaymentLinkCategory.OneOffEscrowLink)
                    {
                        addtionalAmount = model.TotalAmount * Convert.ToInt32(_appSettings.PaymentLinkPercentage) / 100;
                        model.TotalAmount = model.TotalAmount + addtionalAmount;
                        model.AdditionalCharges = addtionalAmount;
                        model.HasAdditionalCharges = true;
                    }

                    var linkCatModel = new LinkCategory
                    {
                        ClientAuthenticationId = clientId, Channel = paymentModel.PaymentCategory,
                        TransactionReference = newGuid
                    };
                    using(var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            if (paymentModel.PaymentCategory == MerchantPaymentLinkCategory.Escrow ||
                                paymentModel.PaymentCategory == MerchantPaymentLinkCategory.Basic)
                            {
                                string path = Path.Combine(this._hostingEnvironment.WebRootPath, _appSettings.MerchantLinkPaymentDocument);
                                if (!Directory.Exists(path))
                                {
                                    Directory.CreateDirectory(path);
                                }
                                string fileName = string.Empty;
                                var newFileName = string.Empty;
                                fileName = (paymentModel.Document.FileName);
                                var documentId = Guid.NewGuid().ToString("N").Substring(18);
                                var FileExtension = Path.GetExtension(fileName);
                                fileName = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.MerchantLinkPaymentDocument) + $@"\{newFileName}";

                                // concating  FileName + FileExtension
                                newFileName = documentId + FileExtension;
                                var filePath = Path.Combine(fileName, newFileName);
                                model.Document = newFileName;
                                model.FileLocation = _appSettings.MerchantLinkPaymentDocument;
                                await _context.MerchantPaymentSetup.AddAsync(model);
                                paymentModel.Document.CopyTo(new FileStream(filePath, FileMode.Create));
                                await _context.SaveChangesAsync();
                                await _context.LinkCategory.AddAsync(linkCatModel);
                                await _context.SaveChangesAsync();
                                await transaction.CommitAsync();
                                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = model.PaymentLinkUrl };

                            }
                            await _context.MerchantPaymentSetup.AddAsync(model);
                            await _context.SaveChangesAsync();
                            await _context.LinkCategory.AddAsync(linkCatModel);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = model.PaymentLinkUrl };
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError};
                        }
                      
                    }

                  
                    ////if (paymentModel.PaymentCategory == MerchantPaymentCategory.Basic)
                    ////{

                    ////}

                    ////model.DeliveryTime = paymentModel.DeliveryTime;
                    ////model.PaymentMethod = paymentModel.PaymentMethod;
                    ////await _context.MerchantPaymentSetup.AddAsync(model);
                    ////await _context.SaveChangesAsync();
                    ////return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = model.PaymentLinkUrl };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidPaymentcategory };
               
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetAllPaymentLinksByMerchant(long clientId)
        {
            try
            {
               // clientId = 10014;
                var getlinks = await _customerService.GetPaymentLinks(clientId);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = getlinks };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetCustomerPayments(long clientId)
        {
            try
            {
                // clientId = 40072;
                var result = await _customerService.GetCustomerPaymentsByMerchantPayRef(clientId);
                return result;
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetCustomers(long clientId)
        {
            try
            {
                 //clientId = 40072;
                var result = await _customerService.GetCustomerByMerchantId(clientId);
                return result;
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GenerateInvoice(InvoiceRequestDto invoiceRequestDto, long clientId,
            string businessName)
        {
            try
            {

               // clientId = 30043;
                if (await _context.InvoicePaymentLink.AnyAsync(x => x.InvoiceName == invoiceRequestDto.InvoiceName))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateInvoiceName };
                var transactionReference = Guid.NewGuid().ToString();
                var model = new InvoicePaymentLink
                {
                    TransactionStatus = false, ClientAuthenticationId = clientId, CustomerEmail = invoiceRequestDto.CustomerEmail,
                    DueDate = Convert.ToDateTime(invoiceRequestDto.DueDate), InvoiceName = invoiceRequestDto.InvoiceName,
                    Qty = invoiceRequestDto.Qty, UnitPrice = invoiceRequestDto.UnitPrice, TransactionReference = transactionReference,
                    TotalAmount = invoiceRequestDto.Qty * invoiceRequestDto.UnitPrice + invoiceRequestDto.ShippingFee,
                    ShippingFee = invoiceRequestDto.ShippingFee, Description = invoiceRequestDto.Description
                };
                using(var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        await _context.InvoicePaymentLink.AddAsync(model);
                        await _context.SaveChangesAsync();
                        var linkCatModel = new LinkCategory
                        {
                            ClientAuthenticationId = clientId,
                            Channel = MerchantPaymentLinkCategory.InvoiceLink,
                            TransactionReference = transactionReference
                        };
                        await _context.LinkCategory.AddAsync(linkCatModel);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        await _invoiceService.SendInvoiceAsync(invoiceRequestDto.CustomerEmail,
                            invoiceRequestDto.UnitPrice, model.TotalAmount, model.DateEntered, invoiceRequestDto.InvoiceName,
                            model.TransactionReference
                            );
                        //send mail
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                    }
                }
                
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
