using System.Threading.Tasks;

namespace SocialPay.Job.Repository.IntraBankService
{
    public interface IIntraBankTransferService
    {
        Task<string> GetPendingTransactions();
    }
}
