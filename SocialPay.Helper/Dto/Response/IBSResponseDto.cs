using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace SocialPay.Helper.Dto.Response
{
    public class IBSResponseDto
    {
    }

	[XmlRoot(ElementName = "Rec")]
	public class Rec
	{
		[XmlElement(ElementName = "BANKNAME")]
		public string BANKNAME { get; set; }
		[XmlElement(ElementName = "BANKCODE")]
		public string BANKCODE { get; set; }
	}

	[XmlRoot(ElementName = "NIPBanklist")]
	public class NIPBanklist
	{
		[XmlElement(ElementName = "Rec")]
		public List<Rec> Rec { get; set; }
	}

	[XmlRoot(ElementName = "IBSResponse")]
	public class IBSGetBanksResponse
	{
		[XmlElement(ElementName = "ReferenceID")]
		public string ReferenceID { get; set; }
		[XmlElement(ElementName = "RequestType")]
		public string RequestType { get; set; }
		[XmlElement(ElementName = "ResponseCode")]
		public string ResponseCode { get; set; }
		[JsonIgnore]
		[XmlElement(ElementName = "ResponseText")]
		public string ResponseText { get; set; }
		[XmlElement(ElementName = "NIPBanklist")]
		public NIPBanklist NIPBanklist { get; set; }
	}
}
