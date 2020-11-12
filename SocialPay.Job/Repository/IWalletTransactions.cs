using System.Threading.Tasks;

namespace SocialPay.Job.Repository
{
    public interface IWalletTransactions
    {
        Task<string> GetPendingTransactions();
    }
}
