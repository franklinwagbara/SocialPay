using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
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
                BaseAddress = new Uri(_appSettings.nibsQRCodeBaseUrl)
            };

        }

        public async Task<CreateNibsMerchantQrCodeResponse> CreateMerchant(CreateNibsMerchantRequestDto requestModel)
        {
            try
            {
                requestModel.Tin = "012348484";
                requestModel.Fee = 0.5;

                var jsonRequest = JsonConvert.SerializeObject(requestModel);

                var response = new CreateNibsMerchantQrCodeResponse();

                _log4net.Info("Initiating CreateMerchantWallet request" + " | " + jsonRequest + " | " + DateTime.Now);

                var signature = jsonRequest.GenerateHmac(_appSettings.nibsQRCodeClientSecret, true);            

                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeXClientHeaderName, _appSettings.nibsQRCodeClientId);
                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeCheckSumHeaderName, signature);

                var request = await _client.PostAsync($"{_appSettings.nibsQRCodeCreateMerchantUrl}",
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                var result = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    response = JsonConvert.DeserializeObject<CreateNibsMerchantQrCodeResponse>(result);

                    if(response.returnCode != "Success")
                    {
                        response.ResponseCode = AppResponseCodes.Failed;

                        return response;
                    }

                    response.ResponseCode = AppResponseCodes.Success;

                    return response;
                }

                response.jsonResponse = result;
                response.ResponseCode = AppResponseCodes.Failed;

                return response;

            }
            catch (Exception ex)
            {

                return new CreateNibsMerchantQrCodeResponse { ResponseCode = AppResponseCodes.InternalError};
            }
        }

        public async Task<CreateNibsSubMerchantQrCodeResponse> CreateSubMerchant(CreateNibbsSubMerchantDto requestModel)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(requestModel);

                var response = new CreateNibsSubMerchantQrCodeResponse();

                _log4net.Info("Initiating Create sub Merchant request" + " | " + jsonRequest + " | " + DateTime.Now);

                var signature = jsonRequest.GenerateHmac(_appSettings.nibsQRCodeClientSecret, true);

                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeXClientHeaderName, _appSettings.nibsQRCodeClientId);
                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeCheckSumHeaderName, signature);

                var request = await _client.PostAsync($"{_appSettings.nibsQRCodeCreateSubMerchantUrl}",
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                var result = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    response = JsonConvert.DeserializeObject<CreateNibsSubMerchantQrCodeResponse>(result);

                    if (response.returnCode != "Success")
                    {
                        response.ResponseCode = AppResponseCodes.Failed;

                        return response;
                    }

                    response.ResponseCode = AppResponseCodes.Success;

                    return response;
                }

                response.jsonResponse = result;
                response.ResponseCode = AppResponseCodes.Failed;

                return response;

            }
            catch (Exception ex)
            {

                return new CreateNibsSubMerchantQrCodeResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<BindMechantResponseDto> BindMerchant(BindMerchantRequestDto requestModel)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(requestModel);

                var response = new BindMechantResponseDto();

                _log4net.Info("Initiating Create sub Merchant request" + " | " + jsonRequest + " | " + DateTime.Now);

                var signature = jsonRequest.GenerateHmac(_appSettings.nibsQRCodeClientSecret, true);

                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeXClientHeaderName, _appSettings.nibsQRCodeClientId);
                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeCheckSumHeaderName, signature);

                var request = await _client.PostAsync($"{_appSettings.nibsQRCodeBindMerchantAccountUrl}",
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                var result = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    response = JsonConvert.DeserializeObject<BindMechantResponseDto>(result);

                    if (response.ReturnCode != "Success")
                    {
                        response.ResponseCode = AppResponseCodes.Failed;

                        return response;
                    }
                    response.ResponseCode = AppResponseCodes.Success;

                    return response;
                }

                response.jsonResponse = result;
                response.ResponseCode = AppResponseCodes.Failed;

                return response;

            }
            catch (Exception ex)
            {
                return new BindMechantResponseDto { ResponseCode = AppResponseCodes.InternalError };
            }
        }

       // public static string T

        public async Task<WebApiResponse> RegisterNewWebHook(RegisterWebhookRequestDto model)
        {
            var response = new WebApiResponse();
           
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(model);

                var signature = jsonRequest.GenerateHmac(_appSettings.nibsQRCodeClientSecret, true);

                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeXClientHeaderName, _appSettings.nibsQRCodeClientId);
               // _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeXWebhookChecksum, _appSettings.nibsQRCodeClientId);
                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeCheckSumHeaderName, signature);

                var request = await _client.PostAsync($"{_appSettings.nibsQRCodeWebHookRegisterUrl}",
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                var content = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                   // var apiResponse = JsonConvert.DeserializeObject<List<WebHookFilterResponseDto>>(content);
                    response.ResponseCode = AppResponseCodes.Success;
                    response.Data = "Success";
                    return response;
                }

                response.ResponseCode = AppResponseCodes.Failed;

                return response;

            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetWebHookFilter()
        {
            var response = new WebApiResponse();

            try
            {
                var request = await _client.GetAsync(_appSettings.nibsQRCodeWebHookFilterUrl);

                var content = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    var apiResponse = JsonConvert.DeserializeObject<List<WebHookFilterResponseDto>>(content);
                    response.ResponseCode = AppResponseCodes.Success;
                    response.Data = apiResponse;
                    return response;
                }

                response.ResponseCode = AppResponseCodes.Failed;

                return response;

            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<DynamicQRCodeResponsDto> DynamicPay(DynamicPaymentDefaultRequestDto requestModel)
        {
            try
            {
                requestModel.mchNo = "M0000000001";
                requestModel.subMchNo = "S0000000002";

                var jsonRequest = JsonConvert.SerializeObject(requestModel);

                var response = new DynamicQRCodeResponsDto();

                _log4net.Info("Initiating Create sub Merchant request" + " | " + jsonRequest + " | " + DateTime.Now);

                var signature = jsonRequest.GenerateHmac(_appSettings.nibsQRCodeClientSecret, true);

                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeXClientHeaderName, _appSettings.nibsQRCodeClientId);
                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeCheckSumHeaderName, signature);

                var request = await _client.PostAsync($"{_appSettings.nibsQRCodeDynamicPayUrl}",
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                var result = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    
                    response = JsonConvert.DeserializeObject<DynamicQRCodeResponsDto>(result);

                    if (response.returnCode != "Success")
                    {
                        response.ResponseCode = AppResponseCodes.Failed;

                        return response;
                    }

                    response.ResponseCode = AppResponseCodes.Success;

                    return response;
                }

                //response.jsonResponse = result;
                response.ResponseCode = AppResponseCodes.Failed;

                return response;

            }
            catch (Exception ex)
            {

                return new DynamicQRCodeResponsDto { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
