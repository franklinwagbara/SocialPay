using System.Threading.Tasks;

namespace SocialPay.Job.Repository.DeliveryDayMerchantWalletTransaction
{
    public interface IDeliveryDayMerchantTransfer
    {
        Task<string> GetPendingTransactions();
    }
}
