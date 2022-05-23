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
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaSendEmailVerificationCode : ISpectaSendEmailVerificationCode
    {

        private readonly SocialPayDbContext _context;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        private readonly SpectaOnboardingLogger _spectaOnboardingLogger;

        public SpectaSendEmailVerificationCode(SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper, SpectaOnboardingLogger spectaOnboardingLogger)
        {
            _context = context;
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService;
            _spectaOnboardingLogger = spectaOnboardingLogger;
        }

        public async Task<WebApiResponse> SendEmailVerificationCode(SendEmailVerificationCodeRequestDto model)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var checkregistered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == model.email);

                        if (!checkregistered.RegistrationStatus.Equals(SpectaProcessCodes.RegisterCustomer))
                        {
                            checkregistered.RegistrationStatus = SpectaProcessCodes.RegisterCustomer;
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }

                        if (checkregistered.RegistrationStatus != SpectaProcessCodes.RegisterCustomer)
                            return new WebApiResponse { ResponseCode = checkregistered.RegistrationStatus, Message = "Processing stage is not Send Email Verification Code", StatusCode = ResponseCodes.InternalError };
                       
                        var requestmodel = _mapper.Map<SendEmailVerificationCodeRequest>(model);
                        await _context.SendEmailVerificationCodeRequest.AddAsync(requestmodel);

                        var request = await _spectaOnboardingService.SendEmailVerificationCode(model);                       
                        
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return request;

                        var response = (SpectaResponseWithBoolResultMessage.SpectaResponseDto)request.Data;

                        var sendemailverificationcoderesponse = new SendEmailVerificationCodeResponse();
                        sendemailverificationcoderesponse.result = Convert.ToBoolean(response?.result);
                        sendemailverificationcoderesponse.success = Convert.ToBoolean(response?.success);
                        sendemailverificationcoderesponse.unAuthorizedRequest = response.unAuthorizedRequest;
                        sendemailverificationcoderesponse.__abp = Convert.ToBoolean(response?.__abp);

                        if (response.error != null)
                        {
                            sendemailverificationcoderesponse.code = response.error.code;
                            sendemailverificationcoderesponse.details = response.error.details;
                            sendemailverificationcoderesponse.message = response.error.message;
                            sendemailverificationcoderesponse.validationErrors = response.error.validationErrors;
                        }

                        await _context.SendEmailVerificationCodeResponse.AddAsync(sendemailverificationcoderesponse);

                        if (checkregistered != null)
                            checkregistered.RegistrationStatus = SpectaProcessCodes.SendEmailVerificationCode;

                        await _context.SaveChangesAsync();

                        if (request.ResponseCode != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", Data = request.Data, StatusCode = ResponseCodes.InternalError };
                        
                        await transaction.CommitAsync();
                     
                       return new WebApiResponse { ResponseCode = SpectaProcessCodes.success, Message = "Success", Data = request.Data, StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _spectaOnboardingLogger.LogRequest($"{"Error occured -- SendEmailVerificationCode" + ex.ToString()}{"-"}{DateTime.Now}", true);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", StatusCode = ResponseCodes.InternalError };

                    }
                }
            }
            catch (Exception ex)
            {

                _spectaOnboardingLogger.LogRequest($"{"Error occured -- SendEmailVerificationCode" + ex.ToString()}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex.Message };

            }
        }

    }

}
