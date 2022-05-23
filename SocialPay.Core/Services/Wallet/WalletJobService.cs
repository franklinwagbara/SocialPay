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
    public class WalletRepoJobService
    {
        private readonly HttpClient _client;
        private readonly AppSettings _appSettings;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(WalletRepoJobService));

        public WalletRepoJobService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_appSettings.walletBaseUrl)
            };

        }

        public async Task<WalletToWalletResponseDto> WalletToWalletTransferAsync(WalletTransferRequestDto model)
        {
            var apiResponse = new WalletToWalletResponseDto { };

            _log4net.Info("Job Service" + "-" + "Wallet To Wallet TransferAsync.........." + " | " + model.toacct + " | " + model.paymentRef + " | " + model.frmacct + " | "+ model.amt + " | "+ DateTime.Now);

            try
            {
                var request = JsonConvert.SerializeObject(model);

                //Unlocking

                var response = await _client.PostAsync($"{_appSettings.walletExtensionUrl}{_appSettings.walletTowalletUrl}",
                  new StringContent(request, Encoding.UTF8, "application/json"));

                var result = await response.Content.ReadAsStringAsync();
                
                _log4net.Info("Job Service" + "-" + "WalletToWalletTransferAsync response" + " | " + result + " | "+ model.toacct + " | " + model.paymentRef + " | " + model.frmacct + " | " + model.amt + " | " + DateTime.Now);

                if (response.IsSuccessStatusCode)
                {
                    apiResponse = JsonConvert.DeserializeObject<WalletToWalletResponseDto>(result);
                    apiResponse.responsedata = result;



                   //Lock

                    return apiResponse;
                }

                apiResponse.response = AppResponseCodes.Failed;
                apiResponse.responsedata = result;

                return apiResponse;
            }
            catch (Exception ex)
            {
                _log4net.Error("Job Service" + "-" + "Error occured" + " | " + "WalletToWalletTransferAsync" + " | " + model.frmacct + " | "+ model.toacct + " | "+ ex.Message.ToString() + " | " + DateTime.Now);

                return new WalletToWalletResponseDto { response = AppResponseCodes.InternalError };
            }
        }
    }
}
