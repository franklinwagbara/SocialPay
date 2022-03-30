using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.FioranoT24;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Fiorano
{
    public class FioranoAPIService
    {
        private readonly AppSettings _appSettings;
        private readonly HttpClient _client;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(FioranoAPIService));
        private readonly FioranoT24Logger _fioranoT24Logger;

        public FioranoAPIService(IOptions<AppSettings> appSettings, FioranoT24Logger fioranoT24Logger)
        {
            _appSettings = appSettings.Value;

            _client = new HttpClient
            {
                BaseAddress = new Uri(_appSettings.fioranoBaseUrl),
            };
            _fioranoT24Logger = fioranoT24Logger;
        }

        public async Task<FTResponseDto> InitiateTransaction(string jsonRequest)
        {
            try
            {
                _fioranoT24Logger.LogRequest($"{"Job Service: Initiate Fiorano transfer service"}{ " | "}{jsonRequest}{" | "}{DateTime.Now}");

                var response = await _client.PostAsync(_appSettings.fioranoFundsTransferUrl,
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                var result = await response.Content.ReadAsStringAsync();

                _fioranoT24Logger.LogRequest($"{"Job Service: InitiateTransaction response"}{" | "}{result}{" | "}{DateTime.Now}");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = JsonConvert.DeserializeObject<FTResponseDto>(result);

                    if (responseBody.FTResponse.ResponseCode != AppResponseCodes.Success)
                    {
                        responseBody.Message = result;
                        responseBody.ResponseCode = AppResponseCodes.TransactionFailed;

                        return responseBody;
                    }

                    responseBody.Message = result;
                    responseBody.ResponseCode = AppResponseCodes.Success;

                    return responseBody;
                }

                return new FTResponseDto { ResponseCode = AppResponseCodes.FiranoDebitError, Message = result };
            }
            catch (Exception ex)
            {
                _fioranoT24Logger.LogRequest($"{"Job Service: An error occured while initiating fiorano transactions"}{" | "}{ex}{" - "}{DateTime.Now}");

                return new FTResponseDto { ResponseCode = AppResponseCodes.InternalError, Message = "Internal error occured" };
            }
        }

    }
}
