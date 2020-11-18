using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.AcceptedEscrowOrdersBankTransaction
{
    public interface IAcceptedEscrowRequestBankTransaction
    {
        Task<string> GetPendingTransactions();
    }
}
