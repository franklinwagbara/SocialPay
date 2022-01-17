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
    public class SpectaAddOrrInformationService : ISpectaAddOrrInformation
    {
        private readonly SocialPayDbContext _context;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaAddOrrInformationService));

        public SpectaAddOrrInformationService(SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService;
        }

        public async Task<WebApiResponse> AddOrrInformation(AddOrrInformationRequestDto model, string email)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var checkregistered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == email);

                        if (checkregistered.RegistrationStatus != SpectaProcessCodes.VerifyBvnPhoneConfirmationCode)
                            return new WebApiResponse { ResponseCode = checkregistered.RegistrationStatus, Message = "Processing stage is not Add-Orr-Information" };
                      
                        var requestmodel = _mapper.Map<AddOrrInformationRequest>(model);
                        await _context.AddOrrInformationRequest.AddAsync(requestmodel);
                       
                        var request = await _spectaOnboardingService.AddOrrInformation(model, email);
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
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", Data = request.Data };
                        
                        await transaction.CommitAsync();

                        return new WebApiResponse { ResponseCode = AppResponseCodes.AddOrrInformation, Message = "Success", Data = request.Data };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _log4net.Error("Error occured" + " | " + "AddOrrInformation" + " | " + ex + " | " + DateTime.Now);
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex.Message };

                    }
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "AddOrrInformation" + " | " + ex + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex.Message };

            }
        }

    }
}
