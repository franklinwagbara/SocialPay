using System.Threading.Tasks;

namespace SocialPay.Job.Repository.AcceptedOrdersWalletTransaction
{
    public interface IAcceptedOrders
    {
        Task<string> GetPendingTransactions();
    }
}
