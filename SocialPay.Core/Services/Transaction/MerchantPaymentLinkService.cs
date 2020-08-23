using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Transaction
{
    public class MerchantPaymentLinkService
    {
        private readonly SocialPayDbContext _context;
        public MerchantPaymentLinkService(SocialPayDbContext context)
        {
            _context = context;
        }

        public async Task<WebApiResponse> GeneratePaymentLink(MerchantpaymentLinkRequestDto paymentModel, long clientId)
        {
            try
            {
                var model = new MerchantPaymentSetup { };
                model.PaymentLinkName = paymentModel.PaymentLinkName;
                model.AdditionalDetails = paymentModel.AdditionalDetails;
                model.Description = paymentModel.Description;
                model.Amount = paymentModel.Amount;
                model.CustomUrl = paymentModel.CustomUrl;
                model.RedirectAfterPayment = paymentModel.RedirectAfterPayment;
                model.DeliveryMethod = paymentModel.DeliveryMethod;
                model.ClientAuthenticationId = clientId;

                if(paymentModel.PaymentCategory == MerchantPaymentCategory.Basic)
                {
                    await _context.MerchantPaymentSetup.AddAsync(model);
                    await _context.SaveChangesAsync();
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }
                model.DeliveryTime = paymentModel.DeliveryTime;
                model.PaymentMethod = paymentModel.PaymentMethod;
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
