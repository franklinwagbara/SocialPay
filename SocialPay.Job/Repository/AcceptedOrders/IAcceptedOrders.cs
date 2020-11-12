using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.AcceptedOrders
{
    public interface IAcceptedOrders
    {
        Task<string> GetPendingTransactions();
    }
}
