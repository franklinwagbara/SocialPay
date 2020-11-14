using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Services.Data;
using SocialPay.Core.Services.IBS;
using SocialPay.Core.Services.Validations;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.InterBankService
{
    public class InterBankPendingTransferService
    {
        private readonly AppSettings _appSettings;
        private readonly BankServiceRepositoryJobService _bankServiceRepositoryJobService;
        private readonly IBSReposerviceJob _iBSReposerviceJob;
        private readonly SqlRepository _sqlRepository;
        public InterBankPendingTransferService(IServiceProvider service, IOptions<AppSettings> appSettings,
            BankServiceRepositoryJobService bankServiceRepositoryJobService,
            IBSReposerviceJob iBSReposerviceJob, SqlRepository sqlRepository)
        {
            Services = service;
            _appSettings = appSettings.Value;
            _bankServiceRepositoryJobService = bankServiceRepositoryJobService;
            _iBSReposerviceJob = iBSReposerviceJob;
            _sqlRepository = sqlRepository;
        }
        public IServiceProvider Services { get; }

        public async Task<WebApiResponse> ProcessTransactionsOld(List<TransactionLog> pendingRequest)
        {
            try
            {
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    foreach (var item in pendingRequest)
                    {
                        string bankCode = string.Empty;
                        var getBankInfo = await context.MerchantBankInfo
                           .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);
                        if (getBankInfo == null)
                            return null;

                        if (getBankInfo.BankCode == _appSettings.SterlingBankCode)
                        {
                            bankCode = getBankInfo.BankCode;
                            var getTransInfo = await context.TransactionLog
                           .SingleOrDefaultAsync(x => x.TransactionLogId == item.TransactionLogId);

                            getTransInfo.DeliveryDayTransferStatus = OrderStatusCode.WalletFundingProgress;
                            getTransInfo.LastDateModified = DateTime.Now;
                            context.Update(getTransInfo);
                            await context.SaveChangesAsync();

                            //////////var initiateRequest = await _fioranoTransferRepository
                            //////////   .InititiateDebit(Convert.ToString(getTransInfo.TotalAmount),
                            //////////   "Card-Payment" + " - " + item.TransactionReference +
                            //////////   " - " + item.CustomerTransactionReference, item.TransactionReference,
                            //////////   getBankInfo.Nuban, true);

                            //////////if (initiateRequest.ResponseCode == AppResponseCodes.Success)
                            //////////{
                            //////////    getTransInfo.DeliveryDayTransferStatus = OrderStatusCode.CompletedDirectFundTransfer;
                            //////////    getTransInfo.LastDateModified = DateTime.Now;
                            //////////    context.Update(getTransInfo);
                            //////////    await context.SaveChangesAsync();
                            //////////    return null;
                            //////////}

                            //////////getTransInfo.DeliveryDayTransferStatus = OrderStatusCode.Failed;
                            //////////getTransInfo.LastDateModified = DateTime.Now;
                            //////////context.Update(getTransInfo);
                            //////////await context.SaveChangesAsync();
                            //return null;
                        }

                        //Other banks transfer
                        //  return null;
                    }
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }

            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> ProcessTransactions(string destinationAccount, decimal amount,
            string desBankCode, string sourceAccount)
        {
            try
            {
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();

                    var nameEnquiryModel = new IBSNameEnquiryRequestDto
                    {
                        DestinationBankCode = desBankCode, ToAccount = destinationAccount,
                        RequestType = _appSettings.nameEnquiryRequestType, ReferenceID = Guid.NewGuid().ToString()
                    };
                  
                    var lockAccountModel = new LockAccountRequestDto
                    {
                        sDate = DateTime.Today, eDate = DateTime.Today.AddMinutes(10),
                        acct = sourceAccount, amt = amount, reasonForLocking ="Funds transfer"
                    };
                    var lockAccount = await _bankServiceRepositoryJobService.LockAccountWithReasonAsync(lockAccountModel);
                    //if (lockAccount.Contains(""))
                    //    return new WebApiResponse { ResponseCode = AppResponseCodes.AccountLockFailed }; 
                    var nipEnquiry = await _iBSReposerviceJob.InitiateNameEnquiry(nameEnquiryModel);
                    if(nipEnquiry.ResponseCode != AppResponseCodes.Success)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InterBankNameEnquiryFailed };
                    var nipRequestModel = new NipFundstransferRequestDto
                    {
                        BraCodeVal = _appSettings.socialT24Bracode, Amount = amount,
                        AppID = Convert.ToInt32(_appSettings.socialPayAppID), CurCodeVal = _appSettings.socialPayT24CurCode,
                        CusNumVal = _appSettings.socialPayT24CustomerNum, DestinationBankCode = desBankCode,
                        Fee = 10, Vat = 0.75, ChannelCode = "2", LedCodeVal =  _appSettings.socialPayT24CustomerLedCode,
                        NESessionID = "999377373673736", AccountName = nipEnquiry.AccountName, AccountNumber = destinationAccount,
                        BeneficiaryKYCLevel = nipEnquiry.KYCLevel, BeneficiaryBankVerificationNumber = nipEnquiry.BVN,
                        OriginatorAccountNumber = sourceAccount, OriginatorKYCLevel =nipEnquiry.KYCLevel,
                        OriginatorBankVerificationNumber = _appSettings.socialT24BVN, 
                        PaymentRef = "Social-Pay-Merchant-Payment" + ""+ Guid.NewGuid().ToString().Substring(0,6),
                        AccountLockID = lockAccount, OrignatorName = _appSettings.socialPayT24AccountName, SubAcctVal = "0"
                    };

                    var insertRequest = await _sqlRepository.InsertNipTransferRequest(nipRequestModel);

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }

            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
