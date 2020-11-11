using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository
{
    public interface IWalletTransactions
    {
        Task<string> GetPendingTransactions();
    }
}
