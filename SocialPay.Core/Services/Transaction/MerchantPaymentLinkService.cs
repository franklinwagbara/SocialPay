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
        public MerchantPaymentLinkService(SocialPayDbContext context, IOptions<AppSettings> appSettings,
            Utilities utilities, ICustomerService customerService)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _utilities = utilities;
            _customerService = customerService;
        }

        public async Task<WebApiResponse> GeneratePaymentLink(MerchantpaymentLinkRequestDto paymentModel,
            long clientId, string userStatus)
        {
            try
            {
                //clientId = 10014;
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
    }
}
