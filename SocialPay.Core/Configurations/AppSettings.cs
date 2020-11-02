﻿namespace SocialPay.Core.Configurations
{
    public class AppSettings
    {
        public string appKey { get; set; }
        public string appId { get; set; }
        public string SecretKey { get; set; }
        public string SterlingBankCode { get; set; }
        public string BaseApiUrl { get; set; }
        public string EwsServiceUrl { get; set; }
        public string LdapServiceUrl { get; set; }
        public string IBSserviceUrl { get; set; }
        public string WebportalUrl { get; set; }
        public string BankServiceUrl { get; set; }
        public string nameEnquiryRequestType { get; set; }
        public string getBanksRequestType { get; set; }
        public string currencyCode { get; set; }
        public string walletBaseUrl { get; set; }
        public string createwalletUrl { get; set; }
        public string paymentlinkUrl { get; set; }
        public string walletExtensionUrl { get; set; }
        public string sterlingpaymentGatewayRequestUrl { get; set; }
        public string paywithSpectaBaseUrl { get; set; }
        public string paywithSpectaCallBackUrl { get; set; }
        public string paywithSpectaPurchaseUrlExtension { get; set; }
        public string mid { get; set; }
        public string paymentCombination { get; set; }
        public string deliverySLA { get; set; }
        public string MerchantLinkPaymentDocument { get; set; }
        public string PaymentLinkPercentage { get; set; }
        public string EmailTemplatesPath { get; set; }
        public string InvoiceTemplateDescription { get; set; }
        public string receiptTemplateDescription { get; set; }
        public string paywithspectamerchanId { get; set; }
        public string invoicePaymentlinkUrl { get; set; }
    }
}
