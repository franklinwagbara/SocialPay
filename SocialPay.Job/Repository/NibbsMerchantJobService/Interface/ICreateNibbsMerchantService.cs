using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NibbsMerchantJobService.Interface
{
    public interface ICreateNibbsMerchantService
    {
        Task<string> GetPendingTransactions();
    }
}
