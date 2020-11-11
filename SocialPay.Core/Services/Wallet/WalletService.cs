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
                
               
                //model.CURRENCYCODE = "NGN"; 
                //model.DOB = "2090-05-21";
                //model.firstname = "Pat";
                //model.lastname = "Pat";
                //model.Gender = "M";
               // model.mobile = "83487783473";
                var request = JsonConvert.SerializeObject(model);
                ////var content = new StringContent(
                ////    request, Encoding.UTF8,
                ////    "application/json");                
                ////var response = await _client.PostAsync(_appSettings.walletExtensionUrl + _appSettings.createwalletUrl, content);
                ////var result = await response.Content.ReadAsStringAsync();

                var response = await _client.PostAsync(_appSettings.walletExtensionUrl + _appSettings.createwalletUrl,
                    new StringContent(request, Encoding.UTF8, "application/json"));
                var result = await response.Content.ReadAsStringAsync();
                if(response.IsSuccessStatusCode)
                {
                     apiResponse = JsonConvert.DeserializeObject<WalletResponseDto>(result);
                     apiResponse.responsedata = result;
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

        public async Task<WalletResponseDto> ClearMerchantWallet(string phoneNumber)
        {
            var apiResponse = new WalletResponseDto { };
            try
            {

                var response = await _client.GetAsync(_appSettings.walletExtensionUrl 
                    + _appSettings.clearwalletUrl + phoneNumber);
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

                var response = await _client.GetAsync(_appSettings.walletExtensionUrl
                    + _appSettings.getwalletDetailsUrl + phoneNumber);
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    apiResponse = JsonConvert.DeserializeObject<GetWalletInfoResponseDto>(result);
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

        public async Task<WalletToWalletResponseDto> WalletToWalletTransferAsync(WalletTransferRequestDto model)
        {
            var apiResponse = new WalletToWalletResponseDto { };
            try
            {
                var request = JsonConvert.SerializeObject(model);
                var response = await _client.PostAsync(_appSettings.walletExtensionUrl + _appSettings.walletTowalletUrl,
                  new StringContent(request, Encoding.UTF8, "application/json"));
                var result = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    apiResponse = JsonConvert.DeserializeObject<WalletToWalletResponseDto>(result);
                    apiResponse.responsedata = result;
                    return apiResponse;
                }
                apiResponse.response = AppResponseCodes.Failed;
                apiResponse.responsedata = result;
                return apiResponse;
            }
            catch (Exception ex)
            {
                return new WalletToWalletResponseDto { response = AppResponseCodes.InternalError };
            }
        }
    }
}
