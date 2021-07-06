using SocialPay.Core.Services.Wallet;

namespace SocialPay.Job.Repository.Wallet
{
    public class WalletCoreService
    {

        private readonly WalletRepoService _walletRepoService;
        public WalletCoreService(WalletRepoService walletRepoService)
        {
            _walletRepoService = walletRepoService;
        }
    }
}
