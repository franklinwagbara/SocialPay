using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NonEscrowWalletTransaction
{
    public interface INonEscrowWalletTransaction
    {
        Task<string> GetPendingTransactions();
    }
}
