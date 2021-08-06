using System.Collections.Generic;

namespace SocialPay.Helper.Dto.Response
{
    public class Datum
    {
        public int id { get; set; }
        public string bouquet_name { get; set; }
        public int amount { get; set; }
        public string product_key { get; set; }
        public string product_key2 { get; set; }
        public string product_key3 { get; set; }
        public string bouquet_category { get; set; }
    }

    public class GetBillerResponseDto
    {
        public string ResponseCode { get; set; }
        public string Message { get; set; }
        public List<Datum> data { get; set; }
    }
}
