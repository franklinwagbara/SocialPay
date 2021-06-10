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

namespace SocialPay.Core.Services.Wallet
{
    public class WalletRepoService
    {
        private readonly HttpClient _client;
        private readonly AppSettings _appSettings;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(WalletRepoService));

        public WalletRepoService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            //var username = "test";
            //var password = "NeRWNtWQMS";
            //  baseUrl = "https://pass.sterling.ng/OneWallet/";
            _client = new HttpClient
            {
                BaseAddress = new Uri(_appSettings.walletBaseUrl)
            };

        }
        public async Task<WalletResponseDto> CreateMerchantWallet(MerchantWalletRequestDto model)
        {
            var apiResponse = new WalletResponseDto { };
            try
            {
                var request = JsonConvert.SerializeObject(model);

                _log4net.Info("Initiating CreateMerchantWallet request" + " | " + request + " | " + DateTime.Now);

                var response = await _client.PostAsync($"{_appSettings.walletExtensionUrl}{_appSettings.createwalletUrl}",
                    new StringContent(request, Encoding.UTF8, "application/json"));

                var result = await response.Content.ReadAsStringAsync();

                _log4net.Info("Initiating CreateMerchantWallet response" + " | " + result + " | " + DateTime.Now);

                if (response.IsSuccessStatusCode)
                {
                    apiResponse = JsonConvert.DeserializeObject<WalletResponseDto>(result);
                    apiResponse.responsedata = result;

                    return apiResponse;
                }

                if (result.Contains("Duplicate"))
                {
                    _log4net.Info("Duplicate merchant details" + " - " + result + " - " + model.mobile + " - " + DateTime.Now);

                    apiResponse.response = AppResponseCodes.DuplicateMerchantDetails;
                    apiResponse.responsedata = result;

                    return apiResponse;
                }

                apiResponse.response = AppResponseCodes.Failed;
                apiResponse.responsedata = result;

                return apiResponse;
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured CreateMerchantWallet" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                apiResponse.response = AppResponseCodes.InternalError;
                apiResponse.responsedata = "An error occured while creating wallet";
                return apiResponse;
            }
        }

        public async Task<WalletResponseDto> ClearMerchantWallet(string phoneNumber)
        {
            var apiResponse = new WalletResponseDto { };
            try
            {

                var response = await _client.GetAsync($"{ _appSettings.walletExtensionUrl}{ _appSettings.clearwalletUrl}{ phoneNumber}");
                var result = await response.Content.ReadAsStringAsync();
                
                if (response.IsSuccessStatusCode)
                {
                    //apiResponse = JsonConvert.DeserializeObject<WalletResponseDto>(result);
                    apiResponse.response = AppResponseCodes.Success;
                    return apiResponse;
                }
                apiResponse.response = AppResponseCodes.Failed;
                apiResponse.responsedata = result;
                return apiResponse;
            }
            catch (Exception ex)
            {
                apiResponse.response = AppResponseCodes.InternalError;
                apiResponse.responsedata = "An error occured while creating wallet";
                return apiResponse;
            }
        }

        public async Task<GetWalletInfoResponseDto> GetWalletDetailsAsync(string phoneNumber)
        {
            var apiResponse = new GetWalletInfoResponseDto { };
            try
            {               
                _log4net.Info("Initiating GetWalletDetailsAsync request" + " | " + phoneNumber + " | " + DateTime.Now);

                var response = await _client.GetAsync($"{_appSettings.walletExtensionUrl}{ _appSettings.getwalletDetailsUrl}{ phoneNumber}");
               
                var result = await response.Content.ReadAsStringAsync();
                _log4net.Info("Initiating GetWalletDetailsAsync response" + " | " + result + " | " + DateTime.Now);

                if (response.IsSuccessStatusCode)
                {
                    apiResponse = JsonConvert.DeserializeObject<GetWalletInfoResponseDto>(result);

                    return apiResponse;
                }

                apiResponse.Response = AppResponseCodes.Failed;
                apiResponse.Responsedata = result;

                return apiResponse;
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured GetWalletDetailsAsync" + " | " + phoneNumber + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                apiResponse.Response = AppResponseCodes.InternalError;
                apiResponse.Responsedata = "An error occured while creating wallet";

                return apiResponse;
            }
        }
    }
}
