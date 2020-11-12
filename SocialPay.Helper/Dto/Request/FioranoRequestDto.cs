namespace SocialPay.Helper.Dto.Request
{
    public class FioranoRequestDto
    {
    }

    public class FTRequest
    {
        public string TransactionBranch { get; set; }
        public string TransactionType { get; set; }
        public string DebitAcctNo { get; set; }
        public string DebitCurrency { get; set; }
        public string CreditCurrency { get; set; }
        public string DebitAmount { get; set; }
        public string CreditAccountNo { get; set; }
        public string CommissionCode { get; set; }
        public string VtellerAppID { get; set; }
        public string narrations { get; set; }
        public string SessionId { get; set; }
        public string TrxnLocation { get; set; }
    }

    public class TransactionRequestDto
    {
        public FTRequest FT_Request { get; set; }
    }
}
