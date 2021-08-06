using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
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
                var response = await _client.GetAsync($"{_appSettings.GetBillerByCategoryUrl}{_appSettings.GetBillerByCategoryValue}");

                if (!response.IsSuccessStatusCode)
                    new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Data = { } };

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = await response.Content.ReadAsStringAsync() };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + " NetworkProviders" + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetNetworkProducts(int networkProviderId)
        {
            try
            {
                _log4net.Info("GetPaymentItem request" + DateTime.Now);

                var response = await _client.GetAsync($"{_appSettings.GetBillerProductsUrl}{networkProviderId}");

                if (!response.IsSuccessStatusCode)
                    new WebApiResponse { ResponseCode = AppResponseCodes.Failed };

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = await response.Content.ReadAsStringAsync() };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + " GetPaymentItem" + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
