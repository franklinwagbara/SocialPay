using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.AirtimeVending
{
    public class AirtimeVendingService
    {
        private readonly AppSettings _appSettings;
        private readonly HttpClient _client;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AirtimeVendingService));
        public AirtimeVendingService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;

            _client = new HttpClient
            {
                BaseAddress = new Uri(_appSettings.airtimePaymentBaseUrl),
            };
        }

        public async Task<WebApiResponse> NetworkProviders()
        {
            _log4net.Info("Network Providers request" + DateTime.Now);

            var apiResponse = new WebApiResponse { };
            try
            {
                var request = await _client.GetAsync($"{_appSettings.GetBillerByCategoryUrl}{_appSettings.GetBillerByCategoryValue}");

                var content = await request.Content.ReadAsStringAsync();

                _log4net.Info("get network providers response" + " - "+ content + " - "+ DateTime.Now);

                if (!request.IsSuccessStatusCode)
                    new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Data = { }, StatusCode = ResponseCodes.Badrequest };

                //var data = JsonConvert.DeserializeObject<GetNetWorkProvidersResponseDto>(content);

                //return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = data, StatusCode = ResponseCodes.Success };

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = content, StatusCode = ResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + " NetworkProviders" + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetNetworkProducts(int networkProviderId)
        {
            try
            {
                _log4net.Info("GetPaymentItem request" + DateTime.Now);

                var response = await _client.GetAsync($"{_appSettings.GetBillerProductsUrl}{networkProviderId}");

                var content = await response.Content.ReadAsStringAsync();

                _log4net.Info("GetPaymentItem response" + " - " + networkProviderId + " - " + content + " - " + DateTime.Now);

                if (!content.Contains("ResponseCode"))
                   return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, StatusCode = ResponseCodes.RecordNotFound };

                if (!response.IsSuccessStatusCode)
                   return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, StatusCode = ResponseCodes.RecordNotFound };

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = await response.Content.ReadAsStringAsync(), StatusCode = ResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + " GetPaymentItem" + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> AirtimeSubscription(VendAirtimeViewModel model)
        {
            try
            {
                _log4net.Info("airtimeSubscription request" + " - " + model.nuban + " - "+ model.ReferenceId + " - "+ DateTime.Now);

                var jsonRequest = JsonConvert.SerializeObject(model);

                var request = await _client.PostAsync(_appSettings.PayBillerUrl,
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                var content = await request.Content.ReadAsStringAsync();

                _log4net.Info("airtimeSubscription respnse" + " - " + model.nuban + " - " + model.ReferenceId + " - " + content + " - "+ DateTime.Now);

                if (!request.IsSuccessStatusCode)
                    new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Subscription failed", StatusCode = ResponseCodes.Badrequest };

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = content, StatusCode = ResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + " GetPaymentItem" + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, StatusCode = ResponseCodes.InternalError };
            }
        }

    }
}
