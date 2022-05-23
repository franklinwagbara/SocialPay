using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.PayWithCard
{
    public interface ISettleCardPayment
    {
        Task<string> ProcessSettlement();
    }
}
