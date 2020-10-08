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
    }
}
