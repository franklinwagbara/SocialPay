using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Services.Validations;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using SocialPay.Job.Repository.Fiorano;
using SocialPay.Job.Repository.InterBankService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NonEscrowBankTransactions
{
    public class NonEscrowPendingBankTransaction
    {
        private readonly AppSettings _appSettings;
        private readonly FioranoTransferNonEscrowRepository _fioranoTransferRepository;
        private readonly InterBankPendingTransferService _interBankPendingTransferService;
        private readonly BankServiceRepositoryJobService _bankServiceRepositoryJobService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(NonEscrowPendingBankTransaction));

        public NonEscrowPendingBankTransaction(IServiceProvider service, IOptions<AppSettings> appSettings,
             FioranoTransferNonEscrowRepository fioranoTransferRepository,
         InterBankPendingTransferService interBankPendingTransferService,
         BankServiceRepositoryJobService bankServiceRepositoryJobService)
        {
            Services = service;
            _appSettings = appSettings.Value;
            _fioranoTransferRepository = fioranoTransferRepository;
            _interBankPendingTransferService = interBankPendingTransferService;
            _bankServiceRepositoryJobService = bankServiceRepositoryJobService;
        }
        public IServiceProvider Services { get; }


        public async Task<WebApiResponse> ProcessTransactions(List<TransactionLog> pendingRequest)
        {
            long transactionLogid = 0;

            try
            {
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();
                    foreach (var item in pendingRequest)
                    {
                        _log4net.Info("Job Service" + "-" + "NonEscrowPendingBankTransaction request" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " + DateTime.Now);

                        var validateNuban = await _bankServiceRepositoryJobService.GetAccountFullInfoAsync(_appSettings.socialT24AccountNo, item.TotalAmount);

                        if (validateNuban.ResponseCode == AppResponseCodes.Success)
                        {
                            var requestId = Guid.NewGuid().ToString();
                            var getTransInfo = await context.TransactionLog
                             .SingleOrDefaultAsync(x => x.TransactionLogId == item.TransactionLogId
                             && x.TransactionJourney == TransactionJourneyStatusCodes.WalletTranferCompleted);
                            if (getTransInfo == null)
                                return null;

                            getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.BankTransferProcessing;
                            getTransInfo.LastDateModified = DateTime.Now;
                            context.Update(getTransInfo);
                            await context.SaveChangesAsync();

                            transactionLogid = getTransInfo.TransactionLogId;

                            string bankCode = string.Empty;

                            var getBankInfo = await context.MerchantBankInfo
                               .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);
                           
                            if (getBankInfo == null)
                                return null;

                            if (getBankInfo.BankCode == _appSettings.SterlingBankCode)
                            {
                                bankCode = getBankInfo.BankCode;

                                var initiateRequest = await _fioranoTransferRepository
                                   .InititiateMerchantCredit(Convert.ToString(getTransInfo.TotalAmount),
                                   "Credit Merchant Sterling Acc" + " - " + item.TransactionReference +
                                   " - " + item.PaymentReference, item.TransactionReference,
                                   getBankInfo.Nuban, item.PaymentChannel, "Intra-Bank Transfer",
                                   item.PaymentReference);

                                if (initiateRequest.ResponseCode == AppResponseCodes.Success)
                                {
                                    getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.TransactionCompleted;
                                    getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.TransactionCompleted;
                                    getTransInfo.LastDateModified = DateTime.Now;
                                    context.Update(getTransInfo);
                                    await context.SaveChangesAsync();
                                    _log4net.Info("Job Service" + "-" + "NonEscrowPendingBankTransaction response" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " + DateTime.Now);

                                    return null;
                                }

                                getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.TransactionFailed;
                                getTransInfo.LastDateModified = DateTime.Now;
                                context.Update(getTransInfo);
                                await context.SaveChangesAsync();

                                var failedTransaction = new FailedTransactions
                                {
                                    CustomerTransactionReference = item.CustomerTransactionReference,
                                    Message = initiateRequest.Message,
                                    TransactionReference = item.TransactionReference
                                };
                                await context.FailedTransactions.AddAsync(failedTransaction);
                                await context.SaveChangesAsync();

                                _log4net.Info("Job Service" + "-" + "NonEscrowPendingBankTransaction failed response" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " + DateTime.Now);

                                return null;
                            }

                            _log4net.Info("Job Service" + "-" + "NonEscrowPendingBankTransaction inter bank request" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " + DateTime.Now);


                            var initiateInterBankRequest = await _interBankPendingTransferService.ProcessInterBankTransactions(getBankInfo.Nuban, item.TotalAmount,
                                getBankInfo.BankCode, _appSettings.socialT24AccountNo, item.ClientAuthenticationId,
                                item.PaymentReference, item.TransactionReference);
                            _log4net.Info("Job Service" + "-" + "NonEscrowPendingBankTransaction inter bank response" + " | " + initiateInterBankRequest.ResponseCode + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " + DateTime.Now);

                            if (initiateInterBankRequest.ResponseCode == AppResponseCodes.Success)
                            {
                                getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.TransactionCompleted;
                                getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.TransactionCompleted;
                                getTransInfo.LastDateModified = DateTime.Now;
                                context.Update(getTransInfo);
                                await context.SaveChangesAsync();
                                return null;
                            }

                            var failedResponse = new FailedTransactions
                            {
                                CustomerTransactionReference = item.CustomerTransactionReference,
                                Message = initiateInterBankRequest.Data.ToString(),
                                TransactionReference = item.TransactionReference
                            };
                            await context.FailedTransactions.AddAsync(failedResponse);
                            await context.SaveChangesAsync();
                            _log4net.Info("Job Service" + "-" + "NonEscrowPendingBankTransaction inter bank response failed" + " | " + initiateInterBankRequest.ResponseCode + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " + DateTime.Now);

                            return null;
                        }

                        else
                        {

                            var failedResponse = new FailedTransactions
                            {
                                CustomerTransactionReference = item.CustomerTransactionReference,
                                Message = "Name enquiry failed" + "-" + validateNuban.UsableBal + "-" + item.TotalAmount + "-" + validateNuban.ResponseCode + "-" + item.PaymentReference,
                                TransactionReference = item.TransactionReference
                            };
                            await context.FailedTransactions.AddAsync(failedResponse);
                            await context.SaveChangesAsync();
                        }

                    }

                    //Other banks transfer
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }

            }
            catch (Exception ex)
            {
                _log4net.Error("Job Service" + "-" + "Base Error occured" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                var se = ex.InnerException as SqlException;
                var code = se.Number;
                var errorMessage = se.Message;
                if (errorMessage.Contains("Violation") || code == 2627)
                {
                   
                    _log4net.Error("An error occured. Duplicate transaction reference" + " | " + transactionLogid + " | " + errorMessage + " | "+ ex.Message.ToString() + " | " + DateTime.Now);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
