using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Messaging;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Repositories.UserService
{
    public class UserRepoService
    {
        private readonly SocialPayDbContext _context;
        private readonly Utilities _utilities;
        private readonly AppSettings _appSettings;
        private readonly EmailService _emailService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(UserRepoService));


        public UserRepoService(SocialPayDbContext context, Utilities utilities,
            IOptions<AppSettings> appSettings, EmailService emailService)
        {
            _context = context;
            _utilities = utilities;
            _appSettings = appSettings.Value;
            _emailService = emailService;
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
                _log4net.Error("LogAccountReset request" + " | " + clientId + " | " + token + " | " + DateTime.Now);

                var resetRequest = new AccountResetRequest
                {
                    ClientAuthenticationId = clientId, IsCompleted = false, Token = token,
                    LastDateModified = DateTime.Now,
                };

                await _context.AccountResetRequest.AddAsync(resetRequest);
                await _context.SaveChangesAsync();

                _log4net.Error("LogAccountReset request saved" + " | " + clientId + " | " + token + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "LogAccountReset" + " | " + clientId + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> ChangeUserPassword(PasswordResetDto model, int expiredTime, string appKey)
        {
            _log4net.Info("ChangeUserPassword request" + " | " + model.Token + " | " + appKey + " | " + DateTime.Now);

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
                        _log4net.Info("ChangeUserPassword request saved" + " | " + model.Token + " | " + appKey + " | " + DateTime.Now);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        _log4net.Error("Error occured" + " | " + "ChangeUserPassword" + " | " + model.Token + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                    }
                }
               
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "ChangeUserPassword" + " | " + model.Token + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> ResendGuestAccountDetails(GuestAccountRequestDto model, long clientId)
        {
            try
            {
                _log4net.Info("Resend guest account" + " | " + model.Email + " | " + DateTime.Now);

                var newPassword = Guid.NewGuid().ToString();

                var userInfo = await GetClientAuthenticationAsync(model.Email);
                if (userInfo == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.UserNotFound };

                byte[] passwordHash, passwordSalt;
                _utilities.CreatePasswordHash(newPassword.Encrypt(_appSettings.appKey), out passwordHash, out passwordSalt);

                userInfo.ClientSecretHash = passwordHash;
                userInfo.ClientSecretSalt = passwordSalt;
                userInfo.LastDateModified = DateTime.Now;
                _context.ClientAuthentication.Update(userInfo);
                await _context.SaveChangesAsync();
                _log4net.Info("Resend guest account request updated" + " | " + clientId + " | " + DateTime.Now);


                var emailModal = new EmailRequestDto
                {
                    Subject = "Guest Account Access",
                    SourceEmail = "info@sterling.ng",
                    DestinationEmail = model.Email,
                    // DestinationEmail = "festypat9@gmail.com",
                };

                var mailBuilder = new StringBuilder();
                mailBuilder.AppendLine("Dear" + " " + model.Email + "," + "<br />");
                mailBuilder.AppendLine("<br />");
                mailBuilder.AppendLine("You have successfully sign up as a Guest.<br />");
                mailBuilder.AppendLine("Kindly use this token" + "  " + newPassword + "  " + "to login" + " " + "" + "<br />");
                mailBuilder.AppendLine("Best Regards,");
                emailModal.EmailBody = mailBuilder.ToString();

                var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "Resend guest account" + " | " + clientId + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> ResetPassword(ResetExistingPasswordDto model, long clientId)
        {
            try
            {
                _log4net.Info("ResetPassword request" + " | " + clientId + " | " +  DateTime.Now);

                var userInfo = await GetClientAuthenticationClientIdAsync(clientId);
                if(userInfo == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.UserNotFound };

                if (!VerifyPasswordHash(model.CurrentPassword.Encrypt(_appSettings.appKey), userInfo.ClientSecretHash, userInfo.ClientSecretSalt))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidLogin };

                if (VerifyPasswordHash(model.NewPassword.Encrypt(_appSettings.appKey), userInfo.ClientSecretHash, userInfo.ClientSecretSalt))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicatePassword };

                byte[] passwordHash, passwordSalt;
                _utilities.CreatePasswordHash(model.NewPassword.Encrypt(_appSettings.appKey), out passwordHash, out passwordSalt);

                userInfo.ClientSecretHash = passwordHash;
                userInfo.ClientSecretSalt = passwordSalt;
                userInfo.LastDateModified = DateTime.Now;
                _context.ClientAuthentication.Update(userInfo);
                await _context.SaveChangesAsync();
                _log4net.Info("ResetPassword request saved" + " | " + clientId + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "ResetPassword" + " | " + clientId + " | " + ex.Message.ToString() + " | " + DateTime.Now);

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
