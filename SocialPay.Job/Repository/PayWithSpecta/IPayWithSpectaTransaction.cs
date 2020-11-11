using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.PayWithSpecta
{
    public interface IPayWithSpectaTransaction
    {
        Task<string> GetPendingTransactions();
    }
}
