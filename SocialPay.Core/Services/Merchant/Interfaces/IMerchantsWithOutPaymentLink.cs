﻿using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Merchant.Interfaces
{
    public interface IMerchantsWithOutPaymentLink
    {
        public Task<WebApiResponse> MerchantsWithOutPaymentLink();
    }
}
