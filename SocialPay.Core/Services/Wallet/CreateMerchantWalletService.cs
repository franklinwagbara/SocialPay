using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Wallet
{
	public class CreateMerchantWalletService
    {
		private readonly SocialPayDbContext _context;
		private readonly AppSettings _appSettings;
		private readonly WalletRepoService _walletRepoService;
		private readonly IDistributedCache _distributedCache;
		static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(CreateMerchantWalletService));

		public CreateMerchantWalletService(SocialPayDbContext context,
			 IOptions<AppSettings> appSettings, WalletRepoService walletRepoService,
			  IDistributedCache distributedCache)
		{
			_context = context;
			_appSettings = appSettings.Value;
			_walletRepoService = walletRepoService;
			_distributedCache = distributedCache;
		}

		public async Task<WebApiResponse> CreateWallet(long clientId)
        {
			try
			{
				//clientId = 27;
				_log4net.Info("Initiating CreateWallet request" + " | " + clientId + " | " + DateTime.Now);

				var getUserInfo = await _context.ClientAuthentication
				  .Include(x => x.MerchantWallet)
				  .SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);

				if(getUserInfo.MerchantWallet.Count == 0)
					return new WebApiResponse { ResponseCode = AppResponseCodes.MerchantBusinessInfoRequired };

				var walletModel = new MerchantWalletRequestDto
				{
					CURRENCYCODE = _appSettings.currencyCode,
					DOB = getUserInfo.MerchantWallet.Select(x => x.DoB).FirstOrDefault(),
					firstname = getUserInfo.MerchantWallet.Select(x => x.Firstname).FirstOrDefault(),
					lastname = getUserInfo.MerchantWallet.Select(x => x.Lastname).FirstOrDefault(),
					Gender = getUserInfo.MerchantWallet.Select(x => x.Gender).FirstOrDefault(),
					mobile = getUserInfo.PhoneNumber, AccountTier = "2"
				};
				var result = await _walletRepoService.CreateMerchantWallet(walletModel);
				using(var transaction = await _context.Database.BeginTransactionAsync())
				{
					try
					{
						if (result.response == AppResponseCodes.Success)
						{
							var getWalletInfo = await _context.MerchantWallet.SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);
							getWalletInfo.status = AppResponseCodes.Success;
							_context.Update(getWalletInfo);
							await _context.SaveChangesAsync();
							getUserInfo.StatusCode = AppResponseCodes.Success;
							_context.Update(getUserInfo);
							await _context.SaveChangesAsync();
							await transaction.CommitAsync();
							var cacheKey = Convert.ToString(clientId);
							string serializedCustomerList;
							var userInfo = new UserInfoViewModel { };
							var redisCustomerList = await _distributedCache.GetAsync(cacheKey);
							if (redisCustomerList != null)
							{
								await _distributedCache.RemoveAsync(cacheKey);
								userInfo.Email = getUserInfo.Email;
								userInfo.StatusCode = AppResponseCodes.Success;
								serializedCustomerList = JsonConvert.SerializeObject(userInfo);
								redisCustomerList = Encoding.UTF8.GetBytes(serializedCustomerList);
								var options1 = new DistributedCacheEntryOptions()
								.SetAbsoluteExpiration(DateTime.Now.AddMinutes(30))
								.SetSlidingExpiration(TimeSpan.FromMinutes(15));
								await _distributedCache.SetAsync(cacheKey, redisCustomerList, options1);
								_log4net.Info("CreateWallet response for" + " | " + clientId + " | " + DateTime.Now);
								return new WebApiResponse { ResponseCode = AppResponseCodes.Success, UserStatus = AppResponseCodes.Success };
							}
							await _distributedCache.RemoveAsync(cacheKey);
							userInfo.Email = getUserInfo.Email;
							userInfo.StatusCode = AppResponseCodes.Success;
							serializedCustomerList = JsonConvert.SerializeObject(userInfo);
							redisCustomerList = Encoding.UTF8.GetBytes(serializedCustomerList);
							var options = new DistributedCacheEntryOptions()
							.SetAbsoluteExpiration(DateTime.Now.AddMinutes(30))
							.SetSlidingExpiration(TimeSpan.FromMinutes(15));
							await _distributedCache.SetAsync(cacheKey, redisCustomerList, options);
							_log4net.Info("CreateWallet response for" + " | " + clientId + " | " + DateTime.Now);
							return new WebApiResponse { ResponseCode = AppResponseCodes.Success, UserStatus = AppResponseCodes.Success };
						}
						return new WebApiResponse { ResponseCode = AppResponseCodes.Failed };
					}
					catch (Exception ex)
					{
						_log4net.Error("Error occured" + " | " + "CreateWallet" + " | "+ clientId + " | " + ex.Message.ToString() + " | " + DateTime.Now);
						await transaction.RollbackAsync();
						return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
					}
				}
			
			}
			catch (Exception ex)
			{
				_log4net.Error("Error occured" + " | " + "CreateWallet" + " | " + clientId + " | " + ex.Message.ToString() + " | " + DateTime.Now);

				return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
			}
        }


		public async Task<WebApiResponse> ClearMerchantWalletInfo(string phoneNumber)
		{
			try
			{
				
				var result = await _walletRepoService.ClearMerchantWallet(phoneNumber);
				if (result.response == AppResponseCodes.Success)
				{				
					return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
				}
				return new WebApiResponse { ResponseCode = AppResponseCodes.Failed };
			}
			catch (Exception ex)
			{

				return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
			}
		}

	}
}
