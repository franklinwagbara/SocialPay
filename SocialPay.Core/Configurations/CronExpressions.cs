namespace SocialPay.Core.Configurations
{
    public class CronExpressions
    {
        public string AcceptedEscrowBankOrderTask { get; set; }
        public string AcceptedWalletOrderTask { get; set; }
        public string CardPaymentTask { get; set; }
        public string CreditDefaultMerchantWalletTask { get; set; }
        public string DeclinedEscrowWalletTask { get; set; }
        public string DeliveryDayBankTask { get; set; }
        public string DeliveryDayWalletTask { get; set; }
        public string ExpiredProductNotificationTask { get; set; }
        public string NonEscrowOtherWalletTransactionTask { get; set; }
        public string NonEscrowWalletTransactionTask { get; set; }
        public string NonEscrowBankTransactionTask { get; set; }
        public string ProcessFailedMerchantWalletTask { get; set; }
        public string CreateNibbsMerchantTask { get; set; }
    }
}
