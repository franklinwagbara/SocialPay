using SocialPay.Core.Configurations;
using SocialPay.Core.Services.Wallet;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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

        public async Task<WalletToWalletResponseDto> InitiateWalletToWalletTransfer(WalletTransferRequestDto model)
        {
            try
            {
                var fundsTransfer = await _walletRepoService.WalletToWalletTransferAsync(model);
                return fundsTransfer;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
