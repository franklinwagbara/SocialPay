using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Job.Repository.NonCardPayment
{
    public class OtherTransactions //: IOtherTransaction
    {
        public OtherTransactions(IServiceProvider services)
        {
            Services = services;
           // _transactions = transactions;
        }
        public IServiceProvider Services { get; }
    }
}
