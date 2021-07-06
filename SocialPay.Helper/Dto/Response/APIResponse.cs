namespace SocialPay.Helper.Dto.Response
{
    public class APIResponse<T> where T : class
    {
        public string StatusCode { get; set; }
        public string Message { get; set; }

        public T Data { get; set; }
    }
}
