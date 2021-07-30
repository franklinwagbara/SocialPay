using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
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

namespace SocialPay.Job.Repository.NibbsMerchantJobService.Repository
{
    public class NibbsQrJobCreateSubMerchantRepository
    {
        private readonly AppSettings _appSettings;
        private readonly NibbsQRCodeAPIJobService _nibbsQRCodeAPIJobService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(NibbsQrJobCreateSubMerchantRepository));
        public NibbsQrJobCreateSubMerchantRepository(IOptions<AppSettings> appSettings, IServiceProvider service,
            NibbsQRCodeAPIJobService nibbsQRCodeAPIJobService)
        {
            _appSettings = appSettings.Value;
            Services = service;
            _nibbsQRCodeAPIJobService = nibbsQRCodeAPIJobService ?? throw new ArgumentNullException(nameof(nibbsQRCodeAPIJobService));
        }
        public IServiceProvider Services { get; }

        public async Task ProcessTransactions(List<MerchantQRCodeOnboarding> pendingRequest)
        {
            long transactionLogid = 0;

            try
            {
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();

                    foreach (var item in pendingRequest)
                    {
                        //_log4net.Info("Job Service" + "-" + "Non Escrow Pending Bank Transaction request" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " + DateTime.Now);
                        using (var transaction = await context.Database.BeginTransactionAsync())
                        {
                            try
                            {
                                var merchantResponseInfo = await context.MerchantQRCodeOnboardingResponse
                                    .SingleOrDefaultAsync(x => x.MerchantQRCodeOnboardingId == item.MerchantQRCodeOnboardingId);

                                if(merchantResponseInfo != null)
                                {
                                   
                                    var nibbsMerchantInfo = await context.MerchantQRCodeOnboarding
                                        .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);

                                    var merchant = new SubMerchantQRCodeOnboarding
                                    {
                                        IsDeleted = false,
                                        MerchantEmail = merchantResponseInfo.MerchantEmail,
                                        MchNo = merchantResponseInfo.MchNo,
                                        MerchantName = merchantResponseInfo.MerchantName,
                                        MerchantPhoneNumber = merchantResponseInfo.MerchantPhoneNumber,
                                        SubAmount = "0.00",
                                        SubFixed = "0.00",
                                        MerchantQRCodeOnboardingId = merchantResponseInfo.MerchantQRCodeOnboardingId
                                    };

                                    await context.SubMerchantQRCodeOnboarding.AddAsync(merchant);
                                    await context.SaveChangesAsync();

                                    var defaultRequest = new CreateNibbsSubMerchantDto
                                    {
                                        mchNo = merchant.MchNo,
                                        merchantEmail = merchant.MerchantEmail,
                                        merchantName = merchant.MerchantName,
                                        merchantPhoneNumber = merchant.MerchantPhoneNumber,
                                        subAmount = merchant.SubAmount,
                                        subFixed = merchant.SubFixed
                                    };

                                    var createNibbsSubMerchant = await _nibbsQRCodeAPIJobService.CreateSubMerchant(defaultRequest);

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

                                        await context.SubMerchantQRCodeOnboardingResponse.AddAsync(merchantResponseLog);
                                        await context.SaveChangesAsync();

                                        nibbsMerchantInfo.Status = NibbsMerchantOnboarding.SubAccount;

                                        context.Update(nibbsMerchantInfo);
                                        await context.SaveChangesAsync();

                                        var user = await context.ClientAuthentication
                                        .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);

                                        user.QrCodeStatus = NibbsMerchantOnboarding.SubAccount;
                                        user.LastDateModified = DateTime.Now;
                                        context.Update(user);
                                        await context.SaveChangesAsync();

                                        await transaction.CommitAsync();

                                        //return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Merchant was successfully created" };
                                    }

                                    merchantResponseLog.JsonResponse = createNibbsSubMerchant.jsonResponse;

                                    await context.SubMerchantQRCodeOnboardingResponse.AddAsync(merchantResponseLog);
                                    await context.SaveChangesAsync();

                                    await transaction.CommitAsync();

                                }

                            }
                            catch (Exception ex)
                            {
                                await transaction.RollbackAsync();
                                throw;
                            }
                        }

                    }

                  //  return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }

            }
            catch (Exception ex)
            {
                _log4net.Error("Job Service" + "-" + "Base Error occured" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | " + DateTime.Now);

              //  return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
