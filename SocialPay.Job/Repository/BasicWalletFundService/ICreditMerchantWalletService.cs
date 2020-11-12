using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.BasicWalletFundService
{
    public interface ICreditMerchantWalletService
    {
        Task<string> GetPendingTransactions();
    }
}
