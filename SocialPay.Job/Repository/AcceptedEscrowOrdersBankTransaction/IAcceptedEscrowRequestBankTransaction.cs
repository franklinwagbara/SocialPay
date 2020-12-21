using System.Threading.Tasks;

namespace SocialPay.Job.Repository.AcceptedEscrowOrdersBankTransaction
{
    public interface IAcceptedEscrowRequestBankTransaction
    {
        Task<string> GetPendingTransactions();
    }
}
