using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SocialPay.Helper.Dto.Request
{
    [XmlRoot(ElementName = "IBSRequest")]
    public class IBSNameEnquiryRequestDto : IBSDefaultRequestDto
    {
        [XmlElement(ElementName = "ToAccount")]
        public string ToAccount { get; set; }
        [XmlElement(ElementName = "DestinationBankCode")]
        public string DestinationBankCode { get; set; }
    }

    public class IBSDefaultRequestDto
    {
        [XmlElement(ElementName = "ReferenceID")]
        public string ReferenceID { get; set; }
        [XmlElement(ElementName = "RequestType")]
        public string RequestType { get; set; }
      
    }

    public class IBSGetBanksRequestDto : IBSDefaultRequestDto
    {
        
    }
}
