namespace SocialPay.Helper
{
    public class AppResponseCodes
    {
        public const string Success = "00";
        public const string InternalError = "02";
        public const string Failed = "03";
        public const string DuplicateEmail = "04";
        public const string RecordNotFound = "05";
        public const string InvalidLogin = "06";
        public const string MerchantInfoAlreadyExist = "07";               
        public const string DuplicateMerchantDetails = "08";               
        public const string InvalidAccountNo = "09";               
        public const string InActiveAccountNumber = "10";               
        public const string MerchantBusinessInfoRequired = "11";               
        public const string InterBankNameEnquiryFailed = "12";               
        public const string InvalidBVN = "13";               
    }

    public class MerchantOnboardingProcess
    {
        public const string CreateAccount = "01";
        public const string SignUp = "02";
        public const string BusinessInfo = "03";
        public const string BankInfo = "04";
    }

    public class MerchantWalletProcess
    {
        public const string CreateAccount = "01";
        public const string Processed = "02";       
    }

    public class RoleDetails
    {
        public const string Merchant = "Merchant";
        public const string SuperAdministrator = "Super Administrator";
        public const string Administrator = "Administrator";     
    }
}
