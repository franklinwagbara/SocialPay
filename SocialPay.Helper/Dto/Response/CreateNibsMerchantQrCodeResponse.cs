namespace SocialPay.Helper.Dto.Response
{
    public class CreateNibsMerchantQrCodeResponse
    {
        public string returnCode { get; set; }
        public string returnMsg { get; set; }
        public string mchNo { get; set; }
        public string merchantName { get; set; }
        public string merchantTIN { get; set; }
        public string merchantAddress { get; set; }
        public string merchantContactName { get; set; }
        public string merchantPhoneNumber { get; set; }
        public string merchantEmail { get; set; }
        public string ResponseCode { get; set; }
        public string jsonResponse { get; set; }
    }

    public class CreateNibsSubMerchantQrCodeResponse
    {
        public string returnCode { get; set; }
        public string returnMsg { get; set; }
        public string mchNo { get; set; }
        public string merchantName { get; set; }
        public string subMchNo { get; set; }
        public string qrCode { get; set; }
        public string ResponseCode { get; set; }
        public string jsonResponse { get; set; }
    }

    public class QueryAccountResponse
    {
        public string sessionID { get; set; }
        public string destinationInstitutionCode { get; set; }
        public string channelCode { get; set; }
        public string accountNumber { get; set; }
        public string accountName { get; set; }
        public string bankVerificationNumber { get; set; }
        public string kycLevel { get; set; }
        public string returnCode { get; set; }
        public string returnMsg { get; set; }
        public string paymentReference { get; set; }
        public string amount { get; set; }

    }


    //"sessionID": "999166211101131245346750723498",
    //"destinationInstitutionCode": "999232",
    //"channelCode": "12",
    //"accountNumber": "8030523654",
    //"accountName": "FESTYTECH FESTYTECH",
    //"bankVerificationNumber": "N/A",
    //"kycLevel": "0",
    //"returnCode": "Success",
    //"returnMsg": null,
    //"paymentReference": null,
    //"amount": null


    public class CreateMerchantResponseDTO
    {
        public string returnCode { get; set; }
        public string returnMsg { get; set; }
        public string mchNo { get; set; }
        public string merchantName { get; set; }
        public string merchantTIN { get; set; }
        public string merchantAddress { get; set; }
        public string merchantContactName { get; set; }
        public string merchantPhoneNumber { get; set; }
        public string merchantEmail { get; set; }

    }



    //"returnCode": "Success",
    //"returnMsg": null,
    //"mchNo": "M0000004413",
    //"merchantName": "FESTYTECH FESTYTECH",
    //"merchantTIN": "",
    //"merchantAddress": "my address",
    //"merchantContactName": "Mide",
    //"merchantPhoneNumber": "00000000000",
    //"merchantEmail": "user@olamnide.com"

    public class CreateSubMerchantResponseDTO
    {
        public string mchNo { get; set; }
        public string merchantName { get; set; }
        public string merchantEmail { get; set; }
        public string merchantPhoneNumber { get; set; }
        public string subFixed { get; set; }
        public string subAmount { get; set; }
    }

}
