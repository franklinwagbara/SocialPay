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
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using SocialPay.Core.Services.AirtimeVending;

namespace SocialPay.Core.Services.Bill
{
    public class BillService
    {

        private readonly SocialPayDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly DstvPaymentService _payWithPayUService;
        private readonly AirtimeVendingService _airtimeVendingService;

        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(BillService));

        public BillService(SocialPayDbContext context, IOptions<AppSettings> appSettings, 
            DstvPaymentService payWithPayUService, AirtimeVendingService airtimeVendingService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _appSettings = appSettings.Value;
            _payWithPayUService = payWithPayUService ?? throw new ArgumentNullException(nameof(payWithPayUService));
            _airtimeVendingService = airtimeVendingService ?? throw new ArgumentNullException(nameof(airtimeVendingService));
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

        public async Task<WebApiResponse> PayUAccountLookupPayment(string customerId, long clientId)
        {
             clientId = 167;

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
                return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicatePaymentReference, Message = "Duplicate Payment Reference" };

            await _context.DstvAccountLookup.AddAsync(accountlookdstv);
            await _context.SaveChangesAsync();

            var postsingledstvbill = await _payWithPayUService.InitiatePayUDstvAccountLookupPayment(model);

            if (postsingledstvbill.resultCode != AppResponseCodes.Success)
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request Failed", Data = postsingledstvbill};

            var response = (DstvAccountLookupResponseDto)postsingledstvbill.DataObj;

            var lookuppaymentresponse = new DstvAccountLookupResponse
            {
                resultMessage = response?.resultMessage,
                //pointOfFailure = response?.po,
                resultCode = postsingledstvbill?.resultCode,
                payUVasReference = response?.payUVasReference,
                merchantReference = response?.merchantReference,
                DstvAccountLookupId = accountlookdstv.DstvAccountLookupId
            };

            await _context.DstvAccountLookupResponse.AddAsync(lookuppaymentresponse);
            await _context.SaveChangesAsync();            

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = postsingledstvbill };
        }

        public async Task<WebApiResponse> PayUSingleDstvPayment(SingleDstvPaymentDto model, long clientId)
        {

            //if(await _context.SingleDstvPayment.AnyAsync(x=> x.)

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

            await _context.SingleDstvPayment.AddAsync(singledstv);

            var postsingledstvbill = await _payWithPayUService.InitiatePayUSingleDstvPayment(model);

            var response = (SingleDstvPaymentResponseDto)postsingledstvbill.DataObj;

            var singledstvpaymentresponse = new SingleDstvPaymentResponse
            {
                resultMessage = response?.resultMessage,
                pointOfFailure = response?.pointOfFailure,
                resultCode = postsingledstvbill?.resultCode,
                payUVasReference = response?.payUVasReference,
                merchantReference = response?.merchantReference
            };

            await _context.SingleDstvPaymentResponse.AddAsync(singledstvpaymentresponse);
            await _context.SaveChangesAsync();

            if (postsingledstvbill.resultCode != AppResponseCodes.Success)
                return new WebApiResponse { ResponseCode = postsingledstvbill.resultCode };

            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = postsingledstvbill };

        }

    }

}
