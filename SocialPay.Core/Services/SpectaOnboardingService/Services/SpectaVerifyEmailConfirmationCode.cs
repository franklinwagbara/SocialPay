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
    public class SpectaVerifyEmailConfirmationCode : ISpectaVerifyEmailConfirmationCode
    {

        private readonly SocialPayDbContext _context;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaVerifyEmailConfirmationCode));

        public SpectaVerifyEmailConfirmationCode(SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService;
        }


        public async Task<WebApiResponse> VerifyEmailConfirmationCode(VerifyEmailConfirmationCodeRequestDto model)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var checkregistered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == model.email);

                        if (checkregistered.RegistrationStatus != SpectaProcessCodes.SendEmailVerificationCode)
                            return new WebApiResponse { ResponseCode = checkregistered.RegistrationStatus, Message = "Processing stage is not Send Email Confirmation Code", StatusCode = ResponseCodes.Duplicate };
                       
                        var requestmodel = _mapper.Map<VerifyEmailConfirmationCodeRequest>(model);
                        await _context.VerifyEmailConfirmationCodeRequest.AddAsync(requestmodel);

                        var request = await _spectaOnboardingService.VerifyEmailConfirmationCode(model);
                        
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return request;
                        
                        var response = (SpectaResponseWithObjectResultMessage.SpectaResponseDto)request.Data;
                        var verifyemailconfirmationcoderesponse = new VerifyEmailConfirmationCodeResponse();

                        verifyemailconfirmationcoderesponse.success = Convert.ToBoolean(response?.success);
                        verifyemailconfirmationcoderesponse.unAuthorizedRequest = response.unAuthorizedRequest;
                        verifyemailconfirmationcoderesponse.__abp = Convert.ToBoolean(response?.__abp);
                      
                        if (response.error != null)
                        {
                            verifyemailconfirmationcoderesponse.code = response.error.code;
                            verifyemailconfirmationcoderesponse.details = response.error.details;
                            verifyemailconfirmationcoderesponse.message = response.error.message;
                            verifyemailconfirmationcoderesponse.validationErrors = response.error.validationErrors;
                        }
                        
                        if (checkregistered != null) { checkregistered.RegistrationStatus = SpectaProcessCodes.VerifyEmailConfirmationCode; }
                        await _context.VerifyEmailConfirmationCodeResponse.AddAsync(verifyemailconfirmationcoderesponse);
                        await _context.SaveChangesAsync();
                      
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", Data = request.Data, StatusCode = ResponseCodes.InternalError };
                        
                        await transaction.CommitAsync();
                        
                        return new WebApiResponse { ResponseCode = SpectaProcessCodes.VerifyEmailConfirmationCode, Message = "Success", Data = request.Data, StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _log4net.Error("Error occured" + " | " + "Verify Email Confirmation Code" + " | " + ex + " | " + DateTime.Now);
                        return new WebApiResponse { ResponseCode = SpectaProcessCodes.Failed, Message = "Request failed " };
                    }
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "Verify Email Confirmation Code" + " | " + ex + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Request failed "};
            }
        }

    }

}
