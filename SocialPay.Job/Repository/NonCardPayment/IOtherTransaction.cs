using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NonCardPayment
{
    public interface IOtherTransaction
    {
        Task<string> GetPendingTransactions();
    }
}
