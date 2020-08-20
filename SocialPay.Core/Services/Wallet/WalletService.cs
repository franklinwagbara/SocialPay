using Newtonsoft.Json;
using SocialPay.Helper.Dto.Request;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Wallet
{
    public class WalletService
    {
        private readonly HttpClient _client;

        public WalletService(string baseUrl)
        {
            //var username = "test";
            //var password = "NeRWNtWQMS";
            baseUrl = "https://pass.sterling.ng/OneWallet/";
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };
          
        }

        public async Task<bool> CreateMerchantWallet()
        {
            try
            {
                var model = new MerchantWalletRequestDto { };
                var request = JsonConvert.SerializeObject(model);
                var content = new StringContent(
                    request, Encoding.UTF8,
                    "application/json");
                string uri = "/api/Wallet/CreateWallet";
                var response = await _client.PostAsync($"vendor/{uri}", content);


                var response1 = await _client.PostAsync($"vendor/{uri}", new StringContent(
                    request, Encoding.UTF8,
                    "application/json"));

                //  var content = await response.Content.ReadAsStringAsync();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }
    }
}
