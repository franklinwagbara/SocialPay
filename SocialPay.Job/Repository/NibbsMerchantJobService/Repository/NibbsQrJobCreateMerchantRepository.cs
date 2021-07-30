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
    public class NibbsQrJobCreateMerchantRepository
    {
        private readonly AppSettings _appSettings;
        private readonly NibbsQRCodeAPIJobService _nibbsQRCodeAPIJobService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(NibbsQrJobCreateMerchantRepository));
        public NibbsQrJobCreateMerchantRepository(IOptions<AppSettings> appSettings, IServiceProvider service,
            NibbsQRCodeAPIJobService nibbsQRCodeAPIJobService)
        {
            _appSettings = appSettings.Value;
            Services = service;
            _nibbsQRCodeAPIJobService = nibbsQRCodeAPIJobService ?? throw new ArgumentNullException(nameof(nibbsQRCodeAPIJobService));
        }
        public IServiceProvider Services { get; }

        public async Task ProcessTransactions(List<NibbsQrMerchantViewModel> pendingRequest)
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
                                var user = await context.ClientAuthentication
                                .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);

                                var merchant = new MerchantQRCodeOnboarding
                                {
                                    IsDeleted = false,
                                    Address = item.Address,
                                    ClientAuthenticationId = item.ClientAuthenticationId,
                                    Contact = item.Contact,
                                    Email = item.Email,
                                    Fee = item.Fee,
                                    Name = item.Name,
                                    Phone = item.Phone,
                                    Tin = item.Tin,
                                    IsCompleted = false,
                                    Status = NibbsMerchantOnboarding.CreateAccount
                                };

                                await context.MerchantQRCodeOnboarding.AddAsync(merchant);
                                await context.SaveChangesAsync();

                                var defaultRequest = new CreateNibsMerchantRequestDto
                                {
                                    Address = merchant.Address,
                                    Contact = merchant.Contact,
                                    Email = merchant.Email,
                                    Fee = 00,
                                    Name = merchant.Name,
                                    Phone = merchant.Phone,
                                    Tin = merchant.Tin
                                };

                                var createNibbsMerchant = await _nibbsQRCodeAPIJobService.CreateMerchant(defaultRequest);

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

                                    await context.MerchantQRCodeOnboardingResponse.AddAsync(merchantResponseLog);
                                    await context.SaveChangesAsync();

                                    user.QrCodeStatus = NibbsMerchantOnboarding.CreateAccount;
                                    user.LastDateModified = DateTime.Now;
                                    context.Update(user);
                                    await context.SaveChangesAsync();

                                    await transaction.CommitAsync();

                                    //return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Merchant was successfully created" };
                                }

                                merchantResponseLog.JsonResponse = createNibbsMerchant.jsonResponse;

                                await context.MerchantQRCodeOnboardingResponse.AddAsync(merchantResponseLog);
                                await context.SaveChangesAsync();

                                await transaction.CommitAsync();
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
