using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialPay.Core.Services.ISpectaOnboardingService;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.SpectaOnboarding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services
{
    public class SpectaChargeCard : ISpectaChargeCard
    {
        private readonly SocialPayDbContext _context;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        private readonly SpectaOnboardingLogger _spectaOnboardingLogger;

        public SpectaChargeCard(SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper, SpectaOnboardingLogger spectaOnboardingLogger)
        {
            _context = context;
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService;
            _spectaOnboardingLogger = spectaOnboardingLogger;
        }

        public async Task<WebApiResponse> ChargeCard(ChargeCardRequestDto model)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var checkregistered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == model.Email);
                        if (checkregistered.RegistrationStatus != SpectaProcessCodes.SetDisbursementAccount)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Processing stage is not Charge Card", StatusCode = ResponseCodes.InternalError };
                        var requestmodel = _mapper.Map<ChargeCardRequest>(model);
                        await _context.ChargeCardRequest.AddAsync(requestmodel);
                        var request = await _spectaOnboardingService.ChargeCard(model);
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return request;
                        var response = (PaystackTokennizationResponseDto.PaystackTokennizationResponse)request.Data;
                        var chargecardresponse = new ChargeCardResponse();
                        chargecardresponse.success = Convert.ToBoolean(response.success);
                        chargecardresponse.unAuthorizedRequest = response.unAuthorizedRequest;
                        chargecardresponse.__abp = response.__abp;
                        if (response.error != null)
                        {
                            chargecardresponse.code = response.error.code;
                            chargecardresponse.message = response.error.message;
                        }
                        if (response.result != null)
                        {
                            chargecardresponse.authUrl = response.result.authUrl;
                            chargecardresponse.bankName = response.result.bankName;
                            chargecardresponse.cardId = response.result.cardId;
                            chargecardresponse.cardStatus = response.result.cardStatus;
                            chargecardresponse.displayText = response.result.displayText;

                        }
                        await _context.ChargeCardResponse.AddAsync(chargecardresponse);
                        if (checkregistered != null) { checkregistered.RegistrationStatus = SpectaProcessCodes.ChargeCard; }
                        await _context.SaveChangesAsync();
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", Data = request.Data, StatusCode = ResponseCodes.InternalError };
                        await transaction.CommitAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = request.Data, StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _spectaOnboardingLogger.LogRequest($"{"Error occured -- ChargeCard "+ex.ToString()}{"-"}{DateTime.Now}", true);
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex, StatusCode = ResponseCodes.InternalError };
                    }
                }
            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- ChargeCard " + ex.ToString()}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex.Message, StatusCode = ResponseCodes.InternalError };
            }
        }


    }
}
