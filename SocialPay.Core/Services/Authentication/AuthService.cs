using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Repositories.UserService;
using SocialPay.Core.Services.Account;
using SocialPay.Core.Services.Wallet;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Authentication
{
    public class AuthRepoService : BaseService<ClientAuthentication>
    {
        private readonly SocialPayDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly Utilities _utilities;
        private readonly ADRepoService _aDRepoService;
        private readonly WalletRepoService _walletRepoService;
        private readonly UserRepoService _userRepoService;
        private readonly IDistributedCache _distributedCache;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AuthRepoService));

        public AuthRepoService(SocialPayDbContext context, IOptions<AppSettings> appSettings,
            Utilities utilities, ADRepoService aDRepoService, IDistributedCache distributedCache,
            WalletRepoService walletRepoService, UserRepoService userRepoService) : base(context)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _utilities = utilities;
            _aDRepoService = aDRepoService;
            _distributedCache = distributedCache;
            _walletRepoService = walletRepoService;
            _userRepoService = userRepoService;
        }

        public async Task<ClientAuthentication> GetClientDetails(string email)
        {
            return await _context.ClientAuthentication.SingleOrDefaultAsync(p => p.Email
            == email
            );
        }


        public async Task<ClientAuthentication> GetClientDetailsByClientId(long clientId)
        {
            return await _context.ClientAuthentication.SingleOrDefaultAsync(p => p.ClientAuthenticationId
            == clientId
            );
        }

        public async Task<LoginAPIResponse> Authenticate(LoginRequestDto loginRequestDto)
        {
            _log4net.Info("Authenticate" + " | " + loginRequestDto.Email + " | " +  DateTime.Now);

            try
            {
                var cacheKey = string.Empty;
                var userInfo = new UserInfoViewModel{};
                if (string.IsNullOrEmpty(loginRequestDto.Email) || string.IsNullOrEmpty(loginRequestDto.Password))
                    return new LoginAPIResponse { ResponseCode = AppResponseCodes.Failed};


                var validateuserInfo = await _context.ClientAuthentication
                    .Include(x=>x.MerchantBusinessInfo)
                    .Include(x=>x.MerchantWallet)
                    .Include(x=>x.MerchantBankInfo)
                    .SingleOrDefaultAsync(x => x.Email == loginRequestDto.Email 
                    && x.IsDeleted == false);

                // check if username exists
                if (validateuserInfo == null)
                    return new LoginAPIResponse { ResponseCode = AppResponseCodes.InvalidLogin };

                if(validateuserInfo.IsLocked == true)
                    return new LoginAPIResponse { ResponseCode = AppResponseCodes.AccountIsLocked };

                var userLoginAttempts = await _userRepoService.GetLoginAttemptAsync(validateuserInfo.ClientAuthenticationId);

                var tokenResult = new LoginAPIResponse();
                var key = Encoding.ASCII.GetBytes(_appSettings.SecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor();
                var tokenHandler = new JwtSecurityTokenHandler();

                if (validateuserInfo.RoleName == "Super Administrator")
                {
                    _log4net.Info("Authenticate for super admin" + " | " + loginRequestDto.Email + " | " + DateTime.Now);

                    var validateUserAD = await _aDRepoService.ValidateUserAD(validateuserInfo.UserName, loginRequestDto.Password);
                    if(validateUserAD.ResponseCode != AppResponseCodes.Success)
                    return validateUserAD;

                    tokenDescriptor = new SecurityTokenDescriptor
                    {
                     Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, validateuserInfo.Email),
                    new Claim(ClaimTypes.Role, validateuserInfo.RoleName),
                    new Claim(ClaimTypes.Email, validateuserInfo.Email),
                    new Claim("UserStatus",  validateuserInfo.StatusCode),
                    new Claim(ClaimTypes.NameIdentifier,  Convert.ToString(validateuserInfo.ClientAuthenticationId)),

                    }),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var adtoken = tokenHandler.CreateToken(tokenDescriptor);
                    var adtokenString = tokenHandler.WriteToken(adtoken);
                    tokenResult.AccessToken = adtokenString;
                    tokenResult.ClientId = validateuserInfo.Email;
                    tokenResult.Role = validateuserInfo.RoleName;
                    tokenResult.UserStatus = validateuserInfo.StatusCode;
                    tokenResult.ResponseCode = AppResponseCodes.Success;
                   
                    return tokenResult;
                }
                // check if password is correct
                if (!VerifyPasswordHash(loginRequestDto.Password.Encrypt(_appSettings.appKey), validateuserInfo.ClientSecretHash, validateuserInfo.ClientSecretSalt))
                {
                    using(var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            userLoginAttempts.LoginAttempt ++;
                            userLoginAttempts.IsSuccessful = false;
                            _context.ClientLoginStatus.Update(userLoginAttempts);
                            await _context.SaveChangesAsync();
                            var loginDetails = new LoginAttemptHistory
                            {
                                ClientAuthenticationId = userLoginAttempts.ClientAuthenticationId
                            };
                            await _context.LoginAttemptHistory.AddAsync(loginDetails);
                            await _context.SaveChangesAsync();

                            if(userLoginAttempts.LoginAttempt == Convert.ToInt32(_appSettings.clientloginAttempts))
                            {
                                validateuserInfo.IsLocked = true;
                                validateuserInfo.LastDateModified = DateTime.Now;
                                _context.ClientAuthentication.Update(validateuserInfo);
                                await _context.SaveChangesAsync();
                                await transaction.CommitAsync();
                                _log4net.Info("Authenticate for login was successful" + " | " + loginRequestDto.Email + " | " + DateTime.Now);

                                return new LoginAPIResponse { ResponseCode = AppResponseCodes.AccountIsLocked };
                            }
                            await transaction.CommitAsync();
                            _log4net.Info("Authenticate for login was successful" + " | " + loginRequestDto.Email + " | " + DateTime.Now);

                            return new LoginAPIResponse { ResponseCode = AppResponseCodes.InvalidLogin };
                        }
                        catch (Exception ex)
                        {
                            _log4net.Error("Error occured" + " | " + "Authenticate" + " | " + loginRequestDto.Email + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                            return new LoginAPIResponse { ResponseCode = AppResponseCodes.InternalError };
                        }
                    }
                }

                userLoginAttempts.LoginAttempt = 0;
                userLoginAttempts.IsSuccessful = true;
                _context.ClientLoginStatus.Update(userLoginAttempts);
                await _context.SaveChangesAsync();
                validateuserInfo.IsLocked = false;
                _context.ClientAuthentication.Update(validateuserInfo);
                await _context.SaveChangesAsync();

                if (validateuserInfo.RoleName == "Guest")
                {
                    tokenDescriptor = new SecurityTokenDescriptor
                    {
                       Subject = new ClaimsIdentity(new Claim[]
                       {
                        new Claim(ClaimTypes.Name, validateuserInfo.Email),
                        new Claim(ClaimTypes.Role, validateuserInfo.RoleName),
                        new Claim(ClaimTypes.Email, validateuserInfo.Email),
                        new Claim("UserStatus",  validateuserInfo.StatusCode),
                       // new Claim("businessName",  validateuserInfo.MerchantBusinessInfo.Select(x => x.BusinessName).FirstOrDefault()),
                        new Claim(ClaimTypes.NameIdentifier,  Convert.ToString(validateuserInfo.ClientAuthenticationId)),

                       }),
                        Expires = DateTime.UtcNow.AddDays(1),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    cacheKey = Convert.ToString(validateuserInfo.ClientAuthenticationId);
                    userInfo.Email = validateuserInfo.Email;
                    userInfo.StatusCode = validateuserInfo.StatusCode;
                    string serializedCustomerListGuest = string.Empty;
                    var redisCustomerListGuest = await _distributedCache.GetAsync(cacheKey);
                    if (redisCustomerListGuest == null)
                    {
                        await _distributedCache.RemoveAsync(cacheKey);
                        serializedCustomerListGuest = JsonConvert.SerializeObject(userInfo);
                        redisCustomerListGuest = Encoding.UTF8.GetBytes(serializedCustomerListGuest);
                        var options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(20))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(10));
                        await _distributedCache.SetAsync(cacheKey, redisCustomerListGuest, options);
                    }
                    var guestToken = tokenHandler.CreateToken(tokenDescriptor);
                    var guestTokenString = tokenHandler.WriteToken(guestToken);
                    tokenResult.AccessToken = guestTokenString;
                    tokenResult.ClientId = validateuserInfo.Email;
                    tokenResult.Role = validateuserInfo.RoleName;
                    tokenResult.UserStatus = validateuserInfo.StatusCode;
                    tokenResult.ResponseCode = AppResponseCodes.Success;                    
                    tokenResult.PhoneNumber = validateuserInfo.PhoneNumber;
                    _log4net.Info("Authenticate for login was successful" + " | " + loginRequestDto.Email + " | " + DateTime.Now);

                    return tokenResult;
                }

                double availableWalletBalance = 0;
                var getwalletInfo = await _walletRepoService.GetWalletDetailsAsync(validateuserInfo.PhoneNumber);

                if(getwalletInfo != null)
                {
                    availableWalletBalance = getwalletInfo.Data == null ? 0 : getwalletInfo.Data.Availablebalance;
                }

                tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                   {
                    new Claim(ClaimTypes.Name, validateuserInfo.Email),
                    new Claim(ClaimTypes.Role, validateuserInfo.RoleName),
                    new Claim(ClaimTypes.Email, validateuserInfo.Email),
                    new Claim("UserStatus",  validateuserInfo.StatusCode),
                    new Claim("businessName", validateuserInfo.MerchantBankInfo.Count == 0 ? string.Empty : validateuserInfo.MerchantBusinessInfo.Select(x => x.BusinessName).FirstOrDefault()),
                    new Claim(ClaimTypes.NameIdentifier,  Convert.ToString(validateuserInfo.ClientAuthenticationId)),

                   }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                cacheKey = Convert.ToString(validateuserInfo.ClientAuthenticationId);
                userInfo.Email = validateuserInfo.Email;
                userInfo.StatusCode = validateuserInfo.StatusCode;
                string serializedCustomerList = string.Empty;

                var redisCustomerList = await _distributedCache.GetAsync(cacheKey);

                if(redisCustomerList == null)
                {
                    await _distributedCache.RemoveAsync(cacheKey);
                    serializedCustomerList = JsonConvert.SerializeObject(userInfo);
                    redisCustomerList = Encoding.UTF8.GetBytes(serializedCustomerList);
                    var options = new DistributedCacheEntryOptions()
                    .SetAbsoluteExpiration(DateTime.Now.AddMinutes(20))
                    .SetSlidingExpiration(TimeSpan.FromMinutes(10));

                    await _distributedCache.SetAsync(cacheKey, redisCustomerList, options);
                }
                
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                tokenResult.AccessToken = tokenString;
                tokenResult.ClientId = validateuserInfo.Email;
                tokenResult.Role = validateuserInfo.RoleName;
                tokenResult.UserStatus = validateuserInfo.StatusCode;
                tokenResult.ResponseCode = AppResponseCodes.Success;
                tokenResult.BusinessName = validateuserInfo.MerchantBusinessInfo.Count == 0 ? string.Empty : validateuserInfo.MerchantBusinessInfo.Select(x => x.BusinessName).FirstOrDefault();
                tokenResult.FirstName = validateuserInfo.MerchantWallet.Count == 0 ? string.Empty : validateuserInfo.MerchantWallet.Select(x => x.Firstname).FirstOrDefault();
                tokenResult.LastName = validateuserInfo.MerchantWallet.Count == 0 ? string.Empty : validateuserInfo.MerchantWallet.Select(x => x.Lastname).FirstOrDefault();
                tokenResult.LastName = validateuserInfo.MerchantWallet.Count == 0 ? string.Empty : validateuserInfo.MerchantWallet.Select(x => x.Lastname).FirstOrDefault();
                tokenResult.BankName = validateuserInfo.MerchantBankInfo.Count == 0 ? string.Empty : validateuserInfo.MerchantBankInfo.Select(x => x.BankName).FirstOrDefault();
                tokenResult.Nuban = validateuserInfo.MerchantBankInfo.Count == 0 ? string.Empty : validateuserInfo.MerchantBankInfo.Select(x => x.Nuban).FirstOrDefault();
                tokenResult.AccountName = validateuserInfo.MerchantBankInfo.Count == 0 ? string.Empty : validateuserInfo.MerchantBankInfo.Select(x => x.AccountName).FirstOrDefault();
                tokenResult.PhoneNumber = validateuserInfo.PhoneNumber;
                tokenResult.MerchantWalletBalance = availableWalletBalance;
                _log4net.Info("Authenticate for login was successful" + " | " + loginRequestDto.Email + " | " + DateTime.Now);

                return tokenResult;
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "Authenticate" + " | " + loginRequestDto.Email + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new LoginAPIResponse { ResponseCode = AppResponseCodes.InternalError };
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

        public async Task<WebApiResponse> CreateAccount(string email, string password, string fullname, string phoneNumber)
        {
            _log4net.Info("CreateAccount" + " | " + email + " | " + phoneNumber + " | " +  DateTime.Now);

            try
            {
                byte[] passwordHash, passwordSalt;
                if(string.IsNullOrEmpty(password))
                {
                    var newPassword = Guid.NewGuid().ToString("N").Substring(0, 9) + DateTime.Now.Ticks;
                    password = newPassword;
                }
                _utilities.CreatePasswordHash(password.Encrypt(_appSettings.appKey), out passwordHash, out passwordSalt);
                                
                using(var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var model = new ClientAuthentication
                        {
                            ClientSecretHash = passwordHash,
                            ClientSecretSalt = passwordSalt,
                            Email = email,
                            StatusCode = MerchantOnboardingProcess.GuestAccount,
                            FullName = fullname,
                            IsDeleted = false,
                            PhoneNumber = phoneNumber,
                            RoleName = RoleDetails.CustomerAccount,
                            LastDateModified = DateTime.Now
                        };
                        await _context.ClientAuthentication.AddAsync(model);
                        await _context.SaveChangesAsync();

                        var logGuestAccess = new GuestAccountLog
                        {
                            ClientAuthenticationId = model.ClientAuthenticationId,
                            Status = false,
                            Email = model.Email
                        };

                        await _context.GuestAccountLog.AddAsync(logGuestAccess);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        _log4net.Info("CreateAccount was successful" + " | " + email + " | " + phoneNumber + " | " + DateTime.Now);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = model.ClientAuthenticationId };
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
                _log4net.Error("Error occured" + " | " + "CreateAccount" + " | " + email + " | " + phoneNumber + " | "+  ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> ModifyUserAccount(UpdateUserRequestDto updateUserRequestDto)
        {
            _log4net.Info("ModifyUserAccount request" + " | " + updateUserRequestDto.Email + " | " +  DateTime.Now);

            try
            {
                var validateUser = await _context.ClientAuthentication
                    .SingleOrDefaultAsync(x => x.Email == updateUserRequestDto.Email);

                if(validateUser == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.UserNotFound };

                validateUser.IsDeleted = updateUserRequestDto.Status;
                validateUser.LastDateModified = DateTime.Now;
                _context.Update(validateUser);
                await _context.SaveChangesAsync();
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "ModifyUserAccount" + " | " + updateUserRequestDto.Email + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> UnlockUserAccount(UpdateUserRequestDto updateUserRequestDto)
        {
            try
            {
                var validateUser = await _context.ClientAuthentication
                    .SingleOrDefaultAsync(x => x.Email == updateUserRequestDto.Email);

                if (validateUser == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.UserNotFound };

                var getAccountDetails = await _userRepoService.GetLoginAttemptAsync(validateUser.ClientAuthenticationId);

                getAccountDetails.IsSuccessful = true;
                getAccountDetails.LoginAttempt = 0;
                _context.ClientLoginStatus.Update(getAccountDetails);
                await _context.SaveChangesAsync();
                validateUser.IsLocked = false;
                validateUser.LastDateModified = DateTime.Now;
                _context.Update(validateUser);
                await _context.SaveChangesAsync();

                _log4net.Info($"{"Account was successfully unblocked. - "}{updateUserRequestDto.Email} {" - "}{DateTime.Now}");

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _log4net.Error($"{"Error occured while trying to unblock user. - "}{updateUserRequestDto.Email} {" - "}{DateTime.Now}");

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        ////    var cacheKey = "customerList";
        ////    string serializedCustomerList;
        ////    var customerList = new List<Customer>();
        ////    var redisCustomerList = await distributedCache.GetAsync(cacheKey);
        ////if (redisCustomerList != null)
        ////{
        ////    serializedCustomerList = Encoding.UTF8.GetString(redisCustomerList);
        ////    customerList = JsonConvert.DeserializeObject<List<Customer>>(serializedCustomerList);
        ////}
        ////else
        ////{
        ////    customerList = await context.Customers.ToListAsync();
        ////serializedCustomerList = JsonConvert.SerializeObject(customerList);
        ////    redisCustomerList = Encoding.UTF8.GetBytes(serializedCustomerList);
        ////    var options = new DistributedCacheEntryOptions()
        ////        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(10))
        ////        .SetSlidingExpiration(TimeSpan.FromMinutes(2));
        ////await distributedCache.SetAsync(cacheKey, redisCustomerList, options);
    }

}

