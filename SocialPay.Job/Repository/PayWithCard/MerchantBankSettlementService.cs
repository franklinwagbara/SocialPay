using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Core.Services.Validations;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.PayWithCardJob;
using SocialPay.Job.Repository.Fiorano;
using SocialPay.Job.Repository.InterBankService;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.PayWithCard
{
    public class MerchantBankSettlementService
    {
        private readonly AppSettings _appSettings;
        private readonly FioranoTransferNonEscrowRepository _fioranoTransferRepository;
        private readonly InterBankPendingTransferService _interBankPendingTransferService;
        private readonly BankServiceRepositoryJobService _bankServiceRepositoryJobService;
        private readonly PayWithCardJobLogger _paywithcardjobLogger;
        private readonly HttpClient _client;
        public MerchantBankSettlementService(IServiceProvider service, IOptions<AppSettings> appSettings,
             FioranoTransferNonEscrowRepository fioranoTransferRepository,
         InterBankPendingTransferService interBankPendingTransferService, PayWithCardJobLogger paywithcardjobLogger,
         BankServiceRepositoryJobService bankServiceRepositoryJobService)
        {
            Services = service;
            _appSettings = appSettings.Value;
            _fioranoTransferRepository = fioranoTransferRepository;
            _interBankPendingTransferService = interBankPendingTransferService;
            _bankServiceRepositoryJobService = bankServiceRepositoryJobService;
            _paywithcardjobLogger = paywithcardjobLogger;

            _client = new HttpClient
            {
                BaseAddress = new Uri("https://ipg-requery-dev.sterlingapps.p.azurewebsites.net/"),
            };

        }
        public IServiceProvider Services { get; }

        private async Task<bool> TransactionVerification(String interSwitchResponse, Decimal amount)
        {
            try
            {

                _paywithcardjobLogger.LogRequest($"{"Verification of transaction from interswitch transaction ref" + " | " + interSwitchResponse + " | " + " |  Amount" + amount + " | "}{DateTime.Now}", true);

                string[] transactionResponse = interSwitchResponse.Split("^");
                var transactionRef = transactionResponse[transactionResponse.Length - 1];

                var path = "TransactionRequery";
                var payload = new
                {
                    transactionReference = transactionRef,
                    amount = amount

                };
                var request = JsonConvert.SerializeObject(payload);
                _paywithcardjobLogger.LogRequest($"{"Verification of transaction from interswitch transaction ref" + " | " + interSwitchResponse + " | " + " |  request body" + request + " | "}{DateTime.Now}", true);

                //Log
                var response = await _client.PostAsync(path,
                    new StringContent(request, Encoding.UTF8, "application/json"));

                var result = await response.Content.ReadAsStringAsync();
                var successfulResponse = JsonConvert.DeserializeObject<InterswitchTransactionVerificationDTO>(result);

                _paywithcardjobLogger.LogRequest($"{"response from verification of transaction from interswitch transaction ref" + " | " + interSwitchResponse + " | " + " |  request body" + request + " | response "+ JsonConvert.SerializeObject(successfulResponse)}{DateTime.Now}", true);

                if (successfulResponse.responseCode == "00") return true;

                return false;

            }
            catch(Exception ex)
            {
                _paywithcardjobLogger.LogRequest($"{"An error occured" + " | " + interSwitchResponse + " | " +  " | " + ex.Message.ToString() + " | "}{DateTime.Now}", true);

                return false;
            }
        }

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
                        _paywithcardjobLogger.LogRequest($"{"Job Service" + "-" + "MerchantBankSettlementService Pending Bank Transaction request" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

                        var getTransInfo = await context.TransactionLog
                            .SingleOrDefaultAsync(x => x.TransactionLogId == item.TransactionLogId
                            && x.TransactionJourney == TransactionJourneyStatusCodes.Approved);

                        if (getTransInfo == null)
                            return null;



                        if (! await TransactionVerification(item.Message, item.TotalAmount))
                        {
                            getTransInfo.TransactionStatus = TransactionJourneyStatusCodes.TransactionNotVerified;
                            getTransInfo.LastDateModified = DateTime.Now;
                            context.Update(getTransInfo);
                            await context.SaveChangesAsync();
                            return null;
                        }
                        
                        
                        
                        var validateNuban = await _bankServiceRepositoryJobService.GetAccountFullInfoAsync(_appSettings.socialT24AccountNo, item.TotalAmount);

                        var requestId = Guid.NewGuid().ToString();



                        getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.Pending;
                        getTransInfo.LastDateModified = DateTime.Now;
                        context.Update(getTransInfo);
                        await context.SaveChangesAsync();

                        transactionLogid = getTransInfo.TransactionLogId;

                        var getBankInfo = await context.MerchantBankInfo
                            .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);

                        if (getBankInfo == null)
                        {
                        _paywithcardjobLogger.LogRequest($"{"Job Service" + "-" + "MerchantBankSettlementService PendingBank Transaction Bank info is null" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

                            return null;
                        }

                        //For test purpose
                        ////getBankInfo.BankCode = "000014";
                        ////getBankInfo.Nuban = "0025998012";
                        ////item.TotalAmount = 300;

                        if (getBankInfo.BankCode == _appSettings.SterlingBankCode)
                        {

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
                                getTransInfo.TransactionStatus = TransactionJourneyStatusCodes.TransactionCompleted;
                                getTransInfo.LastDateModified = DateTime.Now;
                                context.Update(getTransInfo);

                                await context.SaveChangesAsync();
                            _paywithcardjobLogger.LogRequest($"{"Job Service" + "-" + "MerchantBankSettlementServicePendingBankTransaction response" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

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

                        _paywithcardjobLogger.LogRequest($"{"Job Service" + "-" + "MerchantBankSettlementServicePendingBankTransaction failed response" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

                            return null;
                        }

                    _paywithcardjobLogger.LogRequest($"{"Job Service" + "-" + "MerchantBankSettlementService PendingBankTransaction inter bank request" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

                    var initiateInterBankRequest = await _interBankPendingTransferService.ProcessInterBankTransactions(getBankInfo.Nuban, item.TotalAmount,
                            getBankInfo.BankCode, _appSettings.socialT24AccountNo, item.ClientAuthenticationId,
                            item.PaymentReference, item.TransactionReference);
                    _paywithcardjobLogger.LogRequest($"{"Job Service" + "-" + "MerchantBankSettlementService PendingBankTransaction inter bank response" + " | " + initiateInterBankRequest.ResponseCode + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

                    if (initiateInterBankRequest.ResponseCode == AppResponseCodes.Success)
                    {
                        getTransInfo.TransactionJourney = TransactionJourneyStatusCodes.TransactionCompleted;
                        getTransInfo.ActivityStatus = TransactionJourneyStatusCodes.TransactionCompleted;
                        getTransInfo.TransactionStatus = TransactionJourneyStatusCodes.TransactionCompleted;
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
                    _paywithcardjobLogger.LogRequest($"{"Job Service" + "-" + "MerchantBankSettlementService PendingBankTransaction inter bank response failed" + " | " + initiateInterBankRequest.ResponseCode + " | " + item.PaymentReference + " | " + item.TransactionReference + " | "}{DateTime.Now}", false);

                    return null;

                    } 

                    //Other banks transfer
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }

            }
            catch (Exception ex)
            {
                _paywithcardjobLogger.LogRequest($"{"Job Service" + "-" + "Base Error occured for MerchantBankSettlementService" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", true);

                var se = ex.InnerException as SqlException;
                var code = se.Number;
                var errorMessage = se.Message;
                if (errorMessage.Contains("Violation") || code == 2627)
                {
                    _paywithcardjobLogger.LogRequest($"{"An error occured. Duplicate transaction reference" + " | " + transactionLogid + " | " + errorMessage + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", true);

                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }






    }
}
