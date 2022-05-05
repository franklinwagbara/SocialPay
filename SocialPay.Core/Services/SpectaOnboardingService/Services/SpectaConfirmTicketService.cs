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

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaConfirmTicketService : ISpectaConfirmTicket
    {
        private readonly SocialPayDbContext _context;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaConfirmTicketService));

        public SpectaConfirmTicketService(SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService;
        }

        public async Task<WebApiResponse> ConfirmTicket(ConfirmTicketRequestDto model)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var checkregistered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == model.Email);

                        if (checkregistered.RegistrationStatus != SpectaProcessCodes.RequestTicket)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Processing stage is not Confirm Ticket", StatusCode = ResponseCodes.InternalError };
                        
                        var requestmodel = _mapper.Map<ConfirmTicketRequest>(model);
                        await _context.ConfirmTicketRequest.AddAsync(requestmodel);
                        
                        var request = await _spectaOnboardingService.ConfirmTicket(model);
                        
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return request;
                        
                        var response = (ConfirmTicketResponseDto.ConfirmTicketResponse)request.Data;

                        var confirmticketresponse = new ConfirmTicketResponse();
                        confirmticketresponse.success = response.success;
                        confirmticketresponse.unAuthorizedRequest = response.unAuthorizedRequest;
                        confirmticketresponse.__abp = response.__abp;

                        if (response.error != null)
                        {
                            confirmticketresponse.details = response.error.details;
                            confirmticketresponse.message = response.error.message;
                            confirmticketresponse.validationErrors = response.error.validationErrors;
                        }
                        if (response.result != null)
                        {
                            confirmticketresponse.code = response.result.code;
                            confirmticketresponse.shortDescription = response.result.shortDescription;
                            confirmticketresponse.limitExpiryDate = response.result.limitExpiryDate;
                            confirmticketresponse.loanLimit = response.result.loanLimit;

                        }
                       
                        await _context.ConfirmTicketResponse.AddAsync(confirmticketresponse);
                       
                        if (checkregistered != null) { checkregistered.RegistrationStatus = SpectaProcessCodes.ConfirmTicket; }
                        
                        await _context.SaveChangesAsync();
                       
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", Data = request.Data, StatusCode = ResponseCodes.InternalError };
                       
                        await transaction.CommitAsync();
                        
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = request.Data, StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _log4net.Error("Error occured" + " | " + "ConfirmTicket" + " | " + ex + " | " + DateTime.Now);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed ", StatusCode = ResponseCodes.InternalError };
                    }
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "ConfirmTicket" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex, StatusCode = ResponseCodes.InternalError };
            }
        }
    }
}
