using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NonEscrowCardWalletTransaction
{
    public interface INonEscrowCardWalletTransaction
    {
        Task<string> GetPendingTransactions();
    }
}
