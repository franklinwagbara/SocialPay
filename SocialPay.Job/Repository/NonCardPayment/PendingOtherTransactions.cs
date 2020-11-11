using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Job.Repository.NonCardPayment
{
    public class PendingOtherTransactions
    {
        public PendingOtherTransactions(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }
    }
}
