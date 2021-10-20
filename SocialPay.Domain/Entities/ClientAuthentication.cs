using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace SocialPay.Domain.Entities
{
    public class ClientAuthentication : BaseEntity
    {
        public ClientAuthentication()
        {
            PinRequest = new HashSet<PinRequest>();
            MerchantBusinessInfo = new HashSet<MerchantBusinessInfo>();
            MerchantBankInfo = new HashSet<MerchantBankInfo>();
            MerchantActivitySetup = new HashSet<MerchantActivitySetup>();
            MerchantWallet = new HashSet<MerchantWallet>();
            CreateWalletResponse = new HashSet<CreateWalletResponse>();
            MerchantPaymentSetup = new HashSet<MerchantPaymentSetup>();
            ItemAcceptedOrRejected = new HashSet<ItemAcceptedOrRejected>();
            InvoicePaymentLink = new HashSet<InvoicePaymentLink>();
            LinkCategory = new HashSet<LinkCategory>();
            TransactionLog = new HashSet<TransactionLog>();
            MerchantStore = new HashSet<MerchantStore>();
            ProductCategory = new HashSet<ProductCategory>();
            MerchantQRCodeOnboarding = new HashSet<MerchantQRCodeOnboarding>();
            QrPaymentRequest = new HashSet<QrPaymentRequest>();
            DstvAccountLookup = new HashSet<DstvAccountLookup>();
            SingleDstvPayment = new HashSet<SingleDstvPayment>();
            FioranoBillsRequest = new HashSet<FioranoBillsRequest>();
            WebHookRequest = new HashSet<WebHookRequest>();
            VendAirtimeRequestLog = new HashSet<VendAirtimeRequestLog>();
            TenantProfile = new HashSet<TenantProfile>();
            UssdServiceRequestLog = new HashSet<UssdServiceRequestLog>();
            MerchantTransactionSetup = new HashSet<MerchantTransactionSetup>();
            SpectaRegisterCustomerRequest = new HashSet<SpectaRegisterCustomerRequest>();
        }
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "NVARCHAR(11)")]
        public string Bvn { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string Email { get; set; }
        [Column(TypeName = "NVARCHAR(50)")]
        public string UserName { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string PhoneNumber { get; set; }
        [Column(TypeName = "NVARCHAR(55)")]
        public string FullName { get; set; }
        public byte[] ClientSecretHash { get; set; }
        public byte[] ClientSecretSalt { get; set; }
        [Column(TypeName = "NVARCHAR(5)")]
        public string StatusCode { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }
        [Column(TypeName = "NVARCHAR(25)")]
        public string RoleName { get; set; }
        [Column(TypeName = "NVARCHAR(30)")]
        public string ReferralCode { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string ReferCode { get; set; }
        [Column(TypeName = "NVARCHAR(20)")]
        public string QrCodeStatus { get; set; }
        public bool HasRegisteredCompany { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public DateTime LastDateModified { get; set; }
        public virtual ICollection<PinRequest> PinRequest { get; set; }
        public virtual ICollection<MerchantBusinessInfo> MerchantBusinessInfo { get; set; }
        public virtual ICollection<MerchantBankInfo> MerchantBankInfo { get; set; }
        public virtual ICollection<MerchantActivitySetup> MerchantActivitySetup { get; set; }
        public virtual ICollection<MerchantWallet> MerchantWallet { get; set; }
        public virtual ICollection<CreateWalletResponse> CreateWalletResponse { get; set; }
        public virtual ICollection<MerchantPaymentSetup> MerchantPaymentSetup { get; set; }
        public virtual ICollection<ItemAcceptedOrRejected> ItemAcceptedOrRejected { get; set; }
        public virtual ICollection<InvoicePaymentLink> InvoicePaymentLink { get; set; }
        public virtual ICollection<LinkCategory> LinkCategory { get; set; }
        public virtual ICollection<TransactionLog> TransactionLog { get; set; }
        public virtual ICollection<AccountResetRequest> AccountResetRequest { get; set; }
        public virtual ICollection<DisputeRequestLog> DisputeRequestLog { get; set; }
        public virtual ICollection<LoginAttemptHistory> LoginAttemptHistory { get; set; }
        public virtual ICollection<ClientLoginStatus> ClientLoginStatus { get; set; }
        public virtual ICollection<DefaultWalletTransferRequestLog> DefaultWalletTransferRequestLog { get; set; }
        public virtual ICollection<DebitMerchantWalletTransferRequestLog> DebitMerchantWalletTransferRequestLog { get; set; }
        public virtual ICollection<CreditMerchantWalletTransferRequestLog> CreditMerchantWalletTransferRequestLog { get; set; }
        public virtual ICollection<DeclinedWalletTransferRequestLog> DeclinedWalletTransferRequestLog { get; set; }
        public virtual ICollection<AcceptedEscrowWalletTransferRequestLog> AcceptedEscrowWalletTransferRequestLog { get; set; }
        public virtual ICollection<InterBankTransactionRequest> InterBankTransactionRequest { get; set; }
        public virtual ICollection<AcceptedEscrowFioranoT24Request> AcceptedEscrowFioranoT24Request { get; set; }
        public virtual ICollection<WalletTransferRequestLog> WalletTransferRequestLog { get; set; }
        public virtual ICollection<DeliveryDayWalletTransferRequestLog> DeliveryDayWalletTransferRequestLog { get; set; }
        public virtual ICollection<AcceptedEscrowInterBankTransactionRequest> AcceptedEscrowInterBankTransactionRequest { get; set; }
        public virtual ICollection<GuestAccountLog> GuestAccountLog { get; set; }
        public virtual ICollection<OtherMerchantBankInfo> OtherMerchantBankInfo { get; set; }
        public virtual ICollection<MerchantStore> MerchantStore { get; set; }
        public virtual ICollection<ProductCategory> ProductCategory { get; set; }
        public virtual ICollection<Product> Product { get; set; }
        public virtual ICollection<MerchantQRCodeOnboarding> MerchantQRCodeOnboarding { get; set; }
        public virtual ICollection<QrPaymentRequest> QrPaymentRequest { get; set; }
        public virtual ICollection<DstvAccountLookup> DstvAccountLookup { get; set; }
        public virtual ICollection<SingleDstvPayment> SingleDstvPayment { get; set; }
        public virtual ICollection<FioranoBillsRequest> FioranoBillsRequest { get; set; }
        public virtual ICollection<WebHookRequest> WebHookRequest { get; set; }
        public virtual ICollection<StoreTransactionLog> StoreTransactionLog { get; set; }
        public virtual ICollection<AccountHistory> AccountHistory { get; set; }
        public virtual ICollection<VendAirtimeRequestLog> VendAirtimeRequestLog { get; set; }
        public virtual ICollection<TenantProfile> TenantProfile { get; set; }
        public virtual ICollection<UssdServiceRequestLog> UssdServiceRequestLog { get; set; }
        public virtual ICollection<MerchantTransactionSetup> MerchantTransactionSetup { get; set; }
        public virtual ICollection<SpectaRegisterCustomerRequest> SpectaRegisterCustomerRequest { get; set; }
    }
}
