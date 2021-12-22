using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Core.Configurations;
using SocialPay.Core.Services.Fiorano;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Loan
{
    public class ApplyForLoanService
    {

        private readonly SocialPayDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly LoanEligibiltyService _loanEligibiltyService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(ApplyForLoanService));
        private readonly HttpClient _client;
        private readonly HttpClient _cardTokenizationClient;
        private readonly FioranoAPIService _fioranoService;
        private readonly IFioranoRequestService _fioranoRequestService;
        private readonly IFioranoResponseService _fioranoResponseService;
        public ApplyForLoanService(SocialPayDbContext context, IOptions<AppSettings> appSettings, LoanEligibiltyService loanEligibiltyService,
            FioranoAPIService fioranoService, IFioranoRequestService fioranoRequestService,
            IFioranoResponseService fioranoResponseService
            )

        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _appSettings = appSettings.Value;
            _loanEligibiltyService = loanEligibiltyService;
            _fioranoService = fioranoService ?? throw new ArgumentNullException(nameof(fioranoService));
            _fioranoRequestService = fioranoRequestService ?? throw new ArgumentNullException(nameof(fioranoRequestService));
            _fioranoResponseService = fioranoResponseService ?? throw new ArgumentNullException(nameof(fioranoResponseService));
            _client = new HttpClient
            {
                BaseAddress = new Uri(_appSettings.CreditBureauSearchBaseURL),
            };


            _cardTokenizationClient = new HttpClient
            {
                BaseAddress = new Uri(_appSettings.TokenizationBaseURL),
            };
            _cardTokenizationClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + _appSettings.CardTokenizationAuthorizationToken);
            _cardTokenizationClient.DefaultRequestHeaders.Add("channel", "specta");
            _cardTokenizationClient.DefaultRequestHeaders.Add("tokentype", "paystack");
        }


        public async Task<WebApiResponse> ApplyForLoan(ApplyForloanRequestDTO model, long clientId)
        {
            clientId = 172;

            try
            {
                bool IsSterlingAccountNumber = false;
                //Verification of eligibility
                var getCustomerEligibility = await _loanEligibiltyService.MerchantEligibilty(clientId);
                if (Convert.ToDecimal(getCustomerEligibility.Data) < model.Amount) return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "User is eligible to this amount of loan (N " + getCustomerEligibility.Data + ") as Maximum." , StatusCode = ResponseCodes.Badrequest };

                bool cheeckForOpenloan = await _context.ApplyForLoan
                .Where(x => x.ClientAuthenticationId == clientId)
                .AnyAsync(x => x.IsAttended == false);

                //////if (cheeckForOpenloan) return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "User has an oustanding loan" };

                //////if (!await _context.LoanRepaymentPlan.Where(x => x.IsDeleted == false).AnyAsync(x => x.LoanRepaymentPlanId == model.LoanRepaymentPlanId)) return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Invalid LoanRepaymentPlanId ", StatusCode = ResponseCodes.Badrequest };

                //var getUser = await _context.MerchantWallet.SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);
                var getclient = await _context.ClientAuthentication
                    .Include(m => m.MerchantWallet).SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);

                //Verification of credibility
                var MerchantCredibility = await creditworthiness(getclient.MerchantWallet.Select(x=>x.Firstname).FirstOrDefault(),
                    getclient.MerchantWallet.Select(x => x.Lastname).FirstOrDefault(), getclient.Bvn,
                    getclient.MerchantWallet.Select(x => x.DoB).FirstOrDefault(),
                    getclient.MerchantWallet.Select(x => x.Mobile).FirstOrDefault(), 
                    getclient.Email, clientId);

                //card tokenization
                var tokenizeCard = await CardTokenization(getclient.FullName, getclient.Bvn,
                    getclient.MerchantWallet.Select(x => x.DoB).FirstOrDefault(),
                   getclient.MerchantWallet.Select(x => x.Mobile).FirstOrDefault(), 
                   getclient.Email, clientId, model.redirectUrl);

                //check if merchant have a sterling bank account

                //////////////var merchantBankDetails = await _context.MerchantBankInfo.SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);

                //////////////if (merchantBankDetails != null)
                //////////////{
                //////////////    if (merchantBankDetails.BankCode == _appSettings.SterlingBankCode) IsSterlingAccountNumber = true;
                //////////////}


                ////////////////Check if merchant have a sterling bank business account

                //////////////var dbPayload = new ApplyForLoan
                //////////////{
                //////////////    ClientAuthenticationId = clientId,
                //////////////    LoanRepaymentPlanId = model.LoanRepaymentPlanId,
                //////////////    Amount = model.Amount,
                //////////////    IsAttended = false,
                //////////////    IsApproved = false,
                //////////////    IsBadDebt = false,
                //////////////    isCustomerClean = MerchantCredibility,
                //////////////    IsCardTokenized = false,
                //////////////    HaveSterlingBankAccount = IsSterlingAccountNumber,
                //////////////    HaveSterlingBankBusinessAccount = false

                //////////////};

                //////////////await _context.ApplyForLoan.AddAsync(dbPayload);
                //////////////await _context.SaveChangesAsync();

                //////////////var responsePayload = new ApplyForLoanResponseDTO
                //////////////{
                //////////////    ApplyForLoanId = dbPayload.ApplyForLoanId,
                //////////////    RedirectUrl = tokenizeCard.redirectUrl
                //////////////};

               //// return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = responsePayload, Message = "Complete the process by tokenizing your card", StatusCode = ResponseCodes.Success };
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = tokenizeCard, Message = "Complete the process by tokenizing your card", StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured", StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> ConfirmTokenization(ConfirmTokenizationRequestDTO model, long clientId)
        {
            try
            {
                var message = "Card is not tokenized";
                var GetLoanDetails = await _context.ApplyForLoan.
                    Where(x => x.IsCardTokenized == false).
                    SingleOrDefaultAsync(x => x.ApplyForLoanId == model.ApplyForLoanId);

                if (GetLoanDetails == null) return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Invalid ApplyForLoanId", StatusCode = ResponseCodes.Badrequest };

                var response = await _cardTokenizationClient.GetAsync("VerifyTransaction?reference=" + model.Ref);
                
                if (!response.IsSuccessStatusCode)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Invalid Ref", StatusCode = ResponseCodes.Badrequest };

                var result = await response.Content.ReadAsStringAsync();
                var successfulResponse = JsonConvert.DeserializeObject<ConfirmTokenizationResponseDTO>(result);

                if (!successfulResponse.status)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Invalid Ref", StatusCode = ResponseCodes.Badrequest };

                GetLoanDetails.IsCardTokenized = successfulResponse.status;
                GetLoanDetails.TokenizationToken = successfulResponse.data.CardDetails.CardToken;
                GetLoanDetails.TokenizationReference = successfulResponse.data.CardDetails.TokenizationReference;
                GetLoanDetails.TokenizationEmail = successfulResponse.data.EmailAddress;
                GetLoanDetails.ConfirmTokenizationResponse = result;

                _context.ApplyForLoan.Update(GetLoanDetails);
                await _context.SaveChangesAsync();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = message , StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured", StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> ApproveLoan(AdminLoanApproverRequestDTO model)
        {

            try
            {

                var message = "";
                //Update the Apply loan db

                var GetLoanDetails = await _context.ApplyForLoan.
                    Where(x => x.IsAttended == true).
                    SingleOrDefaultAsync(x => x.ApplyForLoanId == model.ApplyForLoanId);

                if (GetLoanDetails == null) return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Invalid ApplyForLoanId", StatusCode = ResponseCodes.Badrequest };
                if (!GetLoanDetails.IsCardTokenized) return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Merchant is not tokenized", StatusCode = ResponseCodes.Badrequest };
                if (!model.IsApproved)
                {
                    GetLoanDetails.IsApproved = model.IsApproved;
                    GetLoanDetails.IsAttended = true;
                    _context.ApplyForLoan.Update(GetLoanDetails);
                    await _context.SaveChangesAsync();

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Loan is cancelled ", StatusCode = ResponseCodes.Success };
                }


                using (var transaction = await _context.Database.BeginTransactionAsync())
                {

                    try
                    {

                        //Call the api to send the loan
                        var bankInfo = await _context.MerchantBankInfo.SingleOrDefaultAsync(x => x.ClientAuthenticationId == GetLoanDetails.ClientAuthenticationId);
                      
                        if (bankInfo == default)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Banking info not found", StatusCode = ResponseCodes.RecordNotFound };

                        if (bankInfo.BankCode != _appSettings.SterlingBankCode)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Merchant bank must be Sterling bank to complete this request", StatusCode = ResponseCodes.RecordNotFound };
                      
                        var transactionId = Guid.NewGuid().ToString();

                        // bankInfo.Nuban = "0065428109";


                        var dbDisbursementPayload = new LoanDisbursement
                        {
                            TransactionReference = transactionId,
                            ClientAuthenticationId = GetLoanDetails.ClientAuthenticationId,
                            ApplyForLoanId = model.ApplyForLoanId,
                            disbusedAmount = model.ApprovedAmount,
                            BankCode = bankInfo.BankCode,
                            nuban = bankInfo.Nuban,
                            HaveStartedRepayment = false,
                            status = false
                        };
                        await _context.LoanDisbursement.AddAsync(dbDisbursementPayload);
                        await _context.SaveChangesAsync();


                        var ftModel = new FTRequest
                        {
                            SessionId = transactionId,
                            CommissionCode = _appSettings.fioranoCommisionCode,
                            CreditCurrency = _appSettings.fioranoCreditCurrency,
                            DebitCurrency = _appSettings.fioranoCreditCurrency,
                            VtellerAppID = _appSettings.fioranoVtellerAppID,
                            TrxnLocation = _appSettings.fioranoTrxnLocation,
                            TransactionType = _appSettings.fioranoTransactionType,
                            DebitAcctNo = _appSettings.socialT24AccountNo,
                            TransactionBranch = "NG0020006",
                            narrations = "Processed loan for socialpay merchant",
                            DebitAmount = Convert.ToString(model.ApprovedAmount),
                            CreditAccountNo = bankInfo.Nuban,
                        };

                        var request = new TransactionRequestDto { FT_Request = ftModel };

                        var jsonRequest = JsonConvert.SerializeObject(request);

                        var creditMerchant = await _fioranoService.InitiateTransaction(jsonRequest);


                        if (creditMerchant.ResponseCode == AppResponseCodes.Success)
                        {

                            dbDisbursementPayload.status = true;
                            _context.LoanDisbursement.Update(dbDisbursementPayload);
                            await _context.SaveChangesAsync();

                            GetLoanDetails.IsApproved = model.IsApproved;
                            GetLoanDetails.IsAttended = true;
                            _context.ApplyForLoan.Update(GetLoanDetails);
                            await _context.SaveChangesAsync();
                            message = "Successful disbursement";
                        }
                        else
                        {
                            dbDisbursementPayload.status = false;
                            _context.LoanDisbursement.Update(dbDisbursementPayload);
                            await _context.SaveChangesAsync();
                            message = "Disbursement failed";
                        }

                        await transaction.CommitAsync();
                        return new WebApiResponse { ResponseCode = creditMerchant.ResponseCode, Message = message, StatusCode = ResponseCodes.Success };

                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError , StatusCode = ResponseCodes.InternalError };
                    }

                }
              
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured", StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> LoanStatus(long ApplyForLoanId, long clientId)
        {

            try
            {

                var appliedLoan = await _context.ApplyForLoan.
                    Where(x => x.ClientAuthenticationId == clientId).
                    SingleOrDefaultAsync(x => x.ApplyForLoanId == ApplyForLoanId);
               
                if (appliedLoan == null) return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Invalid ApplyForLoanId", StatusCode = ResponseCodes.Badrequest };
              
                var payloadLoanStatus = new AppliedLoanStatus
                {
                    Amount = appliedLoan.Amount,
                    IsAttended = appliedLoan.IsAttended,
                    IsApproved = appliedLoan.IsApproved,
                    IsBadDebt = appliedLoan.IsBadDebt,
                    isCustomerClean = appliedLoan.isCustomerClean,
                    IsCardTokenized = appliedLoan.IsCardTokenized
                };
              
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = payloadLoanStatus, StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured", StatusCode = ResponseCodes.InternalError };
            }
        }
        public async Task<WebApiResponse> GetAllAppliedLoan()
        {

            try
            {

                var appliedLoan = await _context.ApplyForLoan
                    .Include(x => x.LoanRepaymentPlan)
                    .Select(x => new
                    {
                        x.Amount,
                        x.ApplyForLoanId,
                        x.ClientAuthenticationId,
                        x.HaveSterlingBankAccount,
                        x.HaveSterlingBankBusinessAccount,
                        x.IsAttended,
                        x.IsApproved,
                        x.IsBadDebt,
                        x.IsCardTokenized,
                        x.isCustomerClean,
                        PA = x.LoanRepaymentPlan.PA,
                        Rate = x.LoanRepaymentPlan.PA,
                        x.LoanRepaymentPlanId,
                        x.DateEntered,
                        x.LoanRepaymentPlan.DailySalesPercentage

                    }).ToListAsync();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = appliedLoan, StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured", StatusCode = ResponseCodes.InternalError };
            }
        }

        private async Task<bool> creditworthiness(string firstName, string lastName, string bvn, string dateOfBirth, string phoneNumber, string email, long clientId)
        {

            try
            {
                // return true;
                var payload = new CreditBureauSearchDTO
                {
                    firstName = firstName,
                    lastName = lastName,
                    bvn = bvn,
                    dateOfBirth = DateTime.Parse(dateOfBirth).ToString("yyyy-MM-dd"),
                    email = email,
                    address = "",
                    phoneNumber = phoneNumber,
                    gender = 0

                };

                var request = JsonConvert.SerializeObject(payload);
                //  _log4net.Info("GatewayRequery" + " | " + payload.TransactionID + " | " + clientId + " | " + request + " | " + DateTime.Now);
                var response = await _client.PostAsync(_appSettings.CreditBureauSearch_Crc,
                    new StringContent(request, Encoding.UTF8, "application/json"));

                var result = await response.Content.ReadAsStringAsync();
                //_log4net.Info("Initiate GatewayRequery response" + " | " + model.TransactionID + " | " + payload.amount + " | " + result + " | " + clientId + " | " + DateTime.Now);
                var successfulResponse = JsonConvert.DeserializeObject<CreditBureauSearchResponseDTO>(result);


                var payloadCreditBureauSearch = new MerchantBureauSearch
                {
                    ClientAuthenticationId = clientId,
                    isCustomerClean = successfulResponse.creditSearchResponse.isCustomerClean,
                    response = result
                };

                await _context.MerchantBureauSearch.AddAsync(payloadCreditBureauSearch);
                await _context.SaveChangesAsync();

                return successfulResponse.creditSearchResponse.isCustomerClean;

            }
            catch (Exception e)
            {
                return false;
            }
        }

        private async Task<CradTokenizationInClassResponse> CardTokenization(string fullname, string bvn, string dateOfBirth, string phoneNumber, string email, long clientId, string redirectUrl)
        {
            try
            {
                // DateTime sDate = DateTime.Parse(dateOfBirth);
                // var dob = Convert.ToDateTime(dateOfBirth).ToString("yyyy-MM-dd");

                // var dCon = Convert.ToDateTime(dateOfBirth);

                DateTime date = DateTime.Parse(dateOfBirth);

                var DateObject2 = Convert.ToDateTime(dateOfBirth);

                //DateTime DateObject1 = DateTime.ParseExact(dateOfBirth, "dd/MM/yyyy", null);

                //string curDate = DateObject1.ToString("yyyy-MM-dd");

                //var Culture = new CultureInfo("en-US");
                //Use of Convert.ToDateTime() 
                //DateTime DateObject = Convert.ToDateTime("12-12-1992", Culture);
                // var pDate = DateTime.ParseExact("05/28/2013 12:00:00 AM", "MM/dd/yyyy hh:mm:ss", CultureInfo.InvariantCulture);
                //var dob = DateTime.ParseExact(dateOfBirth, "yyyy-MM-dd", CultureInfo.InvariantCulture);
               //var dob = Convert.ToDateTime(dateOfBirth);

                // var date = DateTime.ParseExact(dateOfBirth, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                // var dd = dob.ToString("yyyy-MM-dd");

                var successfulResponse = new CradTokenizationResponseDTO();

                var payload = new CardTokenizationRequestDTO
                {
                    fullName = fullname,
                    email = email,
                    phone = phoneNumber,
                   // dob = dateOfBirth,
                    dob = "1992-12-14",
                   // dob = DateTime.ParseExact(dateOfBirth, "yyyy/M/dd", null).ToString("yyyy-MM-dd"),
                    // dob = DateTime.Parse(dateOfBirth).ToString("yyyy-MM-dd"),
                    tokenType = "",
                    channel = "",
                    cardMinExpiryInMonths = "8",
                    redirectUrl = redirectUrl,
                    bvn = bvn
                };


                var request = JsonConvert.SerializeObject(payload);
                var response = await _cardTokenizationClient.PostAsync(_appSettings.TokenizationURL,
                   new StringContent(request, Encoding.UTF8, "application/json"));

                var result = await response.Content.ReadAsStringAsync();


                if (response.IsSuccessStatusCode)
                {
                    successfulResponse = JsonConvert.DeserializeObject<CradTokenizationResponseDTO>(result);

                    //Save to db

                    var dbPayload = new CardTokenization
                    {
                        ClientAuthenticationId = clientId,
                        fullName = payload.fullName,
                        email = payload.email,
                        phone = payload.phone,
                        dob = payload.dob,
                        tokenType = payload.tokenType,
                        channel = payload.channel,
                        cardMinExpiryInMonths = payload.cardMinExpiryInMonths,
                        redirectUrl = payload.redirectUrl,
                        bvn = payload.bvn,
                        reference = successfulResponse.data.reference,
                        message = successfulResponse.message,
                        status = successfulResponse.status,
                        responseUrl = successfulResponse.data.url
                    };

                    //await _context.CardTokenization.AddAsync(dbPayload);
                    //await _context.SaveChangesAsync();

                    return new CradTokenizationInClassResponse
                    {
                        status = successfulResponse.status,
                        redirectUrl = successfulResponse.data.url
                    };
                    //End save to db
                }

                return new CradTokenizationInClassResponse
                {
                    status = successfulResponse.status,
                    redirectUrl = result
                };
            }
            catch (Exception ex)
            {
                //var error = ex;
                return new CradTokenizationInClassResponse
                {
                    status = false,
                    redirectUrl = ex.ToString()
                };
            }
        }

    }

}
