using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.PayU
{
    public class DstvPaymentService
    {

        private readonly AppSettings _appSettings;
        private readonly HttpClient _client;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(DstvPaymentService));

        public DstvPaymentService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_appSettings.paywithPayUBaseUrl),
            };

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization",
               Convert.ToBase64String(Encoding.Default.GetBytes("test1:test-1093$18KnX826)Z#fg")));
        }

        public async Task<GetBillerResponseDto> GetDstvGotvBillers(long clientId)
        {

            var apiResponse = new GetBillerResponseDto { };

            try
            {
                var request = await _client.PostAsync($"{_appSettings.GetBillersUrl}", null);

                var content = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    var successfulResponse = JsonConvert.DeserializeObject<GetBillerResponseDto>(content);
                    successfulResponse.ResponseCode = AppResponseCodes.Success;
                    successfulResponse.Message = "Success";

                    return successfulResponse;
                }

                return new GetBillerResponseDto { ResponseCode = AppResponseCodes.Failed, Message = "Failed" };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetBillers" + " | " + clientId + " | " +  ex + " | " + DateTime.Now);

                return new GetBillerResponseDto { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<InitiatePayUPaymentResponse> InitiatePayUSingleDstvPayment(SingleDstvPaymentDto model)

        {
            _log4net.Info("InitiatePayment request" + " | " + model.amountInCents + " | " +  model.merchantReference + " | " + model.customerId + " | "  + DateTime.Now);

            var apiResponse = new InitiatePayUPaymentResponse { };

            try
            {

                var jsonRequest = JsonConvert.SerializeObject(model);

                _log4net.Info("InitiatePayment pay with specta request" + " | " + model.merchantReference + " | " +  jsonRequest + " | " + DateTime.Now);

                var request = await _client.PostAsync($"{_appSettings.paywithPayUSingleDstvPurchaseUrlExtension}",
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                var content = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    var successfulResponse = JsonConvert.DeserializeObject<SingleDstvPaymentResponseDto>(content);
                    apiResponse.DataObj = successfulResponse;
                    apiResponse.resultCode = AppResponseCodes.Success;

                    return apiResponse;
                }

                return new InitiatePayUPaymentResponse { resultCode = AppResponseCodes.Failed };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "InitiatePayment" + " | " + model.merchantReference + " | " + model.customerId  + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new InitiatePayUPaymentResponse { resultCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<InitiatePayUPaymentResponse> InitiatePayUDstvAccountLookupPayment(DstvAccountLookupDto model)

        {
            _log4net.Info("InitiatePayment request" + " | " + model.countryCode + " | " + model.vasId + " | " + model.merchantReference + " | " + model.transactionType + " | " + model.customerId  + " | " + DateTime.Now);

            var apiResponse = new InitiatePayUPaymentResponse { };
            try
            {

                var jsonRequest = JsonConvert.SerializeObject(model);

                _log4net.Info("InitiatePayment pay with specta request" + " | " + model.merchantReference  + " | " + jsonRequest + " | " + DateTime.Now);

                var request = await _client.PostAsync($"{_appSettings.paywithPayUAccountLookupDstvPurchaseUrlExtension}",
                 new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                var content = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    var successfulResponse = JsonConvert.DeserializeObject<DstvAccountLookupResponseDto>(content);
                    apiResponse.DataObj = successfulResponse;
                    apiResponse.resultCode = AppResponseCodes.Success;

                    return apiResponse;
                }
                return new InitiatePayUPaymentResponse { resultCode = AppResponseCodes.Failed };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "InitiatePayment" + " | " + model.merchantReference + " | " + model.customerId  + " | " + ex + " | " + DateTime.Now);

                return new InitiatePayUPaymentResponse { resultCode = AppResponseCodes.InternalError };
            }
        }

    }

}
