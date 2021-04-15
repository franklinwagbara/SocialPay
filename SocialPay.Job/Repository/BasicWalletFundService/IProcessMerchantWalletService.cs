using System.Threading.Tasks;

namespace SocialPay.Job.Repository.BasicWalletFundService
{
    public interface IProcessMerchantWalletService
    {
        Task<string> GetPendingTransactions();
    }
}
