using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Services.QrCode;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
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


                                var QueryAccountRequestDto = new QueryAccountRequestDto();
                                var NewCreateNibsMerchantRequestDto = new NewCreateNibsMerchantRequestDto();
                                var UserSterlingBankInfo = await context.MerchantBankInfo.
                                    Where(x => x.BankCode == _appSettings.SterlingBankCode).
                                    Where(x => x.ClientAuthenticationId == item.ClientAuthenticationId).
                                    SingleOrDefaultAsync();
                                if (UserSterlingBankInfo == null)
                                {
                                    var OthersterSterlingBankInfo = await context.OtherMerchantBankInfo.
                                       Where(x => x.ClientAuthenticationId == item.ClientAuthenticationId).
                                       Where(x => x.BankCode == _appSettings.SterlingBankCode).
                                       SingleOrDefaultAsync();

                                    if (OthersterSterlingBankInfo == null)
                                    {
                                        user.LastDateModified = DateTime.Now;
                                        context.Update(user);
                                        await context.SaveChangesAsync();

                                        await transaction.CommitAsync();
                                        continue;

                                    }
                                    QueryAccountRequestDto.accountNumber = OthersterSterlingBankInfo.Nuban;
                                    NewCreateNibsMerchantRequestDto.accountName = OthersterSterlingBankInfo.AccountName;

                                }
                                else
                                {
                                    QueryAccountRequestDto.accountNumber = UserSterlingBankInfo.Nuban;
                                    NewCreateNibsMerchantRequestDto.accountName = UserSterlingBankInfo.AccountName;

                                }

                                //QueryAccountRequestDto.accountNumber = "7060564377";
                                //NewCreateNibsMerchantRequestDto.accountName = "NNONA CHIMDIKE";

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




                                QueryAccountRequestDto.bankNumber = _appSettings.nibsQRCodeBankNumber;
                                NewCreateNibsMerchantRequestDto.accountNumber = QueryAccountRequestDto.accountNumber;
                                NewCreateNibsMerchantRequestDto.tin = merchant.Tin;
                                NewCreateNibsMerchantRequestDto.name = NewCreateNibsMerchantRequestDto.accountName;
                                NewCreateNibsMerchantRequestDto.phone = merchant.Phone;
                                NewCreateNibsMerchantRequestDto.contact = item.Contact;
                                NewCreateNibsMerchantRequestDto.email = merchant.Email;
                                NewCreateNibsMerchantRequestDto.address = merchant.Address;
                                NewCreateNibsMerchantRequestDto.feeBearer = "0";
                                NewCreateNibsMerchantRequestDto.bankCode = _appSettings.nibsQRCodeBankNumber;

                                var defaultRequest = new createMerchantRequestPayload
                                {
                                    NewCreateNibsMerchantRequestDto = NewCreateNibsMerchantRequestDto,
                                    QueryAccountRequestDto = QueryAccountRequestDto
                                };

                                var createNibbsMerchant = await _nibbsQRCodeAPIJobService.CreateMerchant(defaultRequest);

                                var merchantResponseLog = new MerchantQRCodeOnboardingResponse();

                                await context.MerchantQRCodeOnboarding.AddAsync(merchant);
                                await context.SaveChangesAsync();

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

                                else
                                {
                                    ///  merchantResponseLog.JsonResponse = createNibbsMerchant.jsonResponse;

                                    //await context.MerchantQRCodeOnboardingResponse.AddAsync(merchantResponseLog);
                                    //await context.SaveChangesAsync();

                                    // await transaction.CommitAsync();

                                    user.LastDateModified = DateTime.Now;
                                    context.Update(user);
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
