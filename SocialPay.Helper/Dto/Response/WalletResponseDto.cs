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

    public class GetWalletInfoData
    {
        public string customerid { get; set; }
        public string firstname { get; set; }
        public string nuban { get; set; }
        public int availablebalance { get; set; }
        public string lastname { get; set; }
        public string fullname { get; set; }
        public string mobile { get; set; }
        public string phone { get; set; }
        public string gender { get; set; }
        public string email { get; set; }
        public string currencycode { get; set; }
        public int restind { get; set; }
    }

    public class GetWalletInfoResponseDto
    {
        public string message { get; set; }
        public string response { get; set; }
        public object responsedata { get; set; }
        [JsonProperty("data")]
        public GetWalletInfoData data { get; set; }
    }
}
