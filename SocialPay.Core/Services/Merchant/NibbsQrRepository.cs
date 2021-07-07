using SocialPay.Core.Services.QrCode;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Merchant
{
    public class NibbsQrRepository
    {
        private readonly SocialPayDbContext _context;
        private readonly NibbsQRCodeAPIService _nibbsQRCodeAPIService;
        public NibbsQrRepository(SocialPayDbContext context, NibbsQRCodeAPIService nibbsQRCodeAPIService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _nibbsQRCodeAPIService = nibbsQRCodeAPIService ?? throw new ArgumentNullException(nameof(nibbsQRCodeAPIService));
        }

        public async Task<WebApiResponse> CreateMerchantAsync(NibbsQrMerchantViewModel model, long clientId)
        {
            try
            {
                using(var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var merchant = new MerchantQRCodeOnboarding
                        {
                            IsDeleted = false,
                            Address = model.Address,
                            ClientAuthenticationId = clientId,
                            Contact = model.Contact,
                            Email = model.Email,
                            Fee = model.Fee,
                            Name = model.Name,
                            Phone = model.Phone,
                            Tin = model.Tin                            
                        };

                        await _context.MerchantQRCodeOnboarding.AddAsync(merchant);
                        await _context.SaveChangesAsync();

                        var defaultRequest = new CreateNibsMerchantRequestDto
                        {
                            Address = model.Address,
                            Contact = model.Contact,
                            Email = model.Email,
                            Fee = 00,
                            Name = model.Name,
                            Phone = model.Phone,
                            Tin = model.Tin
                        };

                        var createNibbsMerchant = await _nibbsQRCodeAPIService.CreateMerchant(defaultRequest);

                        var merchantResponseLog = new MerchantQRCodeOnboardingResponse();

                        merchantResponseLog.MerchantQRCodeOnboardingId = merchant.MerchantQRCodeOnboardingId;

                        if (createNibbsMerchant.ResponseCode == AppResponseCodes.Success)
                        {
                            merchantResponseLog.MchNo = createNibbsMerchant.mchNo;
                            merchantResponseLog.MerchantAddress = createNibbsMerchant.merchantAddress;
                            merchantResponseLog.MerchantContactName = createNibbsMerchant.merchantContactName;
                            merchantResponseLog.MerchantEmail = createNibbsMerchant.merchantEmail;
                            merchantResponseLog.MerchantPhoneNumber = createNibbsMerchant.merchantPhoneNumber;
                            merchantResponseLog.ReturnCode = createNibbsMerchant.returnCode;
                            merchantResponseLog.ReturnMsg = createNibbsMerchant.returnMsg;

                            await _context.MerchantQRCodeOnboardingResponse.AddAsync(merchantResponseLog);
                            await _context.SaveChangesAsync();

                            await transaction.CommitAsync();

                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Merchant was successfully created" };
                        }

                        merchantResponseLog.JsonResponse = createNibbsMerchant.jsonResponse;

                        await _context.MerchantQRCodeOnboardingResponse.AddAsync(merchantResponseLog);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Data = "Failed creating merchant" };

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


        public async Task<WebApiResponse> CreateSubMerchantAsync(NibbsSubMerchantViewModel model, long clientId)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var merchant = new SubMerchantQRCodeOnboarding
                        {
                            IsDeleted = false,
                            MerchantEmail = model.merchantEmail,
                            MchNo = model.mchNo,
                            MerchantName = model.merchantName,
                            MerchantPhoneNumber = model.merchantPhoneNumber,
                            SubAmount = model.subAmount,
                            SubFixed = model.subFixed,
                            MerchantQRCodeOnboardingId = model.MerchantQRCodeOnboardingId
                        };

                        await _context.SubMerchantQRCodeOnboarding.AddAsync(merchant);
                        await _context.SaveChangesAsync();

                        var defaultRequest = new CreateNibbsSubMerchantDto
                        {
                          
                        };

                        var createNibbsMerchant = await _nibbsQRCodeAPIService.CreateSubMerchant(defaultRequest);

                        var merchantResponseLog = new SubMerchantQRCodeOnboardingResponse();

                        merchantResponseLog.SubMerchantQRCodeOnboardingId = merchant.SubMerchantQRCodeOnboardingId;

                        if (createNibbsMerchant.ResponseCode == AppResponseCodes.Success)
                        {
                            merchantResponseLog.MchNo = createNibbsMerchant.mchNo;
                            //merchantResponseLog.MerchantAddress = createNibbsMerchant.merchantAddress;
                            //merchantResponseLog.MerchantContactName = createNibbsMerchant.merchantContactName;
                            //merchantResponseLog.MerchantEmail = createNibbsMerchant.merchantEmail;
                            //merchantResponseLog.MerchantPhoneNumber = createNibbsMerchant.merchantPhoneNumber;
                            //merchantResponseLog.ReturnCode = createNibbsMerchant.returnCode;
                            //merchantResponseLog.ReturnMsg = createNibbsMerchant.returnMsg;

                            await _context.SubMerchantQRCodeOnboardingResponse.AddAsync(merchantResponseLog);
                            await _context.SaveChangesAsync();

                            await transaction.CommitAsync();

                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Merchant was successfully created" };
                        }

                        merchantResponseLog.JsonResponse = createNibbsMerchant.jsonResponse;

                        await _context.SubMerchantQRCodeOnboardingResponse.AddAsync(merchantResponseLog);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Data = "Failed creating merchant" };

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
