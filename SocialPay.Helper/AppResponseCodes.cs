﻿namespace SocialPay.Helper
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
        public const string FailedCreatingWallet = "14";
        public const string MerchantBankInfoRequired = "15";
        public const string InvalidADUser = "16";               
        public const string UserNotFoundOnAD = "17";               
        public const string InvalidPaymentLink = "18";               
        public const string InvalidPaymentReference = "19";               
        public const string CancelHasExpired = "20";               
        public const string InvalidPaymentcategory = "21";               
        public const string InvalidPamentChannel = "22";               
        public const string InvalidTransactionReference = "23";               
        public const string IncompleteMerchantProfile = "24";               
        public const string DuplicateInvoiceName = "25";               
    }

    public class MerchantOnboardingProcess
    {
        public const string CreateAccount = "01";
        public const string SignUp = "02";
        public const string BusinessInfo = "03";
        public const string BankInfo = "04";
        public const string Customer = "05";
    }

    public class MerchantWalletProcess
    {
        public const string CreateAccount = "01";
        public const string Processed = "02";       
    }

    public class PaymentChannel
    {
        public const string OneBank = "01";
        public const string Card = "02";
    }

    public class MerchantPaymentCategory
    {
        public const string Basic = "01";
        public const string Escrow = "02";
        public const string OneOffBasicLink = "03";
        public const string OneOffEscrowLink = "04";
    }


    public class OrderStatusCode
    {
        public const string Pending = "01";
        public const string Approved = "02";
        public const string Decline = "03";
    }
    public class RoleDetails
    {
        public const string Merchant = "Merchant";
        public const string SuperAdministrator = "Super Administrator";
        public const string Administrator = "Administrator";     
        public const string CustomerAccount = "Guest";     
    }
}
