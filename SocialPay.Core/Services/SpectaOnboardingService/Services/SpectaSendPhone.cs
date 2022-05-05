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
    public class SpectaSendPhone : ISpectaSendPhone
    {
        private readonly SocialPayDbContext _context;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaSendPhone));

        public SpectaSendPhone(SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService;
        }

        public async Task<WebApiResponse> SendPhone(SendPhoneRequestDto model)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var checkregistered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == model.Email);
                        if (checkregistered.RegistrationStatus != SpectaProcessCodes.ChargeCard)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Processing stage is not send phone", StatusCode = ResponseCodes.InternalError };
                        var requestmodel = _mapper.Map<SendPhoneRequest>(model);
                        await _context.SendPhoneRequest.AddAsync(requestmodel);
                        var request = await _spectaOnboardingService.SendPhone(model);
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return request;
                        var response = (PaystackTokennizationResponseDto.PaystackTokennizationResponse)request.Data;
                        var sendphoneresponse = new SendPhoneResponse();
                        sendphoneresponse.success = Convert.ToBoolean(response.success);
                        sendphoneresponse.unAuthorizedRequest = response.unAuthorizedRequest;
                        sendphoneresponse.__abp = response.__abp;

                        if (response.error != null)
                        {
                            sendphoneresponse.code = response.error.code;
                            sendphoneresponse.message = response.error.message;
                        }
                        if (response.result != null)
                        {
                            sendphoneresponse.authUrl = response.result.authUrl;
                            sendphoneresponse.bankName = response.result.bankName;
                            sendphoneresponse.cardId = response.result.cardId;
                            sendphoneresponse.cardStatus = response.result.cardStatus;
                            sendphoneresponse.displayText = response.result.displayText;

                        }
                        await _context.SendPhoneResponse.AddAsync(sendphoneresponse);
                        if (checkregistered != null) { checkregistered.RegistrationStatus = SpectaProcessCodes.SendPhone; }
                        await _context.SaveChangesAsync();
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", Data = request.Data, StatusCode = ResponseCodes.InternalError };
                        await transaction.CommitAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = request.Data, StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _log4net.Error("Error occured" + " | " + "SendPhone" + " | " + ex + " | " + DateTime.Now);
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex, StatusCode = ResponseCodes.InternalError };

                    }
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "SendPhone" + " | " + ex + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex, StatusCode = ResponseCodes.InternalError };

            }
        }

    }
}
