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
    public class SpectaRequestTicketService : ISpectaRequestTicket
    {

        private readonly SocialPayDbContext _context;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        private readonly SpectaOnboardingLogger _spectaOnboardingLogger;
        public SpectaRequestTicketService(SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper, SpectaOnboardingLogger spectaOnboardingLogger)
        {
            _context = context;
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService;
            _spectaOnboardingLogger = spectaOnboardingLogger;
        }
        public async Task<WebApiResponse> RequestTicket(RequestTicketDto model)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var checkregistered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == model.Email);
                        if (!checkregistered.RegistrationStatus.Equals(SpectaProcessCodes.AddOrrInformation))
                        {
                            checkregistered.RegistrationStatus = SpectaProcessCodes.AddOrrInformation;
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                        }
                        if (checkregistered.RegistrationStatus != SpectaProcessCodes.AddOrrInformation)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Processing stage is not Request Ticket", StatusCode = ResponseCodes.InternalError };
                      
                        var requestmodel = _mapper.Map<RequestTicketRequest>(model);
                        await _context.RequestTicketRequest.AddAsync(requestmodel);

                        var request = await _spectaOnboardingService.RequestTicket(model);
                        
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return request;

                        var response = (RequestTicketResponseDto.RequestTicketResponse)request.Data;

                        var requestticketresponse = new RequestTicketResponse();
                        requestticketresponse.success = response.success;
                        requestticketresponse.unAuthorizedRequest = response.unAuthorizedRequest;
                        requestticketresponse.__abp = response.__abp;

                        if (response.error != null)
                        {
                            requestticketresponse.code = response.error.code;
                            requestticketresponse.details = response.error.details;
                            requestticketresponse.message = response.error.message;
                            requestticketresponse.validationErrors = response.error.validationErrors;
                        }
                       
                        if (response.result != null)
                        {
                            requestticketresponse.shortDescription = response.result.shortDescription;
                            requestticketresponse.requestId = response.result.requestId;
                        }
                        await _context.RequestTicketResponse.AddAsync(requestticketresponse);
                       
                        if (checkregistered != null) { checkregistered.RegistrationStatus = SpectaProcessCodes.RequestTicket; }
                        await _context.SaveChangesAsync();
                       
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", Data = request.Data, StatusCode = ResponseCodes.InternalError };
                       
                        await transaction.CommitAsync();
                        
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = request.Data, StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _spectaOnboardingLogger.LogRequest($"{"Error occured -- RequestTicket" + ex.ToString()}{"-"}{DateTime.Now}", true);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", StatusCode = ResponseCodes.InternalError };
                    }
                }
            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- RequestTicket" + ex.ToString()}{"-"}{DateTime.Now}", true);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed ", StatusCode = ResponseCodes.InternalError };
            }
        }
    }
}
