using SocialPay.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Core.Services.Store
{
    public class StoreInfoRepository
    {
        private readonly SocialPayDbContext _context;
        public StoreInfoRepository(SocialPayDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

       // public async Task<Web>
    }
}
