using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Transaction
{
    public class MerchantPaymentLinkService
    {
        private readonly SocialPayDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly Utilities _utilities;
        public MerchantPaymentLinkService(SocialPayDbContext context, IOptions<AppSettings> appSettings,
            Utilities utilities)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _utilities = utilities;
        }

        public async Task<WebApiResponse> GeneratePaymentLink(MerchantpaymentLinkRequestDto paymentModel, long clientId)
        {
            try
            {
               // clientId = 10014;
                var model = new MerchantPaymentSetup { };
                model.PaymentLinkName = paymentModel.PaymentLinkName;
                model.AdditionalDetails = paymentModel.AdditionalDetails;
                model.Description = paymentModel.Description;
                model.Amount = paymentModel.Amount;
                model.CustomUrl = paymentModel.CustomUrl;
                model.RedirectAfterPayment = paymentModel.RedirectAfterPayment;
                model.DeliveryMethod = paymentModel.DeliveryMethod;
                model.ClientAuthenticationId = clientId;
                var newGuid = Guid.NewGuid().ToString("N");
                var token = model.Amount + "," + model.PaymentCategory +","+ model.PaymentLinkName +","+ newGuid;
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
                    token = model.Amount + model.PaymentCategory + model.PaymentLinkName + newGuid;
                    encryptedToken = token.Encrypt(_appSettings.appKey);
                    //newPin = string.Empty;
                    //newPin = Utilities.GeneratePin();
                    //encryptedPin = newPin.Encrypt(_appSettings.appKey);
                }
                model.TransactionReference = newGuid;
                model.TransactionToken = encryptedToken;
                if (paymentModel.PaymentCategory == MerchantPaymentCategory.Basic)
                {
                    await _context.MerchantPaymentSetup.AddAsync(model);
                    await _context.SaveChangesAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }
                model.DeliveryTime = paymentModel.DeliveryTime;
                model.PaymentMethod = paymentModel.PaymentMethod;
                await _context.MerchantPaymentSetup.AddAsync(model);
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
