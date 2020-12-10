using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace SocialPay.Domain.Entities
{
    public class ClientAuthentication
    {
        public long ClientAuthenticationId { get; set; }
        [Column(TypeName = "VARCHAR(30)")]
        public string Email { get; set; }
        [Column(TypeName = "VARCHAR(20)")]
        public string UserName { get; set; }
        [Column(TypeName = "VARCHAR(15)")]
        public string PhoneNumber { get; set; }
        [Column(TypeName = "VARCHAR(45)")]
        public string FullName { get; set; }
        public byte[] ClientSecretHash { get; set; }
        public byte[] ClientSecretSalt { get; set; }
        [Column(TypeName = "VARCHAR(5)")]
        public string StatusCode { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsLocked { get; set; }
        [Column(TypeName = "VARCHAR(25)")]
        public string RoleName { get; set; }
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
    }
}
