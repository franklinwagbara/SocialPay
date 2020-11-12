using System;

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
