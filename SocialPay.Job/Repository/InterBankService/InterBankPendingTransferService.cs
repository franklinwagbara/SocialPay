using Microsoft.Data.SqlClient;
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
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.InterBankService
{
    public class InterBankPendingTransferService
    {
        private readonly AppSettings _appSettings;
        private readonly BankServiceRepositoryJobService _bankServiceRepositoryJobService;
        private readonly IBSReposerviceJob _iBSReposerviceJob;
        private readonly SqlRepository _sqlRepository;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(InterBankPendingTransferService));

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


        public async Task<WebApiResponse> ProcessInterBankTransactions(string destinationAccount, decimal amount,
            string desBankCode, string sourceAccount, long clientId, 
            string paymentReference, string transactionReference)
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

                    var getFeesAndVat =  _sqlRepository.GetNIPFee(amount);
                    if (getFeesAndVat == null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.NipFeesCalculationFailed };

                    var nipRequestModel = new NipFundstransferRequestDto
                    {
                        BraCodeVal = _appSettings.socialT24Bracode, Amount = amount,
                        AppID = Convert.ToInt32(_appSettings.socialPayAppID), CurCodeVal = _appSettings.socialPayT24CurCode,
                        CusNumVal = _appSettings.socialPayT24CustomerNum, DestinationBankCode = desBankCode,
                        ChannelCode = "2", LedCodeVal =  _appSettings.socialPayT24CustomerLedCode,
                        NESessionID = nipEnquiry.SessionID, AccountName = nipEnquiry.AccountName, AccountNumber = destinationAccount,
                        //BeneficiaryKYCLevel = nipEnquiry.KYCLevel, BeneficiaryBankVerificationNumber = nipEnquiry.BVN,
                        BeneficiaryKYCLevel = nipEnquiry.KYCLevel, BeneficiaryBankVerificationNumber = "",
                        OriginatorAccountNumber = sourceAccount, OriginatorKYCLevel =nipEnquiry.KYCLevel,
                        OriginatorBankVerificationNumber = _appSettings.socialT24BVN, 
                        Fee = Convert.ToDecimal(getFeesAndVat.FeeAmount), Vat = Convert.ToDouble(getFeesAndVat.Vat),
                        PaymentRef = "Social-Pay-Merchant-Payment" + ""+ Guid.NewGuid().ToString().Substring(0,8),
                        AccountLockID = lockAccount, OrignatorName = _appSettings.socialPayT24AccountName, SubAcctVal = "0"
                    };

                    var logInterBankRequest = new InterBankTransactionRequest
                    {
                        DestinationBankCode = nipRequestModel.DestinationBankCode, AccountLockID = nipRequestModel.AccountLockID,
                        SubAcctVal = nipRequestModel.SubAcctVal, AccountName = nipRequestModel.AccountName,
                        NESessionID = nipRequestModel.NESessionID, AccountNumber = nipRequestModel.AccountNumber,
                        Amount = nipRequestModel.Amount, AppID = nipRequestModel.AppID,
                        BeneficiaryBankVerificationNumber = nipRequestModel.BeneficiaryBankVerificationNumber,
                        CusNumVal = nipRequestModel.CusNumVal, BeneficiaryKYCLevel = nipRequestModel.BeneficiaryKYCLevel,
                        BraCodeVal = nipRequestModel.BraCodeVal, ChannelCode = nipRequestModel.ChannelCode, 
                        ClientAuthenticationId = clientId, CurCodeVal = nipRequestModel.CurCodeVal,
                        Fee = nipRequestModel.Fee, Vat = Convert.ToDecimal(nipRequestModel.Vat), LedCodeVal = nipRequestModel.LedCodeVal,
                        OriginatorAccountNumber = nipRequestModel.OriginatorAccountNumber, 
                        OriginatorBankVerificationNumber = nipRequestModel.OriginatorBankVerificationNumber,
                        OriginatorKYCLevel = nipRequestModel.OriginatorKYCLevel, OrignatorName = nipRequestModel.OrignatorName,
                        PaymentRef = nipRequestModel.PaymentRef, TransactionReference = transactionReference,
                        PaymentReference = paymentReference
                    };

                    await context.InterBankTransactionRequest.AddAsync(logInterBankRequest);
                    await context.SaveChangesAsync();

                   return await _sqlRepository.InsertNipTransferRequest(nipRequestModel);
                }

            }
            catch (Exception ex)
            {
                _log4net.Error("An error occured. Base error" + " | " + paymentReference + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                var se = ex.InnerException as SqlException;
                var code = se.Number;
                var errorMessage = se.Message;
                if (errorMessage.Contains("Violation") || code == 2627)
                {
                    //using (var scope = Services.CreateScope())
                    //{
                    //    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    //    var getTransInfo = await context.TransactionLog
                    //      .SingleOrDefaultAsync(x => x.TransactionLogId == transactionLogid);

                    //    getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.TransactionCompleted;
                    //    getTransInfo.LastDateModified = DateTime.Now;
                    //    context.Update(getTransInfo);
                    //    await context.SaveChangesAsync();
                    //}

                    _log4net.Error("An error occured. Duplicate transaction reference" + " | " + paymentReference + " | " + errorMessage + " | "+ ex.Message.ToString() + " | " + DateTime.Now);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction, Data = errorMessage };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
