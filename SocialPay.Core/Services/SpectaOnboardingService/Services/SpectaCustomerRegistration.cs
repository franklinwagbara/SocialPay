﻿using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Services.ISpectaOnboardingService;
using SocialPay.Core.Services.Specta;
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
    public class SpectaCustomerRegistration : ISpectaCustomerRegistration
    {
        private readonly SocialPayDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        private readonly SpectaOnboardingLogger _spectaOnboardingLogger;

        public SpectaCustomerRegistration(IOptions<AppSettings> appSettings, SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper, SpectaOnboardingLogger spectaOnboardingLogger)
        {
            _appSettings = appSettings.Value;
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService ?? throw new ArgumentNullException(nameof(spectaOnboardingService));
            _spectaOnboardingLogger = spectaOnboardingLogger;
        }

        public async Task<WebApiResponse> RegisterCustomer(RegisterCustomerRequestDto model)
        {
            try
            {
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var registered = await _context.SpectaRegisterCustomerRequest.SingleOrDefaultAsync(x => x.emailAddress == model.emailAddress);

                        if (registered != null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateEmail, Message = "Duplicate email", StatusCode = ResponseCodes.Duplicate };

                        var requestmodel = _mapper.Map<SpectaRegisterCustomerRequest>(model);
                        requestmodel.RegistrationStatus = SpectaProcessCodes.RegisterCustomer;
                        var password = requestmodel.password;
                        requestmodel.password = password.Encrypt(_appSettings.appKey);

                        await _context.SpectaRegisterCustomerRequest.AddAsync(requestmodel);
                        await _context.SaveChangesAsync();

                        var request = await _spectaOnboardingService.RegisterCustomer(model);

                        if (request.ResponseCode != AppResponseCodes.Success)
                            return request;

                        var response = (SpectaResponseWithObjectResultMessage.SpectaResponseDto)request.Data;

                        var customerregresponse = new SpectaRegisterCustomerResponse();

                        customerregresponse.success = Convert.ToBoolean(response?.success);
                        customerregresponse.unAuthorizedRequest = response.unAuthorizedRequest;
                        customerregresponse.__abp = Convert.ToBoolean(response?.__abp);

                        if (response.error != null)
                        {
                            customerregresponse.code = response.error.code;
                            customerregresponse.details = response.error.details;
                            customerregresponse.message = response.error.message;
                            customerregresponse.validationErrors = response.error.validationErrors;
                        }

                        customerregresponse.SpectaRegisterCustomerRequestId = requestmodel.SpectaRegisterCustomerRequestId;

                        await _context.SpectaRegisterCustomerResponse.AddAsync(customerregresponse);

                        await _context.SaveChangesAsync();

                        if (request.ResponseCode != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Request failed", StatusCode = ResponseCodes.Badrequest };

                        await transaction.CommitAsync();

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _spectaOnboardingLogger.LogRequest($"{"Error occured -- CustomerRegistration " + ex.ToString()}{"-"}{DateTime.Now}", true);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Request failed", StatusCode = ResponseCodes.InternalError };
                    }
                }
            }
            catch (Exception ex)
            {
                _spectaOnboardingLogger.LogRequest($"{"Error occured -- CustomerRegistration " + ex.ToString()}{"-"}{DateTime.Now}", true);
                return new WebApiResponse { ResponseCode = SpectaProcessCodes.Failed, Message = "Request failed ", StatusCode = ResponseCodes.InternalError };

            }
        }
    }

}
