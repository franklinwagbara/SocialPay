using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Utilities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SterlingIBS;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SocialPay.Core.Services.IBS
{
    public class IBSReposerviceJob
    {
        private readonly AppSettings _appSettings;
        public IBSReposerviceJob(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
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

                var en = new EncryptDecrypt();
                var encryptRequest = en.Encrypt(getBanksStringRequest);
                var encryptedDataRequest = await ibsService.IBSBridgeAsync(encryptRequest, Convert.ToInt32(_appSettings.appId));

                var decryptResponse = en.Decrypt(encryptedDataRequest.Body.IBSBridgeResult.ToString());
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

            try
            {
                var ibsService = new BSServicesSoapClient(BSServicesSoapClient.EndpointConfiguration.IBSServicesSoap, _appSettings.IBSserviceUrl);
                string referenceId = Guid.NewGuid().ToString().Substring(10) + " " + Convert.ToString(DateTime.Now.Ticks);

                var nameEnquiryStringBuilder = new StringBuilder();
                nameEnquiryStringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                nameEnquiryStringBuilder.Append("<IBSRequest>");
                nameEnquiryStringBuilder.Append("<ReferenceID>" + referenceId + "</ReferenceID>");
                nameEnquiryStringBuilder.Append("<RequestType>" + iBSNameEnquiryRequestDto.RequestType + "</RequestType>");
                nameEnquiryStringBuilder.Append("<ToAccount>" + iBSNameEnquiryRequestDto.ToAccount + "</ToAccount>");
                nameEnquiryStringBuilder.Append("<DestinationBankCode>" + iBSNameEnquiryRequestDto.DestinationBankCode + "</DestinationBankCode>");
                nameEnquiryStringBuilder.Append("</IBSRequest>");
                var nameEnquiryStringRequest = nameEnquiryStringBuilder.ToString();

                var en = new EncryptDecrypt();
                var encryptRequest = en.Encrypt(nameEnquiryStringRequest);
                              
                var encryptedDataRequest = await ibsService.IBSBridgeAsync(encryptRequest, Convert.ToInt32(_appSettings.appId));

                var decryptResponse = en.Decrypt(encryptedDataRequest.Body.IBSBridgeResult.ToString());
                var deserializeResponseObject = ObjectToXML(decryptResponse, typeof(IBSNameEnquiryResponseDto));

                var serializeResponse = JsonConvert.SerializeObject(deserializeResponseObject);
                var result = JsonConvert.DeserializeObject<IBSNameEnquiryResponseDto>(serializeResponse);
                return result;
            }
            catch (Exception ex)
            {
                return new IBSNameEnquiryResponseDto { ResponseCode = AppResponseCodes.InternalError };
            }
        }


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
