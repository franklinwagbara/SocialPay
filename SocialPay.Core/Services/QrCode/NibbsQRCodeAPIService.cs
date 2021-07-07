using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.QrCode
{
    public class NibbsQRCodeAPIService
    {
        private readonly HttpClient _client;
        private readonly AppSettings _appSettings;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(NibbsQRCodeAPIService));

        public NibbsQRCodeAPIService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

            _client = new HttpClient
            {
               // BaseAddress = new Uri("https://pass.sterling.ng/")
                BaseAddress = new Uri(_appSettings.nibsQRCodeBaseUrl)
            };

        }

        public async Task<WebApiResponse> CreateMerchant(CreateNibsMerchantRequestDto requestModel)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(requestModel);

                _log4net.Info("Initiating CreateMerchantWallet request" + " | " + jsonRequest + " | " + DateTime.Now);

                var signature = jsonRequest.GenerateHmac(_appSettings.nibsQRCodeClientSecret, true);            

                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeXClientHeaderName, _appSettings.nibsQRCodeClientId);
                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeCheckSumHeaderName, signature);

                var request = await _client.PostAsync($"{_appSettings.nibsQRCodeCreateMerchantUrl}",
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                var result = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    var response = JsonConvert.DeserializeObject<CreateNibsMerchantQrCodeResponse>(result);

                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Success" };
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Data = "Creation of merchant failed" };

            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "error occured" };
            }
        }
    }
}
