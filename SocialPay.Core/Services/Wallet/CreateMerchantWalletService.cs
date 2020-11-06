﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
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
		public CreateMerchantWalletService(SocialPayDbContext context,
			 IOptions<AppSettings> appSettings, WalletRepoService walletRepoService)
		{
			_context = context;
			_appSettings = appSettings.Value;
			_walletRepoService = walletRepoService;
		}
		public async Task<WebApiResponse> CreateWallet(long clientId)
        {
			try
			{
				var getUserInfo = await _context.ClientAuthentication
				  .Include(x => x.MerchantWallet)
				  .SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);

				if(getUserInfo.MerchantWallet.Count > 0)
					return new WebApiResponse { ResponseCode = AppResponseCodes.WalletExist };

				var walletModel = new MerchantWalletRequestDto
				{
					CURRENCYCODE = _appSettings.currencyCode,
					DOB = getUserInfo.MerchantWallet.Select(x => x.DoB).FirstOrDefault(),
					firstname = getUserInfo.MerchantWallet.Select(x => x.Firstname).FirstOrDefault(),
					lastname = getUserInfo.MerchantWallet.Select(x => x.Lastname).FirstOrDefault(),
					Gender = getUserInfo.MerchantWallet.Select(x => x.Gender).FirstOrDefault(),
					mobile = getUserInfo.PhoneNumber,
				};
				var result = await _walletRepoService.CreateMerchantWallet(walletModel);
				if(result.response == AppResponseCodes.Success)
				{
					getUserInfo.StatusCode = AppResponseCodes.Success;
					_context.Update(getUserInfo);
					await _context.SaveChangesAsync();
					return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
				}
				return new WebApiResponse { ResponseCode = AppResponseCodes.Failed };
			}
			catch (Exception ex)
			{

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
