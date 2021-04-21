using Microsoft.Extensions.Options;
using NewIbsService;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Utilities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SterlingIBS;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SocialPay.Core.Services.IBS
{
    public class IBSReposerviceJob
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(IBSReposerviceJob));

        private readonly AppSettings _appSettings;
        private readonly EncryptDecryptJob _encryptDecrypt;

        public IBSReposerviceJob(IOptions<AppSettings> appSettings, EncryptDecryptJob encryptDecrypt)
        {
            _appSettings = appSettings.Value;
            _encryptDecrypt = encryptDecrypt;
        }

        public async Task<WebApiResponse> GetParticipatingBanks(IBSGetBanksRequestDto getBanksRequestModel)
        {

            try
            {
                var ibsService = new BSServicesSoapClient(BSServicesSoapClient.EndpointConfiguration.IBSServicesSoap, _appSettings.IBSserviceUrl);
                string referenceId = Guid.NewGuid().ToString().Substring(10) + " " + Convert.ToString(DateTime.Now.Ticks);

                var getBanksStringBuilder = new StringBuilder();
                getBanksStringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                getBanksStringBuilder.Append("<IBSRequest>");
                getBanksStringBuilder.Append("<ReferenceID>" + referenceId + "</ReferenceID>");
                getBanksStringBuilder.Append("<RequestType>" + getBanksRequestModel.RequestType + "</RequestType>");              
                getBanksStringBuilder.Append("</IBSRequest>");
                var getBanksStringRequest = getBanksStringBuilder.ToString();

                //var en = new EncryptDecrypt();
                var encryptRequest = _encryptDecrypt.Encrypt(getBanksStringRequest);
                var encryptedDataRequest = await ibsService.IBSBridgeAsync(encryptRequest, Convert.ToInt32(_appSettings.appId));

                var decryptResponse = _encryptDecrypt.Decrypt(encryptedDataRequest.Body.IBSBridgeResult.ToString());
                var deserializeResponseObject = ObjectToXML(decryptResponse, typeof(IBSGetBanksResponse));

                var serializeResponse = JsonConvert.SerializeObject(deserializeResponseObject);
                var result = JsonConvert.DeserializeObject<IBSGetBanksResponse>(serializeResponse);

                if (result.ResponseCode == AppResponseCodes.Success)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<IBSNameEnquiryResponseDto> InitiateNameEnquiry(IBSNameEnquiryRequestDto iBSNameEnquiryRequestDto)
        {
            _log4net.Info("Initiating InitiateNameEnquiry request" + " | " + iBSNameEnquiryRequestDto.ReferenceID + " | " + iBSNameEnquiryRequestDto.DestinationBankCode + " | " + iBSNameEnquiryRequestDto.ToAccount + " | " + DateTime.Now);

            try
            {
                var random = new Random();

                string randomNumber = string.Join(string.Empty, Enumerable.Range(0, 10).Select(number => random.Next(0, 9).ToString()));

                var dateFormat = DateTime.UtcNow.ToString("MMddyyyyhhmmss");

                var sessionId = _appSettings.SterlingBankCode + dateFormat + randomNumber;

                var nameEnquiryService = new NewIBSSoapClient(NewIBSSoapClient.EndpointConfiguration.NewIBSSoap, _appSettings.nfpliveBaseUrl);
                string referenceId = Guid.NewGuid().ToString().Substring(10) + " " + Convert.ToString(DateTime.Now.Ticks);

                var sendRequest = await nameEnquiryService.NameEnquiryAsync(sessionId, iBSNameEnquiryRequestDto.DestinationBankCode, _appSettings.nameEnquiryChannelCode, iBSNameEnquiryRequestDto.ToAccount);

                var response = sendRequest.Body.NameEnquiryResult.ToString();

                _log4net.Info("Name Enquiry response" + " | " + response + " - " + iBSNameEnquiryRequestDto.ReferenceID + " | " + iBSNameEnquiryRequestDto.DestinationBankCode + " | " + iBSNameEnquiryRequestDto.ToAccount + " | " + DateTime.Now);

                if (!response.Contains("00"))
                {
                    _log4net.Info("InitiateNameEnquiry request failed" + " | " + iBSNameEnquiryRequestDto.ReferenceID + " | " + iBSNameEnquiryRequestDto.DestinationBankCode + " | " + iBSNameEnquiryRequestDto.ToAccount + " | " + DateTime.Now);

                    return new IBSNameEnquiryResponseDto { ResponseCode = AppResponseCodes.InterBankNameEnquiryFailed };
                }

                var result = new IBSNameEnquiryResponseDto
                {
                    BVN = response.Split(":")[2],
                    AccountName = response.Split(":")[1],
                    ResponseCode = AppResponseCodes.Success,
                    SessionID = sessionId,
                    KYCLevel = string.Empty,
                    ReferenceID = iBSNameEnquiryRequestDto.ReferenceID,
                    RequestType = iBSNameEnquiryRequestDto.RequestType
                };

                if(!string.IsNullOrEmpty(response.Split(":")[3]))
                    result.KYCLevel = response.Split(":")[3];

                return result;
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "InitiateNameEnquiry" + " | " + iBSNameEnquiryRequestDto.ReferenceID + " | " + iBSNameEnquiryRequestDto.DestinationBankCode + " | " + iBSNameEnquiryRequestDto.ToAccount + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new IBSNameEnquiryResponseDto { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        //public async Task<IBSNameEnquiryResponseDto> InitiateNameEnquiry(IBSNameEnquiryRequestDto iBSNameEnquiryRequestDto)
        //{
        //    _log4net.Info("Job Service" + "-" + "InitiateNameEnquiry" + " | " + iBSNameEnquiryRequestDto.DestinationBankCode + " | " + iBSNameEnquiryRequestDto.ReferenceID + " | " + iBSNameEnquiryRequestDto.ToAccount + " | " + DateTime.Now);

        //    try
        //    {
        //        var ibsService = new BSServicesSoapClient(BSServicesSoapClient.EndpointConfiguration.IBSServicesSoap, _appSettings.IBSserviceUrl);

        //        var referenceId = Guid.NewGuid().ToString().Substring(10) + " " + Convert.ToString(DateTime.Now.Ticks);

        //        var nameEnquiryStringBuilder = new StringBuilder();
        //        nameEnquiryStringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        //        nameEnquiryStringBuilder.Append("<IBSRequest>");
        //        nameEnquiryStringBuilder.Append("<ReferenceID>" + referenceId + "</ReferenceID>");
        //        nameEnquiryStringBuilder.Append("<RequestType>" + iBSNameEnquiryRequestDto.RequestType + "</RequestType>");
        //        nameEnquiryStringBuilder.Append("<ToAccount>" + iBSNameEnquiryRequestDto.ToAccount + "</ToAccount>");
        //        nameEnquiryStringBuilder.Append("<DestinationBankCode>" + iBSNameEnquiryRequestDto.DestinationBankCode + "</DestinationBankCode>");
        //        nameEnquiryStringBuilder.Append("</IBSRequest>");

        //        var nameEnquiryStringRequest = nameEnquiryStringBuilder.ToString();

        //       // var en = new EncryptDecrypt();
        //        var encryptRequest = _encryptDecrypt.Encrypt(nameEnquiryStringRequest);

        //        var encryptedDataRequest = await ibsService.IBSBridgeAsync(encryptRequest, Convert.ToInt32(_appSettings.appId));

        //        _log4net.Info("Job Service" + "-" + "InitiateNameEnquiry encryptedDataRequest" + " | " + encryptedDataRequest + " | " + iBSNameEnquiryRequestDto.DestinationBankCode + " | " + iBSNameEnquiryRequestDto.ToAccount + " - " + iBSNameEnquiryRequestDto.ReferenceID + " - " + DateTime.Now);


        //        var decryptResponse = _encryptDecrypt.Decrypt(encryptedDataRequest.Body.IBSBridgeResult.ToString());

        //        var deserializeResponseObject = ObjectToXML(decryptResponse, typeof(IBSNameEnquiryResponseDto));

        //        _log4net.Info("Job Service" + "-" + "InitiateNameEnquiry deserializeResponseObject Response" + " | " + deserializeResponseObject + " | " + iBSNameEnquiryRequestDto.DestinationBankCode + " | " + iBSNameEnquiryRequestDto.ToAccount + " - " + iBSNameEnquiryRequestDto.ReferenceID + " - " + DateTime.Now);


        //        var serializeResponse = JsonConvert.SerializeObject(deserializeResponseObject);

        //        var result = JsonConvert.DeserializeObject<IBSNameEnquiryResponseDto>(serializeResponse);

        //        _log4net.Info("Job Service" + "-" + "InitiateNameEnquiry final result" + " | " + result + " | " + iBSNameEnquiryRequestDto.DestinationBankCode + " | " + iBSNameEnquiryRequestDto.ToAccount + " - " + iBSNameEnquiryRequestDto.ReferenceID + " - " + DateTime.Now);

        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        _log4net.Error("An error occured. InitiateNameEnquiry service" + " | " + iBSNameEnquiryRequestDto.ToAccount + " | " + iBSNameEnquiryRequestDto.DestinationBankCode + " | " + ex.Message.ToString() + " | " + DateTime.Now);

        //        return new IBSNameEnquiryResponseDto { ResponseCode = AppResponseCodes.InternalError };
        //    }
        //}


        public static Object ObjectToXML(string xml, Type objectType)
        {
            StringReader strReader = null;
            XmlSerializer serializer = null;
            XmlTextReader xmlReader = null;
            Object obj = null;
            try
            {
                strReader = new StringReader(xml);
                serializer = new XmlSerializer(objectType);
                xmlReader = new XmlTextReader(strReader);
                obj = serializer.Deserialize(xmlReader);
            }
            catch (Exception exp)
            {
                //Handle Exception Code
            }
            finally
            {
                if (xmlReader != null)
                {
                    xmlReader.Close();
                }
                if (strReader != null)
                {
                    strReader.Close();
                }
            }
            return obj;
        }

    }
}
