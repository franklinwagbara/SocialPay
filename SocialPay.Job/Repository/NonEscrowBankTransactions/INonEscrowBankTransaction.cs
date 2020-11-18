using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NonEscrowBankTransactions
{
    public interface INonEscrowBankTransaction
    {
        Task<string> GetPendingTransactions();
    }
}
