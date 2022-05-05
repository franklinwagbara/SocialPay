using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialPay.Core.Services.ISpectaOnboardingService;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services
{
    public class SpectaValidateCharge : ISpectaValidateCharge
    {
        private readonly SocialPayDbContext _context;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaValidateCharge));

        public SpectaValidateCharge(SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService;
        }

        public async Task<WebApiResponse> ValidateCharge(ValidateChargeRequestDto model)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var checkregistered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == model.Email);
                        if (checkregistered.RegistrationStatus != SpectaProcessCodes.SendPhone && checkregistered.RegistrationStatus != SpectaProcessCodes.SendOtp && checkregistered.RegistrationStatus != SpectaProcessCodes.SendPin)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Processing stage is not Validate Charge", StatusCode = ResponseCodes.InternalError };
                        var requestmodel = _mapper.Map<ValidateChargeRequest>(model);
                        await _context.ValidateChargeRequest.AddAsync(requestmodel);
                        var request = await _spectaOnboardingService.ValidateCharge(model);
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return request;
                        var response = (PaystackTokennizationResponseDto.PaystackTokennizationResponse)request.Data;
                        var validatechargeresponse = new ValidateChargeResponse();
                        validatechargeresponse.success = Convert.ToBoolean(response.success);
                        validatechargeresponse.unAuthorizedRequest = response.unAuthorizedRequest;
                        validatechargeresponse.__abp = response.__abp;

                        if (response.error != null)
                        {
                            validatechargeresponse.code = response.error.code;
                            validatechargeresponse.message = response.error.message;
                        }
                        if (response.result != null)
                        {
                            validatechargeresponse.authUrl = response.result.authUrl;
                            validatechargeresponse.bankName = response.result.bankName;
                            validatechargeresponse.cardId = response.result.cardId;
                            validatechargeresponse.cardStatus = response.result.cardStatus;
                            validatechargeresponse.displayText = response.result.displayText;

                        }
                        await _context.ValidateChargeResponse.AddAsync(validatechargeresponse);
                        if (checkregistered != null) { checkregistered.RegistrationStatus = AppResponseCodes.Success; }
                        await _context.SaveChangesAsync();
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", Data = request.Data, StatusCode = ResponseCodes.InternalError };
                        await transaction.CommitAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = request.Data, StatusCode = ResponseCodes.InternalError };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _log4net.Error("Error occured" + " | " + "ValidateCharge" + " | " + ex + " | " + DateTime.Now);
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex, StatusCode = ResponseCodes.InternalError };
                    }
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "ValidateCharge" + " | " + ex + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex, StatusCode = ResponseCodes.InternalError };
            }
        }

    }
}
