using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.FioranoJob;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.Fiorano
{
    public class CreditDebitService
    {
        private readonly HttpClient _client;
        private readonly AppSettings _appSettings;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(CreditDebitService));
        private readonly FioranoJobLogger _fioranoLogger;
        public CreditDebitService(IOptions<AppSettings> appSettings, FioranoJobLogger fioranoLogger)
        {
            _appSettings = appSettings.Value;
            _fioranoLogger = fioranoLogger;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_appSettings.fioranoBaseUrl)
            };
        }

        public async Task<FTResponseDto> InitiateTransaction(string jsonRequest)
        {
            try
            {
                _fioranoLogger.LogRequest($"{"Job Service: Initiate Fiorano transfer service" + " | " + jsonRequest + " | " }{DateTime.Now}", false);

                var response = await _client.PostAsync(_appSettings.fioranoFundsTransferUrl,
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                var result = await response.Content.ReadAsStringAsync();
                _fioranoLogger.LogRequest($"{"Job Service: InitiateTransaction response" + " | " + result + " | "}{DateTime.Now}", false);

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = JsonConvert.DeserializeObject<FTResponseDto>(result);

                    if(responseBody.FTResponse.ResponseCode != AppResponseCodes.Success)
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
                _fioranoLogger.LogRequest($"{"Job Service: An error occured while initiating fiorano transactions" + " | " + ex.Message.ToString() + " | "}{DateTime.Now}", true);

                return new FTResponseDto { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }

}
