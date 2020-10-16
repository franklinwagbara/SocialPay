using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Repositories.Customer;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Transaction
{
    public class MerchantPaymentLinkService
    {
        private readonly SocialPayDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly Utilities _utilities;
        private readonly ICustomerService _customerService;
        private readonly IHostingEnvironment _hostingEnvironment;
        public MerchantPaymentLinkService(SocialPayDbContext context, IOptions<AppSettings> appSettings,
            Utilities utilities, ICustomerService customerService, IHostingEnvironment environment)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _utilities = utilities;
            _customerService = customerService;
            _hostingEnvironment = environment;
        }

        public async Task<WebApiResponse> GeneratePaymentLink(MerchantpaymentLinkRequestDto paymentModel,
            long clientId, string userStatus)
        {
            try
            {
                //clientId = 10014;

                decimal addtionalAmount = 0;
                if (userStatus != AppResponseCodes.Success)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.IncompleteMerchantProfile };
                if(paymentModel.PaymentCategory == MerchantPaymentCategory.Basic 
                    || paymentModel.PaymentCategory == MerchantPaymentCategory.Escrow
                    || paymentModel.PaymentCategory == MerchantPaymentCategory.OneOffBasicLink
                    || paymentModel.PaymentCategory == MerchantPaymentCategory.OneOffEscrowLink)
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
                    if(paymentModel.PaymentCategory == MerchantPaymentCategory.Escrow ||
                        paymentModel.PaymentCategory == MerchantPaymentCategory.OneOffEscrowLink)
                    {
                        addtionalAmount = model.TotalAmount * Convert.ToInt32(_appSettings.PaymentLinkPercentage) / 100;
                        model.TotalAmount = model.TotalAmount + addtionalAmount;
                        model.AdditionalCharges = addtionalAmount;
                        model.HasAdditionalCharges = true;
                    }
                    if(paymentModel.PaymentCategory == MerchantPaymentCategory.Escrow ||
                        paymentModel.PaymentCategory == MerchantPaymentCategory.Basic)
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
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = model.PaymentLinkUrl };

                    }
                    await _context.MerchantPaymentSetup.AddAsync(model);
                    await _context.SaveChangesAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = model.PaymentLinkUrl };
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
                // clientId = 10013;
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
                // clientId = 10013;
                var result = await _customerService.GetCustomerByMerchantId(clientId);
                return result;
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GenerateInvoice(InvoiceRequestDto invoiceRequestDto, long clientId)
        {
            try
            {
                if (await _context.InvoicePaymentLink.AnyAsync(x => x.InvoiceName == invoiceRequestDto.InvoiceName))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateInvoiceName };
                var model = new InvoicePaymentLink
                {
                    TransactionStatus = false, ClientAuthenticationId = clientId, CustomerEmail = invoiceRequestDto.CustomerEmail,
                    DueDate = Convert.ToDateTime(invoiceRequestDto.DueDate), InvoiceName = invoiceRequestDto.InvoiceName,
                    Qty = invoiceRequestDto.Qty, UnitPrice = invoiceRequestDto.UnitPrice, TransactionReference = Guid.NewGuid().ToString(),
                    TotalAmount = invoiceRequestDto.Qty * invoiceRequestDto.UnitPrice + invoiceRequestDto.ShippingFee,
                    ShippingFee = invoiceRequestDto.ShippingFee, Description = invoiceRequestDto.Description
                };

                await _context.InvoicePaymentLink.AddAsync(model);
                await _context.SaveChangesAsync();
                //send mail
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
