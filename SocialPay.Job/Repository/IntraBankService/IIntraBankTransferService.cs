using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.IntraBankService
{
    public interface IintraBankTransferService
    {
        Task<string> GetPendingTransactions();
    }
}
