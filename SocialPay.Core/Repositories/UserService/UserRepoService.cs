using Microsoft.EntityFrameworkCore;
using SocialPay.Core.Extensions.Common;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Threading.Tasks;

namespace SocialPay.Core.Repositories.UserService
{
    public class UserRepoService
    {
        private readonly SocialPayDbContext _context;
        private readonly Utilities _utilities;
        public UserRepoService(SocialPayDbContext context, Utilities utilities)
        {
            _context = context;
            _utilities = utilities;
        }

        public async Task<ClientAuthentication> GetClientAuthenticationAsync(string email)
        {
            return await _context.ClientAuthentication.SingleOrDefaultAsync(x => x.Email == email);
        }


        public async Task<ClientAuthentication> GetClientAuthenticationClientIdAsync(long clientId)
        {
            return await _context.ClientAuthentication.SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);
        }
        public async Task<AccountResetRequest> GetAccountResetAsync(string token)
        {
            return await _context.AccountResetRequest
                .SingleOrDefaultAsync(x => x.Token == token && x.IsCompleted == false);
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

        public async Task<WebApiResponse> ChangeUserPassword(PasswordResetDto model, int expiredTime, string appKey)
        {
            try
            {
                var getToken = await GetAccountResetAsync(model.Token);
                if (getToken == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                if (getToken.DateEntered.AddMinutes(expiredTime) < DateTime.Now)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.TokenExpired };

                var getUserInfo = await GetClientAuthenticationClientIdAsync(getToken.ClientAuthenticationId);

                byte[] passwordHash, passwordSalt;
                _utilities.CreatePasswordHash(model.NewPassword.Encrypt(appKey), out passwordHash, out passwordSalt);
                using(var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        getUserInfo.ClientSecretHash = passwordHash;
                        getUserInfo.ClientSecretSalt = passwordSalt;
                        _context.Update(getUserInfo);
                        await _context.SaveChangesAsync();
                        getToken.IsCompleted = true;
                        getToken.LastDateModified = DateTime.Now;
                        _context.Update(getToken);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                    }
                }
               
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
