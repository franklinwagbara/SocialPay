using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NonCardPayment
{
    public interface IOtherTransaction
    {
        Task<string> GetPendingTransactions();
    }
}
