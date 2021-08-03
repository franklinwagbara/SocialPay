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
    public class BindMerchantServiceRepository
    {
        private readonly AppSettings _appSettings;
        private readonly NibbsQRCodeAPIJobService _nibbsQRCodeAPIJobService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(BindMerchantServiceRepository));
        public BindMerchantServiceRepository(IOptions<AppSettings> appSettings, IServiceProvider service,
            NibbsQRCodeAPIJobService nibbsQRCodeAPIJobService)
        {
            _appSettings = appSettings.Value;
            Services = service;
            _nibbsQRCodeAPIJobService = nibbsQRCodeAPIJobService ?? throw new ArgumentNullException(nameof(nibbsQRCodeAPIJobService));
        }
        public IServiceProvider Services { get; }
        public async Task ProcessTransactions(List<ClientAuthentication> pendingRequest)
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

                                var merchantInfo = await context.MerchantQRCodeOnboarding
                                    .Include(x => x.MerchantQRCodeOnboardingResponse)
                                   .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);

                                var getMerchantBankInfo = await context.MerchantBankInfo
                                    .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);

                                if (merchantInfo != null)
                                {
                                   
                                    var nibbsMerchantInfo = await context.MerchantQRCodeOnboarding
                                        .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);

                                    var model = new BindMerchantRequestDto();

                                    model.accountName = "OGUNLANA TUNJI";
                                    model.accountNumber = "0122047425";
                                    model.bankNo = "999058";
                                    model.mchNo = "M0000000105";

                                    var bindMerchant = new BindMerchant
                                    {
                                        AccountName = model.accountName,
                                        BankNo = model.bankNo,
                                        AccountNumber = model.accountNumber,
                                        MchNo = model.mchNo,
                                        MerchantQRCodeOnboardingId = nibbsMerchantInfo.MerchantQRCodeOnboardingId,
                                    };

                                    await context.BindMerchant.AddAsync(bindMerchant);
                                    await context.SaveChangesAsync();

                                    var bindNibbsMerchant = await _nibbsQRCodeAPIJobService.BindMerchant(model);

                                    var merchantResponseLog = new BindMerchantResponse();

                                    merchantResponseLog.BindMerchantId = bindMerchant.BindMerchantId;                                    

                                    if (bindNibbsMerchant.ResponseCode == AppResponseCodes.Success)
                                    {
                                        merchantResponseLog.Mch_no = bindNibbsMerchant.Mch_no;
                                        merchantResponseLog.ReturnCode = bindNibbsMerchant.ReturnCode;

                                        await context.BindMerchantResponse.AddAsync(merchantResponseLog);
                                        await context.SaveChangesAsync();

                                        nibbsMerchantInfo.IsCompleted = true;
                                        nibbsMerchantInfo.Status = NibbsMerchantOnboarding.BindMerchant;

                                        context.Update(nibbsMerchantInfo);
                                        await context.SaveChangesAsync();

                                        var user = await context.ClientAuthentication
                                        .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);

                                        user.QrCodeStatus = NibbsMerchantOnboarding.BindMerchant;
                                        user.LastDateModified = DateTime.Now;
                                        context.Update(user);
                                        await context.SaveChangesAsync();

                                        await transaction.CommitAsync();

                                    }
                                    else
                                    {
                                        //merchantResponseLog.JsonResponse = bindNibbsMerchant.jsonResponse;

                                        //await context.BindMerchantResponse.AddAsync(merchantResponseLog);
                                        //await context.SaveChangesAsync();

                                        //await transaction.CommitAsync();
                                    }                                   

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
