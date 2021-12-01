using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialPay.Core.Services.Specta;
using SocialPay.Core.Services.SpectaOnboardingService.Interface;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Services
{
    public class SpectaCustomerRegistration : ISpectaCustomerRegistration
    {
        private readonly SocialPayDbContext _context;
        private readonly ISpectaOnBoarding _spectaOnboardingService;
        private readonly IMapper _mapper;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(PayWithSpectaService));

        public SpectaCustomerRegistration(SocialPayDbContext context, ISpectaOnBoarding spectaOnboardingService, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _spectaOnboardingService = spectaOnboardingService;
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

                        await _context.SpectaRegisterCustomerRequest.AddAsync(requestmodel);
                        await _context.SaveChangesAsync();

                        var request = await _spectaOnboardingService.RegisterCustomer(model);
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
                        
                        return new WebApiResponse { ResponseCode = SpectaProcessCodes.RegisterCustomer, Message = "Success", StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _log4net.Error("Error occured" + " | " + "CustomerRegistration" + " | " + ex + " | " + DateTime.Now);
                        return new WebApiResponse { ResponseCode = SpectaProcessCodes.Failed, Message = "Request failed", StatusCode = ResponseCodes.InternalError };
                    }
                }
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "CustomerRegistration" + " | " + ex + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = SpectaProcessCodes.Failed, Message = "Request failed ", StatusCode = ResponseCodes.InternalError };

            }
        }
    }

}
