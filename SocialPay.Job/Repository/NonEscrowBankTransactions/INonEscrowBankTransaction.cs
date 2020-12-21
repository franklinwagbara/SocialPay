using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NonEscrowBankTransactions
{
    public interface INonEscrowBankTransaction
    {
        Task<string> GetPendingTransactions();
    }
}
