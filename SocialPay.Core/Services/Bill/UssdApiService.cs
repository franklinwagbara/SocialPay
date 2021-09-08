using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Bill
{
    public class UssdApiService
    {
        private readonly AppSettings _appSettings;
        private readonly HttpClient _client;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(UssdApiService));

        public UssdApiService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

            _client = new HttpClient
            {
                BaseAddress = new Uri(_appSettings.UssdBaseUrl),
            };
        }

        public async Task<GenerateReferenceResponseDto> InitiateNewPaymentReference(GenerateReferenceRequestDTO model, long clientId)
        {
            try
            {

                var successfulResponse = new GenerateReferenceResponseDto();

                var jsonRequest = JsonConvert.SerializeObject(model);

                _log4net.Info("Ussd GenerateReference payload " + " | " + model.transRef + " | " + clientId + " | " + jsonRequest + " | " + DateTime.Now);
              
                var response = await _client.PostAsync(_appSettings.UssdGenerateReference,
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                var result = await response.Content.ReadAsStringAsync();

                _log4net.Info("Ussd GenerateReference  response" + " | " + model.transRef + " | " + model.amount + " | " + result + " | " + clientId + " | " + DateTime.Now);
               
                if(response.IsSuccessStatusCode)
                {
                    successfulResponse = JsonConvert.DeserializeObject<GenerateReferenceResponseDto>(result);

                    if(successfulResponse == null)
                    {
                        return new GenerateReferenceResponseDto { ResponseCode = AppResponseCodes.Failed };
                    }

                    successfulResponse.ResponseCode = AppResponseCodes.Success;

                    return successfulResponse;
                }

                successfulResponse.ResponseCode = AppResponseCodes.Failed;

                return successfulResponse;
            }
            catch (Exception ex)
            {

                return new GenerateReferenceResponseDto { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<GateWayResponseDto> UssdTransactionRequery(GatewayRequeryRequestDTO payload)
        {
            try
            {

                var request = JsonConvert.SerializeObject(payload);

                _log4net.Info("GatewayRequery" + " | " + payload.TransactionID + " | " + request + " | " + DateTime.Now);
                var response = await _client.PostAsync("GatewayRequery",
                    new StringContent(request, Encoding.UTF8, "application/json"));

                var result = await response.Content.ReadAsStringAsync();

                _log4net.Info("Initiate GatewayRequery response" + " | " + payload.TransactionID + " | " + payload.amount + " | " + result + " | " +  DateTime.Now);
               
                if(response.IsSuccessStatusCode)
                {
                    var successfulResponse = JsonConvert.DeserializeObject<GateWayResponseDto>(result);

                    return successfulResponse;
                }

                return new GateWayResponseDto { responseCode = AppResponseCodes.Failed };
               
            }
            catch (Exception ex)
            {
                return new GateWayResponseDto { responseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
