using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
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
                    description = description, merchantId = merchantId,
                    reference = transactionReference
                };
                var request = JsonConvert.SerializeObject(model);

                _log4net.Info("InitiatePayment pay with specta request" + " | " + transactionReference + " | " + merchantId + " | " + request + " | "+ DateTime.Now);

                _client.DefaultRequestHeaders.Add(_appSettings.paywithspectaHeaderKey, merchantKey);
               // _client.DefaultRequestHeaders.Add(_appSettings.paywithspectaHeaderKey, _appSettings.paywithspectaHeaderValue);

                var response = await _client.PostAsync(_appSettings.paywithSpectaPurchaseUrlExtension,
                    new StringContent(request, Encoding.UTF8, "application/json"));
               
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

        public async Task<WebApiResponse> CreateSpectaAccount(CreateSpectaRequestDto model)
        {
            _log4net.Info("creating a specta account request" + model.CompanyName + " | " + DateTime.Now);

            try
            {
                int ChannelId = 1;
                var apiResponse = new WebApiResponse { };
                var TINNumberLookUp = await _client.GetAsync(_appSettings.tinvalidationEndpointUrl +model.TinNumber);
                var result = await TINNumberLookUp.Content.ReadAsStringAsync();
                if (!TINNumberLookUp.IsSuccessStatusCode)
                {
                    apiResponse.ResponseCode = AppResponseCodes.TinValidationFailed;
                    return apiResponse;
                }
                var formContent = new FormUrlEncodedContent(new[]
                 {
                    new KeyValuePair<string, string>("CompanyName", model.CompanyName),
                    new KeyValuePair<string, string>("Address", model.Address),
                    new KeyValuePair<string, string>("EmailAddress", model.EmailAddress),
                    new KeyValuePair<string, string>("WebsiteUrl", model.WebsiteUrl),
                    new KeyValuePair<string, string>("BusinessSegmentId",  Convert.ToString(model.BusinessSegmentId)),
                    new KeyValuePair<string, string>("TinNumber", model.TinNumber),
                    new KeyValuePair<string, string>("ChannelId", Convert.ToString(ChannelId)),
                    new KeyValuePair<string, string>("RcNumber", model.RcNumber),
                    new KeyValuePair<string, string>("YearsInBusiness",  Convert.ToString(model.YearsInBusiness)),
                    new KeyValuePair<string, string>("State", model.State),
                    new KeyValuePair<string, string>("SourceOfAwareness", model.SourceOfAwareness),
                    new KeyValuePair<string, string>("StoreDescription", model.StoreDescription),
                    new KeyValuePair<string, string>("PhoneNumbers", model.PhoneNumbers.ToString()),
                    new KeyValuePair<string, string>("Directors", model.Directors.ToString()),

                 }
                );

                var response = await _client.PostAsync(_appSettings.createSpectaUrlExtension, formContent);
                var res = await response.Content.ReadAsStringAsync();
                _log4net.Info("create specta account"  + result + " | "  + DateTime.Now);
                var responseToDTOobject = JsonConvert.DeserializeObject<CreateSpectaAccountResponse>(res);
                if (response.IsSuccessStatusCode)
                {
                    //  var successfulResponse = JsonConvert.DeserializeObject<CreateSpectaAccountResponse>(response.Content);
                    // apiResponse.Data = successfulResponse.Result;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    return apiResponse;

                }
                apiResponse.ResponseCode = AppResponseCodes.Failed;
                apiResponse.Message = responseToDTOobject.error.message;
                return apiResponse;

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "creating a specta account request" + model.CompanyName + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }


        }

        public async Task<WebApiResponse> PaymentVerification(string message)
        {
            _log4net.Info("PaymentVerification request" + " | " + message + " | " +  DateTime.Now);

            var apiResponse = new WebApiResponse { };
            try
            {
                var decodeMessage = Uri.UnescapeDataString(message);
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
                    _log4net.Info("PaymentVerification was successful" + " | " + apiResponse.Message + " | " + result + " | " + DateTime.Now);

                    return apiResponse;
                }
                _log4net.Info("PaymentVerification failed" + " | " + apiResponse.Message + " | " + result + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Data = result };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "PaymentVerification" + " | " + message + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
