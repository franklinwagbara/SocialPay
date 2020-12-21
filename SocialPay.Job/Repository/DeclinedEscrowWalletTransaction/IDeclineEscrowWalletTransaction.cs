using System.Threading.Tasks;

namespace SocialPay.Job.Repository.DeclinedEscrowWalletTransaction
{
    public interface IDeclineEscrowWalletTransaction
    {
        Task<string> GetPendingTransactions();
    }
}
