using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.Fiorano
{
    public class CreditDebitService
    {
        private readonly HttpClient _client;
        private readonly AppSettings _appSettings;
        public CreditDebitService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_appSettings.fioranoBaseUrl)
            };
        }

        public async Task<FTResponseDto> InitiateTransaction(string jsonRequest)
        {
            try
            {

                var response = await _client.PostAsync(_appSettings.fioranoFundsTransferUrl,
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    var responseBody = JsonConvert.DeserializeObject<FTResponseDto>(result);
                    responseBody.Message = result;
                    responseBody.ResponseCode = AppResponseCodes.Success;
                    return responseBody;
                }
                return new FTResponseDto { ResponseCode = AppResponseCodes.FiranoDebitError, Message = result };
            }
            catch (Exception ex)
            {
                return new FTResponseDto { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }

}
