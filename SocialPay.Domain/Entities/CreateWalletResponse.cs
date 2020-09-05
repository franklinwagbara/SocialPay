using System;

namespace SocialPay.Domain.Entities
{
    public class CreateWalletResponse
    {
        public long CreateWalletResponseId { get; set; }
        public long ClientAuthenticationId { get; set; }
        public string Message { get; set; }
        public DateTime DateEntered { get; set; } = DateTime.Now;
        public virtual ClientAuthentication ClientAuthentication { get; set; }
    }
}
