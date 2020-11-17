using Newtonsoft.Json;
using System;

namespace SocialPay.Helper.Dto.Response
{


    public class CreatePurchaseResponseDto
    {
        [JsonProperty("result")]
        public Uri Result { get; set; }

        [JsonProperty("targetUrl")]
        public object TargetUrl { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error")]
        public object Error { get; set; }

        [JsonProperty("unAuthorizedRequest")]
        public bool UnAuthorizedRequest { get; set; }

        [JsonProperty("__abp")]
        public bool Abp { get; set; }
    }

    public partial class Error
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("details")]
        public string Details { get; set; }

        [JsonProperty("validationErrors")]
        public object ValidationErrors { get; set; }
    }

    public partial class PaymentVerificationResponseDto
    {
        [JsonProperty("result")]
        public Result Result { get; set; }

        [JsonProperty("targetUrl")]
        public object TargetUrl { get; set; }

        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error")]
        public object Error { get; set; }

        [JsonProperty("unAuthorizedRequest")]
        public bool UnAuthorizedRequest { get; set; }

        [JsonProperty("__abp")]
        public bool Abp { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("status")]
        public long Status { get; set; }

        [JsonProperty("data")]
        public PaymentVerificationData Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public partial class PaymentVerificationData
    {
        [JsonProperty("isSuccessful")]
        public bool IsSuccessful { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("amount")]
        public long Amount { get; set; }

        [JsonProperty("percentDiscountApplied")]
        public long PercentDiscountApplied { get; set; }

        [JsonProperty("amountSettled")]
        public long AmountSettled { get; set; }

        [JsonProperty("paymentReference")]
        public string PaymentReference { get; set; }

        [JsonProperty("dateOfCompletion")]
        public DateTimeOffset DateOfCompletion { get; set; }
    }
}
