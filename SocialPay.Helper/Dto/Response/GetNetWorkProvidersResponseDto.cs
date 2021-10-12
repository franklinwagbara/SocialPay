using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{
    public class Biller
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }
        public string Narration { get; set; }
    }

    public class MyArray
    {
        public List<Biller> billers { get; set; }
        public string ResponseCode { get; set; }
        public int TotalAvailable { get; set; }
    }

    public class GetNetWorkProvidersResponseDto
    {
        public List<MyArray> MyArray { get; set; }
    }
}
