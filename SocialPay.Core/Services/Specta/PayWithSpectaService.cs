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
		public PayWithSpectaService(IOptions<AppSettings> appSettings)
		{
			_appSettings = appSettings.Value;
			_client = new HttpClient
			{
				BaseAddress = new Uri(_appSettings.paywithSpectaBaseUrl),
			};
		}
        public async Task<WebApiResponse> InitiatePayment(decimal amount, string description, string transactionReference)
        {
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

                var response = await _client.PostAsync(_appSettings.paywithSpectaPurchaseUrlExtension,
                    new StringContent(request, Encoding.UTF8, "application/json"));
                _client.DefaultRequestHeaders.Add(_appSettings.paywithspectaHeaderKey, _appSettings.paywithspectaHeaderValue);
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var successfulResponse = JsonConvert.DeserializeObject<CreatePurchaseResponseDto>(result);
                    apiResponse.Data = successfulResponse.Result;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    return apiResponse;
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed };
			}
			catch (Exception)
			{
				return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
			}
        }

        public async Task<WebApiResponse> PaymentVerification(string message)
        {
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
            catch (Exception)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
