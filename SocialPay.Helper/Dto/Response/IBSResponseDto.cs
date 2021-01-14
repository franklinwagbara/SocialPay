using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SocialPay.Helper.Dto.Response
{
    [XmlRoot(ElementName = "IBSResponse")]
	public class IBSNameEnquiryResponseDto
	{
		[XmlElement(ElementName = "ReferenceID")]
		public string ReferenceID { get; set; }
		[XmlElement(ElementName = "RequestType")]
		public string RequestType { get; set; }
		[XmlElement(ElementName = "ResponseCode")]
		public string ResponseCode { get; set; }
		[XmlElement(ElementName = "ResponseText")]
		public string ResponseText { get; set; }
		[XmlElement(ElementName = "SessionID")]
		public string SessionID { get; set; }
		[XmlElement(ElementName = "BVN")]
		public string BVN { get; set; }
		[XmlElement(ElementName = "KYCLevel")]
		public string KYCLevel { get; set; }
		[XmlElement(ElementName = "AccountName")]
		public string AccountName { get; set; }
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

	[XmlRoot(ElementName = "NameEnquiryResponse", Namespace = "http://tempuri.org/")]
	//[JsonProperty("NameEnquiryResponse")]
	public class NameEnquiryResponseDto
	{
		[XmlElement(ElementName = "NameEnquiryResult", Namespace = "http://tempuri.org/")]
		public string NameEnquiryResult { get; set; }
		[XmlAttribute(AttributeName = "xmlns")]
		public string Xmlns { get; set; }
		public string ResponseCode { get; set; }
	}

	[XmlRoot(ElementName = "NameEnquiryResult")]
	public class NameEnquiryResult
	{
	}
}
