using Microsoft.EntityFrameworkCore;
using SocialPay.Domain.Entities;

namespace SocialPay.Domain
{
    public class SocialPayDbContext : DbContext
    {
        public SocialPayDbContext(DbContextOptions<SocialPayDbContext> options) : base(options)
        { }
        public DbSet<ClientAuthentication> ClientAuthentication { get; set; }
        public DbSet<PinRequest> PinRequest { get; set; }
        public DbSet<MerchantBusinessInfo> MerchantBusinessInfo { get; set; }
        public DbSet<MerchantBankInfo> MerchantBankInfo { get; set; }
        public DbSet<MerchantActivitySetup> MerchantActivitySetup { get; set; }
        public DbSet<MerchantWallet> MerchantWallet { get; set; }
        public DbSet<CreateWalletResponse> CreateWalletResponse { get; set; }
        public DbSet<MerchantPaymentSetup> MerchantPaymentSetup { get; set; }
        public DbSet<CustomerTransaction> CustomerTransaction { get; set; }
        public DbSet<ItemAcceptedOrRejected> ItemAcceptedOrRejected { get; set; }
        public DbSet<CustomerOtherPaymentsInfo> CustomerOtherPaymentsInfo { get; set; }
        public DbSet<InvoicePaymentLink> InvoicePaymentLink { get; set; }
        public DbSet<LinkCategory> LinkCategory { get; set; }
        public DbSet<TransactionLog> TransactionLog { get; set; }
        public DbSet<InvoicePaymentInfo> InvoicePaymentInfo { get; set; }
        public DbSet<FailedTransactions> FailedTransactions { get; set; }
        public DbSet<AccountResetRequest> AccountResetRequest { get; set; }
        public DbSet<FioranoT24CardCreditRequest> FioranoT24CardCreditRequest { get; set; }
        public DbSet<FioranoT24TransactionResponse> FioranoT24TransactionResponse { get; set; }
        public DbSet<WalletTransferRequestLog> WalletTransferRequestLog { get; set; }
        public DbSet<WalletTransferResponse> WalletTransferResponse { get; set; }
        public DbSet<DisputeRequestLog> DisputeRequestLog { get; set; }
        public DbSet<PaymentResponse> PaymentResponse { get; set; }
        public DbSet<LoginAttemptHistory> LoginAttemptHistory { get; set; }
        public DbSet<ClientLoginStatus> ClientLoginStatus { get; set; }
        public DbSet<DefaultWalletTransferRequestLog> DefaultWalletTransferRequestLog { get; set; }
        public DbSet<CreditMerchantWalletTransferRequestLog> CreditMerchantWalletTransferRequestLog { get; set; }
        public DbSet<DebitMerchantWalletTransferRequestLog> DebitMerchantWalletTransferRequestLog { get; set; }
        public DbSet<NonEscrowFioranoT24Request> NonEscrowFioranoT24Request { get; set; }
        public DbSet<AcceptedEscrowWalletTransferRequestLog> AcceptedEscrowWalletTransferRequestLog { get; set; }
        public DbSet<AcceptedEscrowFioranoT24Request> AcceptedEscrowFioranoT24Request { get; set; }
        public DbSet<DeclinedWalletTransferRequestLog> DeclinedWalletTransferRequestLog { get; set; }
        public DbSet<InterBankTransactionRequest> InterBankTransactionRequest { get; set; }
        public DbSet<DeliveryDayWalletTransferRequestLog> DeliveryDayWalletTransferRequestLog { get; set; }
        public DbSet<FioranoT24DeliveryDayRequest> FioranoT24DeliveryDayRequest { get; set; }
        public DbSet<AcceptedEscrowInterBankTransactionRequest> AcceptedEscrowInterBankTransactionRequest { get; set; }
        public DbSet<DeliveryDayEscrowInterBankTransactionRequest> DeliveryDayEscrowInterBankTransactionRequest { get; set; }
        public DbSet<GuestAccountLog> GuestAccountLog { get; set; }
        public DbSet<OtherMerchantBankInfo> OtherMerchantBankInfo { get; set; }
        public DbSet<InvoicePaymentLinkToMulitpleEmails> InvoicePaymentLinkToMulitpleEmails { get; set; }
        public DbSet<MerchantStoreLog> MerchantStoreRequest { get; set; }
        public DbSet<MerchantQRCodeOnboardingResponse> MerchantQRCodeOnboardingResponse { get; set; }
        public DbSet<MerchantQRCodeOnboarding> MerchantQRCodeOnboarding { get; set; }
        public DbSet<SubMerchantQRCodeOnboarding> SubMerchantQRCodeOnboarding { get; set; }
        public DbSet<SubMerchantQRCodeOnboardingResponse> SubMerchantQRCodeOnboardingResponse { get; set; }
        public DbSet<BindMerchant> BindMerchant { get; set; }
        public DbSet<BindMerchantResponse> BindMerchantResponse { get; set; }
        public DbSet<MerchantStore> MerchantStore { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductItems> ProductItems { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductInventory> ProductInventory { get; set; }
        public DbSet<ProductInventoryHistory> productInventoryHistories { get; set; }
        public DbSet<QrPaymentRequest> QrPaymentRequest { get; set; }
        public DbSet<QrPaymentResponse> QrPaymentResponse { get; set; }
        public DbSet<SingleDstvPayment> SingleDstvPayment { get; set; }
        public DbSet<SingleDstvPaymentResponse> SingleDstvPaymentResponse { get; set; }
        public DbSet<DstvAccountLookup> DstvAccountLookup { get; set; }
        public DbSet<DstvAccountLookupResponse> DstvAccountLookupResponse { get; set; }
        public DbSet<FioranoBillsRequest> FioranoBillsRequest { get; set; }
        public DbSet<FioranoBillsPaymentResponse> FioranoBillsPaymentResponse { get; set; }
        public DbSet<WebHookRequest> WebHookRequest { get; set; }
        public DbSet<WebHookTransactionRequestLog> WebHookTransactionRequestLog { get; set; }
        public DbSet<StoreTransactionLog> StoreTransactionLog { get; set; }
        public DbSet<EventLog> EventLog { get; set; }
        public DbSet<AccountHistory> AccountHistory { get; set; }
        public DbSet<StoreTransactionLogDetails> StoreTransactionLogDetails { get; set; }
        public DbSet<VendAirtimeRequestLog> VendAirtimeRequestLog { get; set; }
        //public DbSet<TenantProfile> TenantProfile { get; set; }
        public DbSet<UssdServiceRequestLog> UssdServiceRequestLog { get; set; }
        public DbSet<MerchantTransactionSetup> MerchantTransactionSetup { get; set; }
        public DbSet<SpectaRegisterCustomerRequest> SpectaRegisterCustomerRequest { get; set; }
        public DbSet<SpectaRegisterCustomerResponse> SpectaRegisterCustomerResponse { get; set; }
        public DbSet<LoanDailyDeductionLog> LoanDailyDeductionLog { get; set; }
        public DbSet<ApplyForLoan> ApplyForLoan { get; set; }
        public DbSet<LoanDisbursement> LoanDisbursement { get; set; }
        public DbSet<LoanMonthlyDeductionLog> loanMonthlyDeductionLog { get; set; }
        public DbSet<LoanRepaymentPlan> LoanRepaymentPlan { get; set; }
        public DbSet<LoginAttemptHistory> LoginAttemptHistories { get; set; }
        public DbSet<CardTokenization> CardTokenization { get; set; }
        public DbSet<MerchantBureauSearch> MerchantBureauSearch { get; set; }
        public DbSet<SendEmailVerificationCodeRequest> SendEmailVerificationCodeRequest { get; set; }
        public DbSet<SendEmailVerificationCodeResponse> SendEmailVerificationCodeResponse { get; set; }
        public DbSet<VerifyEmailConfirmationCodeRequest> VerifyEmailConfirmationCodeRequest { get; set; }
        public DbSet<VerifyEmailConfirmationCodeResponse> VerifyEmailConfirmationCodeResponse { get; set; }
    }
}
