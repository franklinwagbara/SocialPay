using System;
using System.Collections.Generic;

namespace SocialPay.Domain.Entities
{
    public class ClientAuthentication
    {
        public long ClientAuthenticationId { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }
        public byte[] ClientSecretHash { get; set; }
        public byte[] ClientSecretSalt { get; set; }
        public string StatusCode { get; set; }
        public bool IsDeleted { get; set; }
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
        public virtual ICollection<WalletTransferRequestLog> WalletTransferRequestLog { get; set; }
    }
}
