using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialPay.Core.Services.SpectaOnboardingService.Interface;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaSendBvnPhoneVerificationCode : ISpectaSendBvnPhoneVerificationCode
    {
        private readonly SocialPayDbContext _context;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaSendBvnPhoneVerificationCode));

        public SpectaSendBvnPhoneVerificationCode(SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService;
        }

        public async Task<WebApiResponse> SendBvnPhoneVerificationCode(SendBvnPhoneVerificationCodeRequestDto model)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var checkregistered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == model.emailAddress);
                        if (!checkregistered.RegistrationStatus.Equals(SpectaProcessCodes.VerifyEmailConfirmationCode))
                        {
                            checkregistered.RegistrationStatus = SpectaProcessCodes.VerifyEmailConfirmationCode;
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }

                        if (checkregistered.RegistrationStatus != SpectaProcessCodes.VerifyEmailConfirmationCode)
                            return new WebApiResponse { ResponseCode = checkregistered.RegistrationStatus, Message = "Processing stage is not Send Bvn Phone Verification Code", StatusCode = ResponseCodes.InternalError };
                       
                        var request = await _spectaOnboardingService.SendBvnPhoneVerificationCode(model.emailAddress);
                       
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return request;

                        var response = (SpectaResponseWithObjectResultMessage.SpectaResponseDto)request.Data;

                        var sendBvnPhoneVerificationresponse = new SendBvnPhoneVerificationCodeResponse();

                        sendBvnPhoneVerificationresponse.success = Convert.ToBoolean(response?.success);
                        sendBvnPhoneVerificationresponse.unAuthorizedRequest = response.unAuthorizedRequest;
                        sendBvnPhoneVerificationresponse.__abp = Convert.ToBoolean(response?.__abp);
                      
                        if (response.error != null)
                        {
                            sendBvnPhoneVerificationresponse.code = response.error.code;
                            sendBvnPhoneVerificationresponse.details = response.error.details;
                            sendBvnPhoneVerificationresponse.message = response.error.message;
                            sendBvnPhoneVerificationresponse.validationErrors = response.error.validationErrors;
                        }
                        await _context.SendBvnPhoneVerificationCodeResponse.AddAsync(sendBvnPhoneVerificationresponse);
                       
                        if (checkregistered != null) { checkregistered.RegistrationStatus = SpectaProcessCodes.SendBvnPhoneVerificationCode; }
                       
                        await _context.SaveChangesAsync();
                       
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", Data = request.Data, StatusCode = ResponseCodes.InternalError };
                       
                        await transaction.CommitAsync();
                       
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = request.Data, StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _log4net.Error("Error occured" + " | " + "SendBvnPhoneVerificationCode" + " | " + ex + " | " + DateTime.Now);
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex };
                    }
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "SendBvnPhoneVerificationCode" + " | " + ex + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex };
            }
        }

    }

}
