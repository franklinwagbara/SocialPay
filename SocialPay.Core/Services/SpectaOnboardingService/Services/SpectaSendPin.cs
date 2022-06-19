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
    public class SpectaSendPin : ISpectaSendPin
    {

        private readonly SocialPayDbContext _context;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        private readonly SpectaOnboardingLogger _spectaOnboardingLogger;

        public SpectaSendPin(SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper, SpectaOnboardingLogger spectaOnboardingLogger)
        {
            _context = context;
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService;
            _spectaOnboardingLogger = spectaOnboardingLogger;
        }
        public async Task<WebApiResponse> SendPin(SendPinRequestDto model)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var checkregistered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == model.Email);
                        if (checkregistered.RegistrationStatus != SpectaProcessCodes.SendOtp)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Processing stage is not send PIN", StatusCode = ResponseCodes.InternalError };
                        var requestmodel = _mapper.Map<SendPinRequest>(model);
                        await _context.SendPinRequest.AddAsync(requestmodel);
                        var request = await _spectaOnboardingService.SendPin(model);
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return request;
                        var response = (PaystackTokennizationResponseDto.PaystackTokennizationResponse)request.Data;
                        var sendpinresponse = new SendPinResponse();
                        sendpinresponse.success = Convert.ToBoolean(response.success);
                        sendpinresponse.unAuthorizedRequest = response.unAuthorizedRequest;
                        sendpinresponse.__abp = response.__abp;

                        if (response.error != null)
                        {
                            sendpinresponse.code = response.error.code;
                            sendpinresponse.message = response.error.message;
                        }
                        if (response.result != null)
                        {
                            sendpinresponse.authUrl = response.result.authUrl;
                            sendpinresponse.bankName = response.result.bankName;
                            sendpinresponse.cardId = response.result.cardId;
                            sendpinresponse.cardStatus = response.result.cardStatus;
                            sendpinresponse.displayText = response.result.displayText;

                        }
                        await _context.SendPinResponse.AddAsync(sendpinresponse);
                        if (checkregistered != null) { checkregistered.RegistrationStatus = SpectaProcessCodes.SendPin; }
                        await _context.SaveChangesAsync();
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", Data = request.Data, StatusCode = ResponseCodes.InternalError };
                        await transaction.CommitAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = request.Data, StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _spectaOnboardingLogger.LogRequest($"{"Error occured -- SendPin" + ex.ToString()}{"-"}{DateTime.Now}", true);
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex, StatusCode = ResponseCodes.InternalError };
                    }
                }
            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- SendPin" + ex.ToString()}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex, StatusCode = ResponseCodes.InternalError };
            }
        }

    }
}
