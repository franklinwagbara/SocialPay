namespace SocialPay.Helper.Dto.Response
{
    public class FioranoResponseDto
    {
    }

    public class FTResponse
    {
        public string ReferenceID { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseText { get; set; }
        public string Balance { get; set; }
        public string COMMAMT { get; set; }
        public string CHARGEAMT { get; set; }
        public string FTID { get; set; }
    }

    public class FTResponseDto
    {
        public FTResponse FTResponse { get; set; }
        public string ResponseCode { get; set; }
        public string Message { get; set; }
    }
}
