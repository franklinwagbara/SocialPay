﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NotificationService
{
    public interface INotificationServices
    {
        Task<string> GetPendingTransactions();
    }
}
