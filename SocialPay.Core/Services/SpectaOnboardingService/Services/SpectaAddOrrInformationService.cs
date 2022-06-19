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
    public class SpectaAddOrrInformationService : ISpectaAddOrrInformation
    {
        private readonly SocialPayDbContext _context;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        private readonly SpectaOnboardingLogger _spectaOnboardingLogger;

        public SpectaAddOrrInformationService(SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper, SpectaOnboardingLogger spectaOnboardingLogger)
        {
            _context = context;
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService;
            _spectaOnboardingLogger = spectaOnboardingLogger;
        }

        public async Task<WebApiResponse> AddOrrInformation(AddOrrInformationRequestDto model)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var checkregistered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == model.Email);

                        if (checkregistered.RegistrationStatus != SpectaProcessCodes.VerifyBvnPhoneConfirmationCode)
                            return new WebApiResponse { ResponseCode = checkregistered.RegistrationStatus, Message = "Processing stage is not Add-Orr-Information", StatusCode = ResponseCodes.Badrequest};
                      
                        var requestmodel = _mapper.Map<AddOrrInformationRequest>(model);
                        await _context.AddOrrInformationRequest.AddAsync(requestmodel);
                       
                        var request = await _spectaOnboardingService.AddOrrInformation(model);

                        if (request.ResponseCode != AppResponseCodes.Success)
                            return request;

                        var response = (SpectaResponseWithObjectResultMessage.SpectaResponseDto)request.Data;

                        var addorrinforesponse = new AddOrrInformationResponse();
                        addorrinforesponse.success = response.success;
                        addorrinforesponse.unAuthorizedRequest = response.unAuthorizedRequest;
                        addorrinforesponse.__abp = response.__abp;

                        if (response.error != null)
                        {
                            addorrinforesponse.code = response.error.code;
                            addorrinforesponse.details = response.error.details;
                            addorrinforesponse.message = response.error.message;
                            addorrinforesponse.validationErrors = response.error.validationErrors;
                        }
                        await _context.AddOrrInformationResponse.AddAsync(addorrinforesponse);
                       
                        if (checkregistered != null) { checkregistered.RegistrationStatus = SpectaProcessCodes.AddOrrInformation; }
                       
                        await _context.SaveChangesAsync();
                       
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", Data = request.Data, StatusCode = ResponseCodes.Badrequest };
                        
                        await transaction.CommitAsync();

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = request.Data, StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _spectaOnboardingLogger.LogRequest($"{"Error occured -- AddOrrInformation "+ ex.ToString()}{"-"}{DateTime.Now}", true);
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed ", Data = "Request failed", StatusCode = ResponseCodes.InternalError };

                    }
                }
            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- AddOrrInformation " + ex.ToString()}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed ", Data = "Request failed", StatusCode = ResponseCodes.InternalError };

            }
        }

    }
}
