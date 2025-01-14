﻿using SocialPay.Core.Services.Merchant.Interfaces;
using SocialPay.Domain;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SocialPay.Helper.SerilogService.Merchant;

namespace SocialPay.Core.Services.Merchant.Services
{
    public class MerchantsWithOutPaymentLinkService : IMerchantsWithOutPaymentLink
    {
        private readonly SocialPayDbContext _context;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(MerchantsWithOutPaymentLinkService));
        private readonly MerchantsLogger _merchantLogger;
        public MerchantsWithOutPaymentLinkService(SocialPayDbContext context, MerchantsLogger merchantLogger)
        {
            _context = context;
            _merchantLogger = merchantLogger;
        }
        public async Task<WebApiResponse> MerchantsWithOutPaymentLink()
        {
            try
            {

                var query = await (from c in _context.ClientAuthentication
                                   where !(from m in _context.MerchantPaymentSetup select m.ClientAuthenticationId).Contains(c.ClientAuthenticationId)
                                   select new MerchantsWithOutPaymentLinkResponseDto()
                                   {
                                       Email = c.Email,
                                       FullName = c.FullName,
                                       PhoneNumber = c.PhoneNumber,
                                       Bvn = c.Bvn,
                                       ReferCode = c.ReferCode,
                                       ReferralCode = c.ReferralCode,
                                       RegisteredDate = c.DateEntered,
                                       LastDateModified = c.LastDateModified,
                                   }).OrderByDescending(x=> x.RegisteredDate).ToListAsync();

                if (query.Count == 0)
                {
                    _merchantLogger.LogRequest($"{"No Record Found"}{" | "}{"Merchants WithOut Payment Link"}");
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "No Recond Found", Data = query, StatusCode = ResponseCodes.RecordNotFound };
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = query, StatusCode = ResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _merchantLogger.LogRequest($"{"Error occured "}{ex}{" | "}{ "Merchants WithOut Payment Link"}");
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Data = null, Message = "Internal error occured", StatusCode = ResponseCodes.InternalError };

            }
        }
    }
}
