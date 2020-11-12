using System.Threading.Tasks;

namespace SocialPay.Job.Repository.DeliveryDayMerchantTransaction
{
    public interface IDeliveryDayMerchantTransfer
    {
        Task<string> GetPendingTransactions();
    }
}
