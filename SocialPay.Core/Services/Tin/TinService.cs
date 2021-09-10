using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Tin
{
    public class TinService
    {
		private readonly AppSettings _appSettings;
		private readonly HttpClient _client;
		public TinService(IOptions<AppSettings> appSettings)
		{
			_appSettings = appSettings.Value;
			_client = new HttpClient
			{
				BaseAddress = new Uri(_appSettings.tinvalidationBaseUrl),
			};
		}

        public async Task<WebApiResponse> ValidateTin(string tin)
        {
            try
            {
                var response = await _client.GetAsync(_appSettings.tinvalidationEndpointUrl + tin);
                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = JsonConvert.DeserializeObject<TinValidationResponseDto>(result);
                    if(jsonResponse.status == false)

                        return new WebApiResponse { ResponseCode = AppResponseCodes.TinValidationFailed, Message = ResponseMessage.TINValidationError };

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = ResponseMessage.Success };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.TinValidationFailed, Message = ResponseMessage.TINValidationError };
            }
            catch (Exception)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ResponseMessage.InternalError };
            }
        }

    }
}
