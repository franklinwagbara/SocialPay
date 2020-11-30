using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Core.Repositories.UserService
{
    public class UserRepoService
    {
        private readonly SocialPayDbContext _context;
        private readonly Utilities _utilities;
        private readonly AppSettings _appSettings;

        public UserRepoService(SocialPayDbContext context, Utilities utilities,
            IOptions<AppSettings> appSettings)
        {
            _context = context;
            _utilities = utilities;
            _appSettings = appSettings.Value;
        }

        public async Task<ClientAuthentication> GetClientAuthenticationAsync(string email)
        {
            return await _context.ClientAuthentication.SingleOrDefaultAsync(x => x.Email == email);
        }

        public async Task<ClientLoginStatus> GetLoginAttemptAsync(long clientId)
        {
            return await _context.ClientLoginStatus.SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);
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



        public async Task<WebApiResponse> ResetPassword(ResetExistingPasswordDto model, long clientId)
        {
            try
            {
                var userInfo = await GetClientAuthenticationClientIdAsync(clientId);
                if(userInfo == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.UserNotFound };

                if (!VerifyPasswordHash(model.CurrentPassword.Encrypt(_appSettings.appKey), userInfo.ClientSecretHash, userInfo.ClientSecretSalt))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidLogin };
                byte[] passwordHash, passwordSalt;
                _utilities.CreatePasswordHash(model.NewPassword.Encrypt(_appSettings.appKey), out passwordHash, out passwordSalt);

                userInfo.ClientSecretHash = passwordHash;
                userInfo.ClientSecretSalt = passwordSalt;
                userInfo.LastDateModified = DateTime.Now;
                _context.ClientAuthentication.Update(userInfo);
                await _context.SaveChangesAsync();
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };

            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }

            return true;
        }

        ///  if (!VerifyPasswordHash(loginRequestDto.Password.Encrypt(_appSettings.appKey), validateuserInfo.ClientSecretHash, validateuserInfo.ClientSecretSalt))
        ///return new LoginAPIResponse { ResponseCode = AppResponseCodes.InvalidLogin

    }
}
