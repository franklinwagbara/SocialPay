using System;
using System.Collections.Generic;

namespace SocialPay.Domain.Entities
{
    public class ClientAuthentication
    {
        public long ClientAuthenticationId { get; set; }
        public string Email { get; set; }
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
    }
}
