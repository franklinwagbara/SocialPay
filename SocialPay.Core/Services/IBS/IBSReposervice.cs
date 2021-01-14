using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using NewIbsService;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Utilities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using SterlingIBS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SocialPay.Core.Services.IBS
{
    public class IBSReposervice
    {
        private readonly AppSettings _appSettings;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(IBSReposervice));

        public IBSReposervice(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }


        public async Task<List<GetParticipatingBanksViewModel>> GetBanks()
        {
            var result = new List<GetParticipatingBanksViewModel>();
            try
            {
                _log4net.Info("Initiating GetParticipatingBanks request" + " | " +  DateTime.Now);

                using (SqlConnection conn = new SqlConnection(_appSettings.nipdbConnectionString))
                {

                    string queryString = "select distinct bankcode, bankname from tbl_participatingbanks order by bankname asc";
                    SqlCommand oCmd = new SqlCommand(queryString, conn);
                    await conn.OpenAsync();
                    using (SqlDataReader oReader = await oCmd.ExecuteReaderAsync())
                    {

                        while (await oReader.ReadAsync())
                        {
                            var details = new GetParticipatingBanksViewModel();
                            details.BankCode = oReader["bankcode"].ToString();
                            details.BankName = oReader["bankname"].ToString();
                            //details.Category = oReader["category"].ToString();
                            //details.Statusflag = oReader["statusflag"].ToString();

                            result.Add(details);
                        }
                        await conn.CloseAsync();
                        await conn.DisposeAsync();
                        _log4net.Info("Initiating GetParticipatingBanks response" + " | " + result.Count + " | " + DateTime.Now);

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetParticipatingBanks" + " | "  + ex.Message.ToString() + " | " + DateTime.Now);

                return result;
            }

        }


        public async Task<WebApiResponse> GetParticipatingBanksTest()
        {
          //  _log4net.Info("Initiating GetParticipatingBanks request" + " | " + getBanksRequestModel.ReferenceID + " | " + getBanksRequestModel.RequestType + " | " + DateTime.Now);

            try
            {
                var ibsService = new NewIBSSoapClient(NewIBSSoapClient.EndpointConfiguration.NewIBSSoap);
                string referenceId = Guid.NewGuid().ToString().Substring(10) + " " + Convert.ToString(DateTime.Now.Ticks);

               
               // var en = new EncryptDecrypt();
                //var encryptRequest = en.Encrypt(getBanksStringRequest);
                var encryptedDataRequest = await ibsService.NameEnquiryAsync(referenceId, "000014", "1", "0025998012");
               // _log4net.Info("Initiating GetParticipatingBanks response" + " | " + encryptedDataRequest + " | " + getBanksRequestModel.RequestType + " | " + DateTime.Now);

              
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed };
            }
            catch (Exception ex)
            {
              //  _log4net.Error("Error occured" + " | " + "GetParticipatingBanks" + " | " + getBanksRequestModel.ReferenceID + " | " + getBanksRequestModel.RequestType + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetParticipatingBanks(IBSGetBanksRequestDto getBanksRequestModel)
        {
            _log4net.Info("Initiating GetParticipatingBanks request" + " | " + getBanksRequestModel.ReferenceID + " | " + getBanksRequestModel.RequestType + " | " +   DateTime.Now);

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

                _log4net.Info("Initiating GetParticipatingBanks xml request" + " | " + getBanksStringRequest + " | " + getBanksRequestModel.RequestType + " | " + DateTime.Now);

                var en = new EncryptDecrypt();
                var encryptRequest = en.Encrypt(getBanksStringRequest);
                var encryptedDataRequest = await ibsService.IBSBridgeAsync(encryptRequest, Convert.ToInt32(_appSettings.appId));
                _log4net.Info("Initiating GetParticipatingBanks response" + " | " + encryptedDataRequest + " | " + getBanksRequestModel.RequestType + " | " + DateTime.Now);
               
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
                _log4net.Error("Error occured" + " | " + "GetParticipatingBanks" + " | " + getBanksRequestModel.ReferenceID + " | " + getBanksRequestModel.RequestType + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }



        public async Task<IBSNameEnquiryResponseDto> InitiateNameEnquiryOld(IBSNameEnquiryRequestDto iBSNameEnquiryRequestDto)
        {
            _log4net.Info("Initiating InitiateNameEnquiry request" + " | " + iBSNameEnquiryRequestDto.ReferenceID + " | " + iBSNameEnquiryRequestDto.DestinationBankCode + " | " + iBSNameEnquiryRequestDto.ToAccount + " | " +  DateTime.Now);


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
                _log4net.Info("Initiating InitiateNameEnquiry response" + " | " + encryptedDataRequest + " | " + iBSNameEnquiryRequestDto.ToAccount + " | " + iBSNameEnquiryRequestDto.DestinationBankCode + " | "+ DateTime.Now);

                var decryptResponse = en.Decrypt(encryptedDataRequest.Body.IBSBridgeResult.ToString());
                var deserializeResponseObject = ObjectToXML(decryptResponse, typeof(IBSNameEnquiryResponseDto));

                var serializeResponse = JsonConvert.SerializeObject(deserializeResponseObject);
                var result = JsonConvert.DeserializeObject<IBSNameEnquiryResponseDto>(serializeResponse);
                return result;
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "InitiateNameEnquiry" + " | " + iBSNameEnquiryRequestDto.ReferenceID + " | " + iBSNameEnquiryRequestDto.DestinationBankCode + " | " + iBSNameEnquiryRequestDto.ToAccount + " | " +  ex.Message.ToString() + " | " + DateTime.Now);
                return new IBSNameEnquiryResponseDto { ResponseCode = AppResponseCodes.InternalError };
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

                var ibsService = new NewIBSSoapClient(NewIBSSoapClient.EndpointConfiguration.NewIBSSoap, _appSettings.nfpliveUrl);
                string referenceId = Guid.NewGuid().ToString().Substring(10) + " " + Convert.ToString(DateTime.Now.Ticks);

                var sendRequest = await ibsService.NameEnquiryAsync(sessionId, iBSNameEnquiryRequestDto.DestinationBankCode, "1", iBSNameEnquiryRequestDto.ToAccount);
                var response = sendRequest.Body.NameEnquiryResult.ToString();

                if (!response.Contains("00"))
                    return new IBSNameEnquiryResponseDto { ResponseCode = AppResponseCodes.InterBankNameEnquiryFailed };


                //00:PATRICK FESTUS OREVAGHENE: 22341210148:2
                var result = new IBSNameEnquiryResponseDto
                {
                    BVN = response.Split(":")[2],
                    AccountName = response.Split(":")[1],
                    ResponseCode = AppResponseCodes.Success
                };

                return result;
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "InitiateNameEnquiry" + " | " + iBSNameEnquiryRequestDto.ReferenceID + " | " + iBSNameEnquiryRequestDto.DestinationBankCode + " | " + iBSNameEnquiryRequestDto.ToAccount + " | " + ex.Message.ToString() + " | " + DateTime.Now);
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
