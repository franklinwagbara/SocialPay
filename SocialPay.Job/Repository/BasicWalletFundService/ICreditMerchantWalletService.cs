using System.Threading.Tasks;

namespace SocialPay.Job.Repository.BasicWalletFundService
{
    public interface ICreditMerchantWalletService
    {
        Task<string> GetPendingTransactions();
    }
}
