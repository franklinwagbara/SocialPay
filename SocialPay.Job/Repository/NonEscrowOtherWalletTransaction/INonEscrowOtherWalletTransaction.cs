using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NonEscrowOtherWalletTransaction
{
    public interface INonEscrowOtherWalletTransaction
    {
        Task<string> GetPendingTransactions();
    }
}
