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
    public class SpectaSendOtp : ISpectaSendOtp
    {
        private readonly SocialPayDbContext _context;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaSendOtp));

        public SpectaSendOtp(SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService;
        }

        public async Task<WebApiResponse> SendOtp(SendOtpRequestDto model)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var checkregistered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == model.Email);
                        if (checkregistered.RegistrationStatus != SpectaProcessCodes.SendPhone)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Processing stage is not send OTP", StatusCode = ResponseCodes.InternalError };
                        var requestmodel = _mapper.Map<SendOtpRequest>(model);
                        await _context.SendOtpRequest.AddAsync(requestmodel);
                        var request = await _spectaOnboardingService.SendOtp(model);
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return request;
                        var response = (PaystackTokennizationResponseDto.PaystackTokennizationResponse)request.Data;
                        var sendotpresponse = new SendOtpResponse();
                        sendotpresponse.success = Convert.ToBoolean(response.success);
                        sendotpresponse.unAuthorizedRequest = response.unAuthorizedRequest;
                        sendotpresponse.__abp = response.__abp;

                        if (response.error != null)
                        {
                            sendotpresponse.code = response.error.code;
                            sendotpresponse.message = response.error.message;
                        }
                        if (response.result != null)
                        {
                            sendotpresponse.authUrl = response.result.authUrl;
                            sendotpresponse.bankName = response.result.bankName;
                            sendotpresponse.cardId = response.result.cardId;
                            sendotpresponse.cardStatus = response.result.cardStatus;
                            sendotpresponse.displayText = response.result.displayText;

                        }
                        await _context.SendOtpResponse.AddAsync(sendotpresponse);
                        if (checkregistered != null) { checkregistered.RegistrationStatus = SpectaProcessCodes.SendOtp; }
                        await _context.SaveChangesAsync();
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", Data = request.Data, StatusCode = ResponseCodes.InternalError };
                        await transaction.CommitAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = request.Data, StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _log4net.Error("Error occured" + " | " + "SendOtp" + " | " + ex + " | " + DateTime.Now);
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex, StatusCode = ResponseCodes.InternalError };

                    }
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "SendOtp" + " | " + ex + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex, StatusCode = ResponseCodes.InternalError };
            }
        }

    }
}
