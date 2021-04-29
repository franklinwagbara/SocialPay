using SocialPay.Core.Services.Wallet;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Threading.Tasks;

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
