using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Services.PayU;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Threading.Tasks;
using SocialPay.Core.Services.AirtimeVending;
using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Helper.ViewModel;
using System.Linq;

namespace SocialPay.Core.Services.Bill
{
    public class BillService
    {

        private readonly SocialPayDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly DstvPaymentService _payWithPayUService;
        private readonly AirtimeVendingService _airtimeVendingService;
        private readonly IVendAirtimeRequestService _vendAirtimeRequestService;
        private readonly IMerchantBankingInfoService _merchantBankingInfoService;

        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(BillService));

        public BillService(SocialPayDbContext context, IOptions<AppSettings> appSettings, 
            DstvPaymentService payWithPayUService, AirtimeVendingService airtimeVendingService,
            IVendAirtimeRequestService vendAirtimeRequestService,
            IMerchantBankingInfoService merchantBankingInfoService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _appSettings = appSettings.Value;
            _payWithPayUService = payWithPayUService ?? throw new ArgumentNullException(nameof(payWithPayUService));
            _airtimeVendingService = airtimeVendingService ?? throw new ArgumentNullException(nameof(airtimeVendingService));
            _vendAirtimeRequestService = vendAirtimeRequestService ?? throw new ArgumentNullException(nameof(vendAirtimeRequestService));
            _merchantBankingInfoService = merchantBankingInfoService ?? throw new ArgumentNullException(nameof(merchantBankingInfoService));
        }

        public async Task<GetBillerResponseDto> GetBillersAsync(long clientId)
        {
            //clientId = 167;
            return await _payWithPayUService.GetDstvGotvBillers(clientId);
        }

        public async Task<WebApiResponse> GetNetworkProviders(long clientId)
        {
            return await _airtimeVendingService.NetworkProviders();
        }

        public async Task<WebApiResponse> GetAirtimeProducts(long clientId, int networkId)
        {
            return await _airtimeVendingService.GetNetworkProducts(networkId);
        }

        public async Task<WebApiResponse> VendAirtimeRequest(VendAirtimeDTO vendAirtimeDTO, long clientId)
        {
            var bankInfo = await _merchantBankingInfoService.GetMerchantBankInfo(clientId);
           
            if (bankInfo == null)
                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Banking info not found" };

            if (bankInfo.BankCode != _appSettings.SterlingBankCode)
                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Merchant bank must be Sterling bank to complete this request" };

            var generator = new Random();

            var referenceId = generator.Next(100000, 1000000).ToString() + "" + generator.Next(100000, 1000000).ToString();

            var model = new VendAirtimeViewModel
            {
                ClientAuthenticationId = clientId,
                ReferenceId = referenceId,
                RequestType = _appSettings.vtuRequestType,
                Translocation = "",
                Amount = Convert.ToDouble(vendAirtimeDTO.amt),
                Paymentcode = vendAirtimeDTO.paymentcode,
                Mobile = vendAirtimeDTO.mobile,
                email = _appSettings.vtuDefaultEmail,
                SubscriberInfo1 = vendAirtimeDTO.mobile,
                nuban = "0065428109",
               // nuban = bankInfo.Nuban,
                TransactionType = _appSettings.vtuTransactionType,
                AppId = _appSettings.vtuAppId,
                TerminalID = ""
            };

            await _vendAirtimeRequestService.AddAsync(model);
            
            return await _airtimeVendingService.AirtimeSubscription(model);

        }

        public async Task<DstvAccountLookupResponseDto> PayUAccountLookupPayment(string customerId, long clientId)
        {
           //  clientId = 167;

            var model = new DstvAccountLookupDto
            {
                customerId = customerId,
                countryCode = _appSettings.CountryCode,
                vasId = _appSettings.VasId,
                transactionType = _appSettings.AccountLookTransactionType
            };

            var accountlookdstv = new DstvAccountLookup
            {
                merchantReference = $"{"SP-"}{"B-"}{Guid.NewGuid().ToString()}",
                transactionType = model.transactionType,
                vasId = model.vasId,
                countryCode = model.countryCode,
                customerId = model.customerId,
                ClientAuthenticationId = clientId
            };

            if(await _context.DstvAccountLookup.AnyAsync(x=>x.merchantReference == accountlookdstv.merchantReference))
                return new DstvAccountLookupResponseDto { resultCode = AppResponseCodes.DuplicatePaymentReference, resultMessage = "Duplicate Payment Reference" };

            await _context.DstvAccountLookup.AddAsync(accountlookdstv);
            await _context.SaveChangesAsync();

            var postsingledstvbill = await _payWithPayUService.InitiatePayUDstvAccountLookupPayment(model);

            if (postsingledstvbill.resultCode != AppResponseCodes.Success)
                return postsingledstvbill;

          //  var response = (DstvAccountLookupResponseDto)postsingledstvbill.DataObj;

            var lookuppaymentresponse = new DstvAccountLookupResponse
            {
                resultMessage = postsingledstvbill.resultMessage,
                //pointOfFailure = response?.po,
                resultCode = postsingledstvbill.resultCode,
                payUVasReference = postsingledstvbill.payUVasReference,
                merchantReference = postsingledstvbill.merchantReference,
                DstvAccountLookupId = accountlookdstv.DstvAccountLookupId
            };

            await _context.DstvAccountLookupResponse.AddAsync(lookuppaymentresponse);
            await _context.SaveChangesAsync();

            return postsingledstvbill;
           // return new InitiatePayUPaymentResponse { resultCode = AppResponseCodes.Success, Data = postsingledstvbill };
        }

        public async Task<WebApiResponse> PayUSingleDstvPayment(SingleDstvPaymentDto model, long clientId)
        {

            try
            {
                //if(await _context.SingleDstvPayment.AnyAsync(x=> x.)

                //clientId = 200;

                var singledstv = new SingleDstvPayment();

                foreach (var item in model.customFields)
                {
                    singledstv.key = item.key;
                    singledstv.value = item.value;
                }

                singledstv.amountInCents = model.amountInCents;
                singledstv.merchantReference = model.merchantReference;
                singledstv.transactionType = _appSettings.SinglePaymentTransactionType;
                singledstv.vasId = _appSettings.VasId;
                singledstv.countryCode = _appSettings.CountryCode;
                singledstv.customerId = model.customerId;
                singledstv.ClientAuthenticationId = clientId;

                await _context.SingleDstvPayment.AddAsync(singledstv);
                await _context.SaveChangesAsync();

                var defaultRequest = new SingleDstvPaymentDefaultDto
                {
                    amountInCents = model.amountInCents,
                    countryCode = _appSettings.CountryCode,
                    customerId = model.customerId,
                    customFields = model.customFields,
                    merchantReference = model.merchantReference,
                    transactionType = _appSettings.SinglePaymentTransactionType,
                    vasId = _appSettings.VasId
                };

                var postsingledstvbill = await _payWithPayUService.InitiatePayUSingleDstvPayment(defaultRequest);

                var singledstvpaymentresponse = new SingleDstvPaymentResponse
                {
                    resultMessage = postsingledstvbill.resultMessage,
                    resultCode = postsingledstvbill.resultCode,
                    payUVasReference = postsingledstvbill.payUVasReference,
                    merchantReference = postsingledstvbill.merchantReference,
                    SingleDstvPaymentId = singledstv.SingleDstvPaymentId
                };

                await _context.SingleDstvPaymentResponse.AddAsync(singledstvpaymentresponse);
                await _context.SaveChangesAsync();

              //  var message = postsingledstvbill.customFields.customfield.Select(x => x.value).FirstOrDefault();            

                return new WebApiResponse { ResponseCode = postsingledstvbill.resultCode, Data = postsingledstvbill };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Internal error occured" };
            }

        }

    }

}
