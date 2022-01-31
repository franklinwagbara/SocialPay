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
    public class SpectaCreateIndividualCurrentAccountService : ISpectaCreateIndividualCurrentAccount
    {
        private readonly SocialPayDbContext _context;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(SpectaCreateIndividualCurrentAccountService));

        public SpectaCreateIndividualCurrentAccountService(SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService;
        }
        public async Task<WebApiResponse> CreateIndividualCurrentAccount(CreateIndividualCurrentAccountRequestDto model)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var checkregistered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == model.Email);

                        if (checkregistered.RegistrationStatus != SpectaProcessCodes.ConfirmTicket 
                            && checkregistered.RegistrationStatus != SpectaProcessCodes.RequestTicket 
                            && checkregistered.RegistrationStatus != SpectaProcessCodes.AddOrrInformation)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Processing stage is not Create Individual Current Account", StatusCode = ResponseCodes.InternalError };
                       
                        var requestmodel = _mapper.Map<CreateIndividualCurrentAccountRequest>(model);
                        await _context.CreateIndividualCurrentAccountRequest.AddAsync(requestmodel);
                        
                        var request = await _spectaOnboardingService.CreateIndividualCurrentAccount(model, model.Email);
                        
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return request;
                       
                        var response = (CreateIndividualCurrentAccountResponseDto.CreateIndividualCurrentAccountResponse)request.Data;
                        var individualcurrentaccountresponse = new CreateIndividualCurrentAccountResponse();
                        individualcurrentaccountresponse.success = response.success;
                        individualcurrentaccountresponse.unAuthorizedRequest = response.unAuthorizedRequest;
                        individualcurrentaccountresponse.__abp = response.__abp;
                        
                        if (response.error != null)
                        {
                            individualcurrentaccountresponse.code = response.error.code;
                            individualcurrentaccountresponse.details = response.error.details;
                            individualcurrentaccountresponse.message = response.error.message;
                            individualcurrentaccountresponse.validationErrors = response.error.validationErrors;
                        }

                        if (response.result != null)
                        {
                            individualcurrentaccountresponse.openedCurrentAccount = response.result.openedCurrentAccount;
                        }
                       
                        await _context.CreateIndividualCurrentAccountResponse.AddAsync(individualcurrentaccountresponse);
                       
                     //   if (checkregistered != null) { checkregistered.RegistrationStatus = SpectaProcessCodes.CreateIndividualCurrentAccount; }
                        await _context.SaveChangesAsync();
                       
                        if (request.ResponseCode != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", Data = request.Data, StatusCode = ResponseCodes.InternalError };
                       
                        await transaction.CommitAsync();
                        
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = request.Data, StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _log4net.Error("Error occured" + " | " + "CreateIndividualCurrentAccount" + " | " + ex + " | " + DateTime.Now);
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed " + ex, StatusCode = ResponseCodes.InternalError };
                    }
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "CreateIndividualCurrentAccount" + " | " + ex + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed ", StatusCode = ResponseCodes.InternalError };
            }
        }
    }
}
