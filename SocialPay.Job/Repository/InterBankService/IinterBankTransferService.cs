using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.InterBankService
{
    public interface IinterBankTransferService
    {
        Task<string> GetPendingTransactions();
    }
}
