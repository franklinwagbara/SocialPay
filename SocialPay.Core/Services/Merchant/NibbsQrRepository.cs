using Microsoft.EntityFrameworkCore;
using SocialPay.Core.Services.QrCode;
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
                            Tin = model.Tin,
                            IsCompleted = false,
                            Status = NibbsMerchantOnboarding.CreateAccount
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
                            merchantResponseLog.MerchantName = createNibbsMerchant.merchantName;
                            merchantResponseLog.MerchantTIN = createNibbsMerchant.merchantTIN;

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
                          mchNo = model.mchNo,
                          merchantEmail = model.merchantEmail,
                          merchantName = model.merchantName,
                          merchantPhoneNumber = model.merchantPhoneNumber,
                          subAmount = model.subAmount,
                          subFixed = model.subFixed
                        };

                        var createNibbsSubMerchant = await _nibbsQRCodeAPIService.CreateSubMerchant(defaultRequest);

                        var merchantResponseLog = new SubMerchantQRCodeOnboardingResponse();

                        merchantResponseLog.SubMerchantQRCodeOnboardingId = merchant.SubMerchantQRCodeOnboardingId;

                        if (createNibbsSubMerchant.ResponseCode == AppResponseCodes.Success)
                        {
                            merchantResponseLog.MchNo = createNibbsSubMerchant.mchNo;
                            merchantResponseLog.MerchantName = createNibbsSubMerchant.merchantName;
                            merchantResponseLog.QrCode = createNibbsSubMerchant.qrCode;
                            merchantResponseLog.ReturnCode = createNibbsSubMerchant.returnCode;
                            merchantResponseLog.ReturnMsg = createNibbsSubMerchant.returnMsg;
                            merchantResponseLog.SubMchNo = createNibbsSubMerchant.subMchNo;                            
                            merchantResponseLog.IsDeleted = false;                         

                            await _context.SubMerchantQRCodeOnboardingResponse.AddAsync(merchantResponseLog);
                            await _context.SaveChangesAsync();

                            await transaction.CommitAsync();

                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Merchant was successfully created" };
                        }

                        merchantResponseLog.JsonResponse = createNibbsSubMerchant.jsonResponse;

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

        public async Task<WebApiResponse> BindMerchantAync(long clientId)
        {
            try
            {

                var model = new BindMerchantRequestDto();

                var nibbsMerchantInfo = await _context.MerchantQRCodeOnboarding
                    .Include(x=>x.MerchantQRCodeOnboardingResponse)
                   .SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);

                //var nibbsMerchantInfo = await _context.MerchantQRCodeOnboardingResponse
                //    .SingleOrDefaultAsync(x => x.MerchantQRCodeOnboardingId == merchantId);

                var getMerchantBankInfo = await _context.MerchantBankInfo.SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);

                //model.accountName = getMerchantBankInfo.AccountName;
                //model.accountNumber = getMerchantBankInfo.Nuban;
                //model.bankNo = getMerchantBankInfo.BankCode;
                //model.mchNo = nibbsMerchantInfo.MerchantQRCodeOnboardingResponse.Select(x=>x.MchNo).FirstOrDefault();


                model.accountName = "OGUNLANA TUNJI";
                model.accountNumber = "0122047425";
                model.bankNo = "999058";
                model.mchNo = "M0000000105";

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var bindMerchant = new BindMerchant
                        {
                            AccountName = model.accountName,
                            BankNo = model.bankNo,
                            AccountNumber = model.accountNumber,
                            MchNo = model.mchNo,
                            MerchantQRCodeOnboardingId = nibbsMerchantInfo.MerchantQRCodeOnboardingId,
                        };

                        //requestModel.mchNo = "M0000000105";
                        //requestModel.bankNo = "999058";
                        //requestModel.accountNumber = "0122047425";
                        //requestModel.accountName = "OGUNLANA TUNJI";

                        await _context.BindMerchant.AddAsync(bindMerchant);
                        await _context.SaveChangesAsync();

                        var bindNibbsMerchant = await _nibbsQRCodeAPIService.BindMerchant(model);

                        var merchantResponseLog = new BindMerchantResponse();

                        merchantResponseLog.BindMerchantId = bindMerchant.BindMerchantId;

                        if (bindNibbsMerchant.ResponseCode == AppResponseCodes.Success)
                        {
                            merchantResponseLog.Mch_no = bindNibbsMerchant.Mch_no;
                            merchantResponseLog.ReturnCode = bindNibbsMerchant.ReturnCode;

                            await _context.BindMerchantResponse.AddAsync(merchantResponseLog);
                            await _context.SaveChangesAsync();

                            nibbsMerchantInfo.IsCompleted = true;
                            nibbsMerchantInfo.Status = NibbsMerchantOnboarding.BindMerchant;

                            _context.Update(nibbsMerchantInfo);
                            await _context.SaveChangesAsync();

                            await transaction.CommitAsync();

                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Bind merchant was successfully created" };
                        }

                        merchantResponseLog.JsonResponse = bindNibbsMerchant.jsonResponse;

                        await _context.BindMerchantResponse.AddAsync(merchantResponseLog);
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
