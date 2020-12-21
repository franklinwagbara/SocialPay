using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NonEscrowWalletTransaction
{
    public interface INonEscrowWalletTransaction
    {
        Task<string> GetPendingTransactions();
    }
}
