using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NibbsMerchantJobService.Interface
{
    public interface ICreateNibbsMerchantService
    {
        Task<string> GetPendingTransactions();
    }
}
