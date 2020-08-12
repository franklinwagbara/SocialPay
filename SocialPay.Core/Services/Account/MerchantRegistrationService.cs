using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Messaging;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Account
{
    public class MerchantRegistrationService : BaseService<ClientAuthentication>
    {
        private readonly SocialPayDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly EmailService _emailService;
        private readonly Utilities _utilities;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(MerchantRegistrationService));
        public MerchantRegistrationService(SocialPayDbContext context,
            IOptions<AppSettings> appSettings, EmailService emailService,
            Utilities utilities) : base(context)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _utilities = utilities;
        }

        public async Task<WebApiResponse> CreateNewMerchant(SignUpRequestDto signUpRequestDto)
        {
            _log4net.Info("Initiating create account" + " | " + signUpRequestDto.Email + " | " + DateTime.Now);

            try
            {
                if(await _context.ClientAuthentication.AnyAsync(x=>x.Email == signUpRequestDto.Email))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateEmail };
                var token = DateTime.Now.ToString() + Guid.NewGuid().ToString() + DateTime.Now.AddMilliseconds(120) + Utilities.GeneratePin();
                var encryptedToken = token.Encrypt(_appSettings.appKey);
                var newPin = Utilities.GeneratePin();
                var encryptedPin = newPin.Encrypt(_appSettings.appKey);
                if (await _context.PinRequest.AnyAsync(x => x.Pin == encryptedPin))
                {
                    newPin = string.Empty;
                    newPin = Utilities.GeneratePin();
                }
                byte[] passwordHash, passwordSalt;
                var resetUrl = _appSettings.WebportalUrl + encryptedToken;
                string urlPath = "<a href=\"" + resetUrl + "\">Click to confirm your sign up process</a>";
                _utilities.CreatePasswordHash(signUpRequestDto.Password.Encrypt(_appSettings.appKey), out passwordHash, out passwordSalt);
              
                using(var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var model = new ClientAuthentication
                        {
                            ClientSecretHash = passwordHash,
                            ClientSecretSalt = passwordSalt,
                            Email = signUpRequestDto.Email,
                            StatusCode = AppResponseCodes.SignUp,
                            FullName = signUpRequestDto.Fullname,
                            IsDeleted = false,
                            PhoneNumber = signUpRequestDto.PhoneNumber,
                            RoleName = RoleDetails.Merchant,
                            LastDateModified = DateTime.Now
                        };
                        await _context.ClientAuthentication.AddAsync(model);
                        await _context.SaveChangesAsync();

                        var pinRequestModel = new PinRequest
                        {
                            ClientAuthenticationId = model.ClientAuthenticationId,
                            TokenSecret = encryptedToken,
                            Status = false,
                            LastDateModified = DateTime.Now,
                            Pin = encryptedPin
                        };
                        await _context.PinRequest.AddAsync(pinRequestModel);
                        await _context.SaveChangesAsync();
                        var emailModal = new EmailRequestDto
                        {
                            Subject = "Merchant Sign Up",
                            SourceEmail = "info@sterling.ng",
                            DestinationEmail = signUpRequestDto.Email,
                            // DestinationEmail = "festypat9@gmail.com",
                            EmailBody = "Your onboarding was successfully created. Kindly use your email as username and" + "   " + "" + "   " + "as password to login"
                        };

                        var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);
                        if(sendMail != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed };
                        await transaction.CommitAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        _log4net.Error("Error occured" + " | " + signUpRequestDto.Email + " | "+  ex.Message.ToString() + " | "+ DateTime.Now);
                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                    }
                }
                
                
                
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + signUpRequestDto.Email + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> ConfirmSignUp(SignUpConfirmationRequestDto model)
        {
            try
            {
                var encryptPin = model.Pin.Encrypt(_appSettings.appKey);
                var token = model.Token.Trim().Replace(" ", "+");

                var validateToken = await _context.PinRequest.SingleOrDefaultAsync(x => x.TokenSecret == token.Trim()
                && x.Pin == encryptPin && x.Status == false);

                using(var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        if (validateToken == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };
                        var getuserInfo = await _context.ClientAuthentication.
                           SingleOrDefaultAsync(x => x.ClientAuthenticationId == validateToken.ClientAuthenticationId);
                        if (getuserInfo == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };
                        getuserInfo.StatusCode = AppResponseCodes.SignUp;
                        _context.ClientAuthentication.Update(getuserInfo);
                        await _context.SaveChangesAsync();
                        validateToken.Status = true;
                        _context.PinRequest.Update(validateToken);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                    }
                }

            }
            catch (Exception ex )
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> OnboardMerchant(MerchantOnboardingRequestDto model)
        {
            try
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
