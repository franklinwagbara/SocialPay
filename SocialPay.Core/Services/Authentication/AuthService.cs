using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
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
        private readonly IDistributedCache _distributedCache;
        public AuthRepoService(SocialPayDbContext context, IOptions<AppSettings> appSettings,
            Utilities utilities, ADRepoService aDRepoService, IDistributedCache distributedCache,
            WalletRepoService walletRepoService) : base(context)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _utilities = utilities;
            _aDRepoService = aDRepoService;
            _distributedCache = distributedCache;
            _walletRepoService = walletRepoService;
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
                    .SingleOrDefaultAsync(x => x.Email == loginRequestDto.Email && x.IsDeleted == false);

                // check if username exists
                if (validateuserInfo == null)
                    return new LoginAPIResponse { ResponseCode = AppResponseCodes.InvalidLogin };
                var tokenResult = new LoginAPIResponse();
                var key = Encoding.ASCII.GetBytes(_appSettings.SecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor();
                var tokenHandler = new JwtSecurityTokenHandler();
                if (validateuserInfo.RoleName == "Super Administrator")
                {
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
                    return new LoginAPIResponse { ResponseCode = AppResponseCodes.InvalidLogin };

               
                if(validateuserInfo.RoleName == "Guest")
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
                return tokenResult;
            }
            catch (Exception ex)
            {
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
            try
            {
                byte[] passwordHash, passwordSalt;
                if(string.IsNullOrEmpty(password))
                {
                    var newPassword = Guid.NewGuid().ToString("N").Substring(0, 9) + DateTime.Now.Ticks;
                    password = newPassword;
                }
                _utilities.CreatePasswordHash(password.Encrypt(_appSettings.appKey), out passwordHash, out passwordSalt);
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
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = model.ClientAuthenticationId };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> ModifyUserAccount(UpdateUserRequestDto updateUserRequestDto)
        {
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

