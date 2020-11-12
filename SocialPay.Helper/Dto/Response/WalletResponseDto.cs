using Newtonsoft.Json;

namespace SocialPay.Helper.Dto.Response
{
    public class Data
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public object email { get; set; }
        public string mobile { get; set; }
    }

    public class WalletResponseDto
    {
        public string message { get; set; }
        public string response { get; set; }
        public object responsedata { get; set; }
        public Data data { get; set; }
    }

    public class WalletToWalletData
    {
        public bool sent { get; set; }
    }

    public class WalletToWalletResponseDto
    {
        public string message { get; set; }
        public string response { get; set; }
        public object responsedata { get; set; }
        [JsonProperty("data")]
        public WalletToWalletData data { get; set; }
    }



    public partial class GetWalletInfoResponseDto
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("response")]
        public string Response { get; set; }

        [JsonProperty("responsedata")]
        public object Responsedata { get; set; }

        [JsonProperty("data")]
        public GetWalletInfoData Data { get; set; }
    }

    public partial class GetWalletInfoData
    {
        [JsonProperty("customerid")]
        public long Customerid { get; set; }

        [JsonProperty("firstname")]
        public string Firstname { get; set; }

        [JsonProperty("nuban")]
        public string Nuban { get; set; }

        [JsonProperty("availablebalance")]
        public long Availablebalance { get; set; }

        [JsonProperty("lastname")]
        public string Lastname { get; set; }

        [JsonProperty("fullname")]
        public string Fullname { get; set; }

        [JsonProperty("mobile")]
        public string Mobile { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("currencycode")]
        public string Currencycode { get; set; }

        [JsonProperty("restind")]
        public long Restind { get; set; }
    }


}
