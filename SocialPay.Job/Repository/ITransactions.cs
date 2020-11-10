using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository
{
    public interface ITransactions
    {
        Task<string> GetPendingTransactions();
    }
}
