using System.Threading.Tasks;

namespace SocialPay.Job.Repository.DeliveryDayBankTransaction
{
    public interface IDeliveryDayBankTransaction
    {
        Task<string> GetPendingTransactions();
    }
}
