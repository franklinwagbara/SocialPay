using System.Threading.Tasks;

namespace SocialPay.Job.Repository.AcceptedEscrowOrdersWalletTransaction
{
    public interface IAcceptedEscrowOrders
    {
        Task<string> GetPendingTransactions();
    }
}
