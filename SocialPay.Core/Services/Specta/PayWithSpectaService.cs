using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Specta
{
    public class PayWithSpectaService
    {
		private readonly AppSettings _appSettings;
		private readonly HttpClient _client;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(PayWithSpectaService));

        public PayWithSpectaService(IOptions<AppSettings> appSettings)
		{
			_appSettings = appSettings.Value;
			_client = new HttpClient
			{
				BaseAddress = new Uri(_appSettings.paywithSpectaBaseUrl),
			};
		}
        public async Task<WebApiResponse> InitiatePayment(decimal amount, string description, string transactionReference, string merchantId, 
            string merchantKey)
        {
            _log4net.Info("InitiatePayment request" + " | " + transactionReference + " | " + amount + " | " + merchantId + " | "+ merchantKey + " | "+ DateTime.Now);

            var apiResponse = new WebApiResponse { };
            try
			{
                var model = new PaywithSpectRequestDto
                {
                    amount = Convert.ToString(amount), callBackUrl = _appSettings.paywithSpectaCallBackUrl,
                    description = description, merchantId = _appSettings.paywithspectamerchanId,
                    reference = transactionReference
                };
                var request = JsonConvert.SerializeObject(model);

                _log4net.Info("InitiatePayment pay with specta request" + " | " + transactionReference + " | " + merchantId + " | " + request + " | "+ DateTime.Now);


                var response = await _client.PostAsync(_appSettings.paywithSpectaPurchaseUrlExtension,
                    new StringContent(request, Encoding.UTF8, "application/json"));

                _client.DefaultRequestHeaders.Add(_appSettings.paywithspectaHeaderKey, _appSettings.paywithspectaHeaderValue);
                //_client.DefaultRequestHeaders.Add(_appSettings.paywithspectaHeaderKey, merchantKey);
               
                var result = await response.Content.ReadAsStringAsync();
                _log4net.Info("InitiatePayment response" + " | " + transactionReference + " | " + amount + " | " + result + " | " + merchantId + " | "+ DateTime.Now);

                if (response.IsSuccessStatusCode)
                {
                    var successfulResponse = JsonConvert.DeserializeObject<CreatePurchaseResponseDto>(result);
                    apiResponse.Data = successfulResponse.Result;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    return apiResponse;
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed };
			}
			catch (Exception ex)
			{
                _log4net.Error("Error occured" + " | " + "InitiatePayment" + " | " + transactionReference + " | " + amount + " | "+ merchantId + " | "+ ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
			}
        }

        public async Task<WebApiResponse> PaymentVerification(string message)
        {
            _log4net.Info("PaymentVerification request" + " | " + message + " | " +  DateTime.Now);

            var apiResponse = new WebApiResponse { };
            try
            {
                var decodeMessage = System.Uri.UnescapeDataString(message);
                var model = new PayWithSpectaVerificationRequestDto
                {
                    verificationToken = decodeMessage,
                };
                var request = JsonConvert.SerializeObject(model);

                var response = await _client.PostAsync(_appSettings.paywithSpectaverifyPaymentUrl,
                    new StringContent(request, Encoding.UTF8, "application/json"));

                var result = await response.Content.ReadAsStringAsync();
                _log4net.Info("PaymentVerification response" + " | " + message + " | " +  result + " | " + DateTime.Now);

                if (response.IsSuccessStatusCode)
                {
                    var successfulResponse = JsonConvert.DeserializeObject<PaymentVerificationResponseDto>(result);
                    apiResponse.Data = successfulResponse.Result;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    apiResponse.Message = Convert.ToString(successfulResponse.Result.Data.PaymentReference);
                    return apiResponse;
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "PaymentVerification" + " | " + message + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
