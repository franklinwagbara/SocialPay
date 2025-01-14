﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Loan
{
    public class LoanRepaymentService
    {

        private readonly SocialPayDbContext _context;
        private readonly AppSettings _appSettings;
        public LoanRepaymentService(SocialPayDbContext context, IOptions<AppSettings> appSettings)

        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _appSettings = appSettings.Value;
        }

        public async Task<WebApiResponse> GetRepaymentModel()
        {

            try
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = await _context.LoanRepaymentPlan.Where(x => x.IsDeleted == false).ToListAsync(), StatusCode = ResponseCodes.Success };

            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "An error occured. Please try again.", Message = "An error occured. Please try again.", StatusCode = ResponseCodes.InternalError };
            }

        }

        public async Task<WebApiResponse> CreateRepaymentModel(LoanRepaymentRequestDto model, long clientId, string email)
        {
            try
            {

                var payload = new LoanRepaymentPlan
                {
                    IsDeleted = false,
                    DailySalesPercentage = model.DailySalesPercentage,
                    Rate = model.Rate,
                    PA = model.PA
                };

                await _context.LoanRepaymentPlan.AddAsync(payload);
                await _context.SaveChangesAsync();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Repayment added successfully", Data = "Repayment added successfully", StatusCode = ResponseCodes.Success };

            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "An error occured. Please try again.", Message = "An error occured. Please try again.", StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> DeleteRepaymentModel(DeleteLoanRepaymentRequestDto model, long clientId, string email)
        {
            try
            {

                var GetLoanRepaymentDetails = await _context.LoanRepaymentPlan.SingleOrDefaultAsync(x => x.LoanRepaymentPlanId == model.LoanRepaymentPlanId);
                GetLoanRepaymentDetails.IsDeleted = true;
                _context.LoanRepaymentPlan.Update(GetLoanRepaymentDetails);

                await _context.SaveChangesAsync();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Loan repayment plan deleted successfully", Message = "Loan repayment plan deleted successfully", StatusCode = ResponseCodes.Success };
            }
            catch (Exception e)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "An error occured. Please try again.", Message = "An error occured. Please try again.", StatusCode = ResponseCodes.InternalError };
            }
        }

    }

}
