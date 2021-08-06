using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NibbsMerchantJobService.Interface
{
    public interface ICreateNibbsSubMerchantService
    {
        Task<string> GetPendingTransactions();
    }
}
