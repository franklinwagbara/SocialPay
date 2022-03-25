using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.Merchant;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.QrCode
{
    public class NibbsQRCodeAPIJobService
    {
        private readonly HttpClient _client;
        private readonly HttpClient __client;
        private readonly AppSettings _appSettings;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(NibbsQRCodeAPIService));
        private readonly MerchantsLogger _merchantLogger;
        public NibbsQRCodeAPIJobService(IOptions<AppSettings> appSettings, MerchantsLogger merchantLogger)
        {
            _appSettings = appSettings.Value;
            _merchantLogger = merchantLogger;
            _client = new HttpClient
            {
                BaseAddress = new Uri(_appSettings.nibsQRCodeBaseUrl)
            };
        }
       


        public async Task<CreateNibsMerchantQrCodeResponse> CreateMerchant(createMerchantRequestPayload requestModel)
        {
            try
            {
                //requestModel.Tin = "012348484";

                //requestModel.Fee = 0.5;
                //requestModel.Phone = "4532366644";
                var response = new CreateNibsMerchantQrCodeResponse();

                //QueryAccount

                var QueryAccountPayload = JsonConvert.SerializeObject(requestModel.QueryAccountRequestDto);

                _merchantLogger.LogRequest($"{"Initiating QueryAccount request"}{ " | " + QueryAccountPayload + " | "}{ DateTime.Now}");


                var signature = QueryAccountPayload.GenerateHmac(_appSettings.nibsQRCodeClientSecret, true);
                _client.DefaultRequestHeaders.Remove(_appSettings.nibsQRCodeXClientHeaderName);
                _client.DefaultRequestHeaders.Remove(_appSettings.nibsQRCodeCheckSumHeaderName);

                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeXClientHeaderName, _appSettings.nibsQRCodeClientId);
                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeCheckSumHeaderName, signature);
                //_client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeClientSecretHeaderName, _appSettings.nibsQRCodeClientSecret);

                var request = await _client.PostAsync($"{_appSettings.nibsQRCodeQueryAccountUrl}",
                    new StringContent(QueryAccountPayload, Encoding.UTF8, "application/json"));
                if (!request.IsSuccessStatusCode)
                {
                    response.ResponseCode = AppResponseCodes.Failed;
                    return response;
                }
                var QueryAccountResult = await request.Content.ReadAsStringAsync();
                var DeserializeQueryAccounPayload = JsonConvert.DeserializeObject<QueryAccountResponse>(QueryAccountResult);
                if (DeserializeQueryAccounPayload.returnCode != "Success")
                {
                    response.ResponseCode = AppResponseCodes.Failed;
                    return response;
                }




                //CreateMerchant

                requestModel.NewCreateNibsMerchantRequestDto.accountName = DeserializeQueryAccounPayload.accountName;
                requestModel.NewCreateNibsMerchantRequestDto.name = DeserializeQueryAccounPayload.accountName;
                var CreateMerchantPayload = JsonConvert.SerializeObject(requestModel.NewCreateNibsMerchantRequestDto);
                _merchantLogger.LogRequest($"{"Create Merchant request"}{ " | " + CreateMerchantPayload + " | "}{ DateTime.Now}");

                signature = CreateMerchantPayload.GenerateHmac(_appSettings.nibsQRCodeClientSecret, true);
                _client.DefaultRequestHeaders.Remove(_appSettings.nibsQRCodeXClientHeaderName);
                _client.DefaultRequestHeaders.Remove(_appSettings.nibsQRCodeCheckSumHeaderName);

                __client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeXClientHeaderName, _appSettings.nibsQRCodeClientId);
                __client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeCheckSumHeaderName, signature);

                var CreateMerchantRequest = await __client.PostAsync($"{_appSettings.nibsQRCodeUpdatedCreateMerchantURL}",
                   new StringContent(CreateMerchantPayload, Encoding.UTF8, "application/json"));
                var CreateMerchantAccountResult = await CreateMerchantRequest.Content.ReadAsStringAsync();

                if (!CreateMerchantRequest.IsSuccessStatusCode)
                {
                    response.ResponseCode = AppResponseCodes.Failed;
                    return response;
                }
                var DeserializeCreateMerchantAccountPayload = JsonConvert.DeserializeObject<CreateNibsMerchantQrCodeResponse>(CreateMerchantAccountResult);
                if (DeserializeCreateMerchantAccountPayload.returnCode != "Success")
                {
                    response.ResponseCode = AppResponseCodes.Failed;
                    return response;
                }

                response = DeserializeCreateMerchantAccountPayload;
                response.ResponseCode = AppResponseCodes.Success;
                return response;




                //var jsonRequest = JsonConvert.SerializeObject(requestModel);



                //_log4net.Info("Initiating CreateMerchantWallet request" + " | " + jsonRequest + " | " + DateTime.Now);

                ////var signature = jsonRequest.GenerateHmac(_appSettings.nibsQRCodeClientSecret, true);            



                //var result = await request.Content.ReadAsStringAsync();

                //if (request.IsSuccessStatusCode)
                //{
                //    response = JsonConvert.DeserializeObject<CreateNibsMerchantQrCodeResponse>(result);

                //    if(response.returnCode != "Success")
                //    {
                //        response.ResponseCode = AppResponseCodes.Failed;

                //        return response;
                //    }

                //    response.ResponseCode = AppResponseCodes.Success;

                //    return response;
                //}

                //response.jsonResponse = result;
                //response.ResponseCode = AppResponseCodes.Failed;

                //return response;

            }
            catch (Exception ex)
            {

                return new CreateNibsMerchantQrCodeResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<CreateNibsSubMerchantQrCodeResponse> CreateSubMerchant(CreateNibbsSubMerchantDto requestModel)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(requestModel);

                var response = new CreateNibsSubMerchantQrCodeResponse();
                _merchantLogger.LogRequest($"{"Initiating Create sub Merchant request"}{ " | " + jsonRequest + " | "}{ DateTime.Now}");
                var signature = jsonRequest.GenerateHmac(_appSettings.nibsQRCodeClientSecret, true);
                _client.DefaultRequestHeaders.Remove(_appSettings.nibsQRCodeXClientHeaderName);
                _client.DefaultRequestHeaders.Remove(_appSettings.nibsQRCodeCheckSumHeaderName);


                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeXClientHeaderName, _appSettings.nibsQRCodeClientId);
                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeCheckSumHeaderName, signature);


                //_client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeXClientHeaderName, _appSettings.nibsQRCodeClientId);
                //_client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeClientSecretHeaderName, _appSettings.nibsQRCodeClientSecret);

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

                _merchantLogger.LogRequest($"{"Bind Merchant request"}{ " | " + jsonRequest + " | "}{ DateTime.Now}");

                var signature = jsonRequest.GenerateHmac(_appSettings.nibsQRCodeClientSecret, true);
                _client.DefaultRequestHeaders.Remove(_appSettings.nibsQRCodeXClientHeaderName);
                _client.DefaultRequestHeaders.Remove(_appSettings.nibsQRCodeCheckSumHeaderName);

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

        public async Task<DynamicPaymentResponseDto> DynamicPay(DynamicPaymentDefaultRequestDto requestModel)
        {
            try
            {
                var jsonRequest = JsonConvert.SerializeObject(requestModel);

                var response = new DynamicPaymentResponseDto();

                _merchantLogger.LogRequest($"{"Dynamic Pay request"}{ " | " + jsonRequest + " | "}{ DateTime.Now}");

                var signature = jsonRequest.GenerateHmac(_appSettings.nibsQRCodeClientSecret, true);
                _client.DefaultRequestHeaders.Remove(_appSettings.nibsQRCodeXClientHeaderName);
                _client.DefaultRequestHeaders.Remove(_appSettings.nibsQRCodeCheckSumHeaderName);

                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeXClientHeaderName, _appSettings.nibsQRCodeClientId);
                _client.DefaultRequestHeaders.Add(_appSettings.nibsQRCodeCheckSumHeaderName, signature);

                var request = await _client.PostAsync($"{_appSettings.nibsQRCodeDynamicPayUrl}",
                    new StringContent(jsonRequest, Encoding.UTF8, "application/json"));

                var result = await request.Content.ReadAsStringAsync();

                if (request.IsSuccessStatusCode)
                {
                    response = JsonConvert.DeserializeObject<DynamicPaymentResponseDto>(result);

                    response.ResponseCode = AppResponseCodes.Success;

                    return response;
                }

                response.jsonResponse = result;
                response.ResponseCode = AppResponseCodes.Failed;

                return response;

            }
            catch (Exception ex)
            {

                return new DynamicPaymentResponseDto { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
