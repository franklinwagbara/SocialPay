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
    public class SpectaVerifyBvnPhoneConfirmationCode : ISpectaVerifyBvnPhoneConfirmationCode
    {
        private readonly SocialPayDbContext _context;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaVerifyBvnPhoneConfirmationCode));

        public SpectaVerifyBvnPhoneConfirmationCode(SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService;
        }

        public async Task<WebApiResponse> VerifyBvnPhoneConfirmationCode(VerifyBvnPhoneConfirmationCodeRequestDto model)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var checkregistered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == model.email);

                        if (checkregistered.RegistrationStatus != SpectaProcessCodes.SendBvnPhoneVerificationCode)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Your specta registration is incomplete. Kindly complete the process", StatusCode = ResponseCodes.InternalError };
                       
                        var requestmodel = _mapper.Map<VerifyBvnPhoneConfirmationCodeRequest>(model);
                       
                        await _context.VerifyBvnPhoneConfirmationCodeRequest.AddAsync(requestmodel);

                        var request = await _spectaOnboardingService.VerifyBvnPhoneConfirmationCode(model);
                       
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return request;
                       
                        var response = (SpectaResponseWithObjectResultMessage.SpectaResponseDto)request.Data;
                      
                        var verifybvnphoneconfirmationcoderesponse = new VerifyBvnPhoneConfirmationCodeResponse();
                        verifybvnphoneconfirmationcoderesponse.success = Convert.ToBoolean(response?.success);
                        verifybvnphoneconfirmationcoderesponse.unAuthorizedRequest = response.unAuthorizedRequest;
                        verifybvnphoneconfirmationcoderesponse.__abp = Convert.ToBoolean(response?.__abp);

                        if (response.error != null)
                        {
                            verifybvnphoneconfirmationcoderesponse.code = response.error.code;
                            verifybvnphoneconfirmationcoderesponse.details = response.error.details;
                            verifybvnphoneconfirmationcoderesponse.message = response.error.message;
                            verifybvnphoneconfirmationcoderesponse.validationErrors = response.error.validationErrors;
                        }

                        await _context.VerifyBvnPhoneConfirmationCodeResponse.AddAsync(verifybvnphoneconfirmationcoderesponse);
                       
                        if (checkregistered != null) { checkregistered.RegistrationStatus = SpectaProcessCodes.VerifyBvnPhoneConfirmationCode; }
                       
                        await _context.SaveChangesAsync();
                        
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", Data = request.Data, StatusCode = ResponseCodes.InternalError };
                        
                        await transaction.CommitAsync();
                        
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = request.Data, StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _log4net.Error("Error occured" + " | " + "Verify Bvn Phone Confirmation Code" + " | " + ex + " | " + DateTime.Now);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex };
                    }
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "Verify Bvn Phone Confirmation Code" + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex };
            }
        }

    }

}
