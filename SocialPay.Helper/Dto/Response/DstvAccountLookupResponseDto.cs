using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper.Dto.Response
{

    public class DataField
    {
        public string key { get; set; }
        public object value { get; set; }
    }

    public class DataFields
    {
        public List<DataField> dataField { get; set; }
    }

    public class DstvAccountLookupResponseDto
    {
        public string merchantReference { get; set; }
        public string payUVasReference { get; set; }
        public string resultCode { get; set; }
        public string resultMessage { get; set; }
        public string vasProvider { get; set; }
        public string vasProviderReference { get; set; }
        public DataFields dataFields { get; set; }
    }
}
