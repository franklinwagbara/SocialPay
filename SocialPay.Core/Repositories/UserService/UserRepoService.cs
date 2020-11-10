using Microsoft.EntityFrameworkCore;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Repositories.UserService
{
    public class UserRepoService
    {
        private readonly SocialPayDbContext _context;

        public UserRepoService(SocialPayDbContext context)
        {
            _context = context;
        }

        public async Task<ClientAuthentication> GetClientAuthenticationAsync(string email)
        {
            return await _context.ClientAuthentication.SingleOrDefaultAsync(x => x.Email == email);
        }


        public async Task<WebApiResponse> LogAccountReset(long clientId, string token)
        {
            try
            {
                var resetRequest = new AccountResetRequest
                {
                    ClientAuthenticationId = clientId, IsCompleted = false, Token = token,
                    LastDateModified = DateTime.Now,
                };
                await _context.AccountResetRequest.AddAsync(resetRequest);
                await _context.SaveChangesAsync();
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
