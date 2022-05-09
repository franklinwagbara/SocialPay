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

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaSetDisbursementAccountService : ISpectaSetDisbursementAccount
    {
        private readonly SocialPayDbContext _context;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        private readonly SpectaOnboardingLogger _spectaOnboardingLogger;

        public SpectaSetDisbursementAccountService(SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper, SpectaOnboardingLogger spectaOnboardingLogger)
        {
            _context = context;
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService;
            _spectaOnboardingLogger = spectaOnboardingLogger;
        }
        public async Task<WebApiResponse> SetDisbursementAccount(SetDisbursementAccountRequestDto model)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var checkregistered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == model.Email);

                        if (checkregistered.RegistrationStatus != SpectaProcessCodes.ConfirmTicket)
                            return new WebApiResponse { ResponseCode = checkregistered.RegistrationStatus, Message = "Processing stage is not Set Disbursement Account" };
                        
                        var requestmodel = _mapper.Map<SetDisbursementAccountRequest>(model);
                        await _context.SetDisbursementAccountRequest.AddAsync(requestmodel);
                       
                        var request = await _spectaOnboardingService.DisbursementAccount(model);
                        
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return request;
                        
                        var response = (SpectaResponseWithObjectResultMessage.SpectaResponseDto)request.Data;
                        
                        var disbursementaccountresponse = new SetDisbursementAccountResponse();
                        disbursementaccountresponse.success = response.success;
                        disbursementaccountresponse.unAuthorizedRequest = response.unAuthorizedRequest;
                        disbursementaccountresponse.__abp = response.__abp;
                        
                        if (response.error != null)
                        {
                            disbursementaccountresponse.details = response.error.details;
                            disbursementaccountresponse.message = response.error.message;
                            disbursementaccountresponse.validationErrors = response.error.validationErrors;
                        }
                        await _context.SetDisbursementAccountResponse.AddAsync(disbursementaccountresponse);
                       
                        if (checkregistered != null) { checkregistered.RegistrationStatus = SpectaProcessCodes.SetDisbursementAccount; }
                       
                        await _context.SaveChangesAsync();
                       
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", Data = request.Data, StatusCode = ResponseCodes.Success };
                       
                        await transaction.CommitAsync();
                        
                        return new WebApiResponse { ResponseCode = SpectaProcessCodes.SetDisbursementAccount, Message = "Success", Data = request.Data, StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _spectaOnboardingLogger.LogRequest($"{"Error occured -- SetDisbursementAccount" + ex.ToString()}{"-"}{DateTime.Now}", true);
                        return new WebApiResponse { ResponseCode = SpectaProcessCodes.Failed, Message = "Request failed " + ex, StatusCode = ResponseCodes.InternalError };
                    }
                }
            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- SetDisbursementAccount" + ex.ToString()}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = SpectaProcessCodes.Failed, Message = "Request failed " + ex, StatusCode = ResponseCodes.InternalError };
            }
        }
    }
}
