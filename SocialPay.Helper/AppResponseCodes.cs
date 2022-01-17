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
        public const string FailedCreatingWallet = "14";
        public const string MerchantBankInfoRequired = "15";
        public const string InvalidADUser = "16";               
        public const string UserNotFoundOnAD = "17";               
        public const string InvalidPaymentLink = "18";               
        public const string InvalidPaymentReference = "19";               
        public const string OrderHasExpired = "20";               
        public const string InvalidPaymentcategory = "21";               
        public const string InvalidPamentChannel = "22";               
        public const string InvalidTransactionReference = "23";               
        public const string IncompleteMerchantProfile = "24";               
        public const string DuplicateInvoiceName = "25";               
        public const string InvalidConfirmation = "26";               
        public const string TransactionAlreadyexit = "27";               
        public const string UserNotFound = "28";               
        public const string TransactionFailed = "29";               
        public const string DuplicateTransaction = "30";               
        public const string WalletExist = "31";               
        public const string TokenExpired = "32";               
        public const string FiranoDebitError = "33";               
        public const string AccountLockFailed = "34";               
        public const string TransactionProcessed = "35";               
        public const string AccountIsLocked = "36";               
        public const string DuplicatePassword = "37";               
        public const string OtpExpired = "38";               
        public const string NipFeesCalculationFailed = "39";               
        public const string TinValidationFailed = "40";               
        public const string InsufficientFunds = "41";               
        public const string IncorrectTransactionPin = "42";               
        public const string DuplicateLinkName = "43";               
        public const string AgeNotWithinRange = "44";               
        public const string InvalidBVNDateOfBirth = "45";               
        public const string BvnValidationError = "46";               
        public const string MerchantDefaultBankInfoNotFound = "47";
        public const string DuplicateStoreName = "48";
        public const string DuplicateCategoryName = "49";
        public const string DuplicateProductName = "50";
        public const string DuplicateTin = "51";
        public const string DuplicatePaymentReference = "52";
        public const string QRMerchantOnboardingNotFoundOrCompleted = "53";
        public const string InvalidBVNDetails = "54";
        public const string InvalidBVNDetailsFirstNameOrLastName = "55";
        public const string InvalidCSVFormat = "56";
        public const string PasswordAlreadyUsed = "57";
        public const string DuplicatePinSetup = "58";
        public const string TransactionPinDoesNotExit = "59";
        public const string InvalidInvoiceDate = "60";
    }

    public class SpectaProcessCodes
    {
        public const string RegisterCustomer = "01";
        public const string SendEmailVerificationCode = "02";
        public const string VerifyEmailConfirmationCode = "03";
        public const string SendBvnPhoneVerificationCode = "04";
        public const string VerifyBvnPhoneConfirmationCode = "05";
        public const string AddOrrInformation = "06";
        public const string RequestTicket = "07";
        public const string ConfirmTicket = "08";
        public const string CreateIndividualCurrentAccount = "09";
        public const string SetDisbursementAccount = "10";
        public const string ChargeCard = "11";
        public const string SendPhone = "12";
        public const string SendOtp = "13";
        public const string SendPin = "14";
        public const string ValidateCharge = "15";
        public const string Failed = "16";
        public const string success = "00";

    }


    public class MerchantOnboardingProcess
    {
        public const string CreateAccount = "01";
        public const string SignUp = "02";
        public const string BusinessInfo = "03";
        public const string BankInfo = "04";
        public const string GuestAccount = "05";
        public const string Wallet = "06";
        public const string SuperAdmin = "07";
    }

    public class MerchantWalletProcess
    {
        public const string CreateAccount = "01";
        public const string Processed = "02";       
    }

    public class StoreTransactionRequestProcess
    {
        public const string InitiateRequest = "01";
        public const string FailedRequest = "02";
        public const string ConfirmedTransaction = "00";
    }
    public class NibbsMerchantOnboarding
    {
        public const string CreateAccount = "01";
        public const string SubAccount = "02";
        public const string BindMerchant = "00";
        public const string NotProfiled = "03";
    }

    public class PaymentChannel
    {
        public const string OneBank = "01";
        public const string Card = "02";
        public const string PayWithSpecta = "03";
        public const string NibbsQR = "04";
        public const string USSD = "05";
    }
    public class MerchantLinkCategory
    {
        public const string Single = "01";
        public const string Store = "02";
    }
    public class MerchantPaymentLinkCategory
    {
        public const string Basic = "01";
        public const string Escrow = "02";
        public const string OneOffBasicLink = "03";
        public const string OneOffEscrowLink = "04";
        public const string InvoiceLink = "05";
    }

    public class WalletTransferMode
    {
        public const string MerchantToSocialPay = "01";
        public const string SocialPayToMerchant = "02";
    }

    public class CustomerPaymentCategory
    {
        public const string BasicLink = "01";
        public const string StoreLink = "02";
    }


    public class AcceptRejectRequest
    {
        public const string Guest = "01";
        public const string Merchant = "02";
    }
    //public class OrderStatusCode
    //{
    //    public const string Pending = "01";
    //    public const string Approved = "02";
    //    public const string Decline = "03";
    //    public const string Dispute = "04";
    //    public const string WalletFundingProgress = "05";
    //    public const string CompletedWalletFunding = "06";
    //    public const string CompletedDirectFundTransfer = "07";
    //    public const string Failed = "08";
    //    public const string BankTransferProcessing = "09";
    //    public const string TransactionCompleted = "10";
    //    public const string ItemAccepted = "11";

    //}

    public class TransactionJourneyStatusCodes
    {
        public const string Pending = "01";
        public const string Approved = "02";
        public const string Decline = "03";
        public const string Dispute = "04";
        public const string WalletFundingProgress = "05";
        public const string CompletedWalletFunding = "06";
        public const string CompletedDirectFundTransfer = "07";
        public const string FirstWalletFundingWasSuccessul = "08";
        public const string BankTransferProcessing = "09";
        public const string TransactionCompleted = "10";
        public const string ItemAccepted = "11";
        public const string TransactionFailed = "12";
        public const string BasicWalletFundingProcessing = "13";
        public const string BasicCompletedWalletFunding = "14"; 
        public const string FioranoFirstFundingProcessing = "15"; 
        public const string FioranoFirstFundingCompleted = "16"; 
        public const string ProcessingApprovedRequest = "17"; 
        public const string OrderWasAcceptedWalletDebited = "18"; 
        public const string AwaitingCustomerFeedBack = "19"; 
        public const string CompletedDeliveryDayWalletFunding = "20"; 
        public const string WalletTranferCompleted = "21"; 
        public const string ProcessingFinalWalletRequest = "22"; 
        public const string CompletedBankTransfer = "23"; 
        public const string ProcessingRejectedRequest = "24"; 
        public const string WalletTranferCompletedForRefund = "25"; 
        public const string WalletFundingProgressFinalDeliveryDay = "26"; 
        public const string DuplicateTransaction = "27"; 
        public const string RecordNotFound = "28"; 
    }
    public class RoleDetails
    {
        public const string Merchant = "Merchant";
        public const string SuperAdministrator = "Super Administrator";
        public const string Administrator = "Administrator";     
        public const string CustomerAccount = "Guest";     
    }

    public class TransactionType
    {
        public const string StorePayment = "01";
        public const string OtherPayment = "02";
    }

    public class EventLogProcess
    {
        public const string Login = "Login";      
        public const string BvnValidation = "Bvn Sign Validation Request";      
        public const string MerchantSignUp = "Merchant Signup";      
        public const string MerchantBusinessInfo = "Created Merchant Business Info";      
        public const string MerchantSignUpConfirmation = "Merchant Signup Confirmation";      
        public const string CreateStore = "Create new store";      
        public const string UnlockAccount = "Unlock account profile";      
    }

    public class ResponseCodes
    {
        public const int Success = 200;
        public const int Badrequest = 400;
        public const int RecordNotFound = 404;
        public const int Duplicate = 409; 
        public const int InternalError = 500;      
    }
}
