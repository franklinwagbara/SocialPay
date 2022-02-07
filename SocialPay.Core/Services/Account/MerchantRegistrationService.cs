﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Messaging;
using SocialPay.Core.Messaging.SendGrid;
using SocialPay.Core.Services.EventLogs;
using SocialPay.Core.Services.IBS;
using SocialPay.Core.Services.Tin;
using SocialPay.Core.Services.Validations;
using SocialPay.Core.Services.Wallet;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly BankServiceRepository _bankServiceRepository;
        private readonly IBSReposervice _iBSReposervice;
        private readonly WalletRepoService _walletRepoService;
        private readonly TinService _tinService;
        private readonly IDistributedCache _distributedCache;
        private readonly SendGridEmailService _sendGridEmailService;
        private readonly EventLogService _eventLogService;

        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(MerchantRegistrationService));
        public MerchantRegistrationService(SocialPayDbContext context,
            IOptions<AppSettings> appSettings, EmailService emailService,
            Utilities utilities, IHostingEnvironment environment,
            BankServiceRepository bankServiceRepository,
            IBSReposervice iBSReposervice, WalletRepoService walletRepoService,
            TinService tinService,
            IDistributedCache distributedCache, SendGridEmailService sendGridEmailService,
            EventLogService eventLogService) : base(context)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _utilities = utilities;
            _hostingEnvironment = environment;
            _bankServiceRepository = bankServiceRepository;
            _iBSReposervice = iBSReposervice;
            _walletRepoService = walletRepoService;
            _distributedCache = distributedCache;
            _tinService = tinService;
            _sendGridEmailService = sendGridEmailService;
            _eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
        }

        private async Task<string> GetReferCode()
        {
            try
            {
                var generator = new Random();

                string refercode = generator.Next(100000, 1000000).ToString();

                if (!await _context.ClientAuthentication.AnyAsync(x => x.ReferCode == refercode))
                    return refercode;

                return await GetReferCode();
            }
            catch (Exception ex)
            {
                return "0";
            }
        }

        public async Task<WebApiResponse> CreateNewMerchant(SignUpRequestDto signUpRequestDto)
        {
            _log4net.Info("Initiating create merchant account" + " | " + signUpRequestDto.Email + " | " + DateTime.Now);

            try
            {
                if (await _context.ClientAuthentication.AnyAsync(x => x.Email == signUpRequestDto.Email
                || x.PhoneNumber == signUpRequestDto.PhoneNumber || x.Bvn == signUpRequestDto.Bvn))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateMerchantDetails, Message = ResponseMessage.DuplicateRecord };


                var validateUser = await _bankServiceRepository.BvnValidation(signUpRequestDto.Bvn,
                    signUpRequestDto.DateOfBirth, signUpRequestDto.FirstName.ToLower(),
                    signUpRequestDto.LastName.ToLower(), signUpRequestDto.Email);

                if (validateUser.ResponseCode != AppResponseCodes.Success)
                {
                    _log4net.Info("BVN validation response" + " | " + signUpRequestDto.Email + " | " + signUpRequestDto.Bvn + " - " + validateUser.Message + " - " + DateTime.Now);

                    return new WebApiResponse { ResponseCode = validateUser.ResponseCode, Data = ResponseMessage.BvnValidation, Message = ResponseMessage.BvnValidation };
                }

                var token = $"{DateTime.Now.ToString()}{Guid.NewGuid().ToString()}{DateTime.Now.AddMilliseconds(120)}{Utilities.GeneratePin()}";
                var encryptedToken = token.Encrypt(_appSettings.appKey);
                var newPin = Utilities.GeneratePin();// + DateTime.Now.Day;
                var encryptedPin = newPin.Encrypt(_appSettings.appKey);

                if (await _context.PinRequest.AnyAsync(x => x.Pin == encryptedPin))
                {
                    newPin = string.Empty;
                    newPin = Utilities.GeneratePin();

                    encryptedPin = newPin.Encrypt(_appSettings.appKey);
                }

                byte[] passwordHash, passwordSalt;

                var resetUrl = _appSettings.WebportalUrl + encryptedToken;

                string urlPath = "<a href=\"" + resetUrl + "\">Click to confirm your sign up process</a>";

                _utilities.CreatePasswordHash(signUpRequestDto.Password.Encrypt(_appSettings.appKey), out passwordHash, out passwordSalt);

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    string referCode = await GetReferCode();
                    try
                    {
                        var model = new ClientAuthentication
                        {
                            ClientSecretHash = passwordHash,
                            ClientSecretSalt = passwordSalt,
                            Bvn = signUpRequestDto.Bvn,
                            Email = signUpRequestDto.Email,
                            StatusCode = MerchantOnboardingProcess.CreateAccount,
                            FullName = $"{signUpRequestDto.FirstName}{" "}{signUpRequestDto.LastName}",
                            IsDeleted = false,
                            PhoneNumber = signUpRequestDto.PhoneNumber,
                            RoleName = RoleDetails.Merchant,
                            LastDateModified = DateTime.Now,
                            UserName = signUpRequestDto.Email,
                            IsLocked = false,
                            ReferralCode = signUpRequestDto.ReferralCode,
                            ReferCode = $"{"SP-"}{referCode}",
                            HasRegisteredCompany = signUpRequestDto.HasRegisteredCompany
                        };

                        await _context.ClientAuthentication.AddAsync(model);
                        await _context.SaveChangesAsync();

                        var merchantWallet = new MerchantWallet
                        {
                            ClientAuthenticationId = model.ClientAuthenticationId,
                            CurrencyCode = _appSettings.currencyCode,
                            DoB = signUpRequestDto.DateOfBirth,
                            Firstname = signUpRequestDto.FirstName,
                            Lastname = signUpRequestDto.LastName,
                            Mobile = signUpRequestDto.PhoneNumber,
                            Gender = signUpRequestDto.Gender,
                            LastDateModified = DateTime.Now,
                            status = MerchantWalletProcess.CreateAccount
                        };

                        await _context.MerchantWallet.AddAsync(merchantWallet);
                        await _context.SaveChangesAsync();

                        var loginstatus = new ClientLoginStatus
                        {
                            ClientAuthenticationId = model.ClientAuthenticationId,
                            IsSuccessful = true,
                            LoginAttempt = 0
                        };

                        await _context.ClientLoginStatus.AddAsync(loginstatus);
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

                        var accountHistory = new AccountHistory
                        {
                            ClientAuthenticationId = model.ClientAuthenticationId,
                            ClientSecretHash = passwordHash,
                            ClientSecretSalt = passwordSalt,
                            ClientSecret = signUpRequestDto.Password.Encrypt(_appSettings.appKey),
                        };

                        await _context.AccountHistory.AddAsync(accountHistory);
                        await _context.SaveChangesAsync();

                        var emailModal = new EmailRequestDto
                        {
                            Subject = "Merchant Signed Up",
                            DestinationEmail = signUpRequestDto.Email,
                            // DestinationEmail = "festypat9@gmail.com",
                            //  EmailBody = "Your onboarding was successfully created. Kindly use your email as username and" + "   " + "" + "   " + "as password to login"
                        };

                        var mailBuilder = new StringBuilder();
                        mailBuilder.AppendLine("Dear" + " " + signUpRequestDto.FirstName + "," + "<br />");
                        mailBuilder.AppendLine("<br />");
                        mailBuilder.AppendLine("You have successfully sign up. Please confirm your sign up by clicking the link below.<br />");
                        mailBuilder.AppendLine("Kindly use this token" + "  " + newPin + "  " + "and" + " " + urlPath + "<br />");
                        // mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                        mailBuilder.AppendLine("Best Regards,");
                        emailModal.EmailBody = mailBuilder.ToString();

                        // var sendMail = await _sendGridEmailService.SendMail(mailBuilder.ToString(), emailModal.DestinationEmail, emailModal.Subject);

                        var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

                        if (sendMail != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = "Error occured while sending email" };

                        await transaction.CommitAsync();

                        var eventLog = new EventRequestDto
                        {
                            ModuleAccessed = EventLogProcess.MerchantSignUp,
                            Description = "Merchant Signup was successful",
                            UserId = signUpRequestDto.Email,
                            ClientAuthenticationId = model.ClientAuthenticationId
                        };

                        await _eventLogService.ActivityRequestLog(eventLog);

                        _log4net.Info("Initiating create merchant account was successful" + " | " + signUpRequestDto.Email + " | " + DateTime.Now);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = ResponseMessage.Success };
                    }
                    catch (Exception ex)
                    {
                        _log4net.Error("Error occured" + " | " + signUpRequestDto.Email + " | " + ex + " | " + DateTime.Now);
                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ResponseMessage.InternalError };
                    }
                }



            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + signUpRequestDto.Email + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ResponseMessage.InternalError };
            }
        }

        public async Task<string> SendWelcomeEmail(ClientAuthentication getuserInfo, EmailService _emailService)
        {
            var emailModal = new EmailRequestDto
            {
                Subject = "Merchant Signed Up Completed",
                DestinationEmail = getuserInfo.Email,
            };

            var mailBuilder = new StringBuilder();
            mailBuilder.AppendLine("<div>");
            mailBuilder.AppendLine("<div style='width: 710px; padding: 5px 0 5px 0;'><img style='width: 98%; height: 350px; display: block; margin: 0 auto;' src=" + _appSettings.WebportalBaseUrl + _appSettings.WelcomeEmailImageExtensionPath + " +  alt='Logo'/>");
            mailBuilder.AppendLine("<div class='text-left' style='background-color: #f0f3f6; width: 700px; min-height: 200px;'>");
            mailBuilder.AppendLine("<div style='padding: 10px 30px 30px 30px;'>");
            mailBuilder.AppendLine("Hello" + " <b>" + getuserInfo.FullName + "</b>," + "<br />");
            mailBuilder.AppendLine("<br />");
            mailBuilder.AppendLine("Thank you for signing up on to Social Pay. We so are excited to have you join us.<br />");
            mailBuilder.AppendLine("<br />");
            mailBuilder.AppendLine("Social pay is a secure and convenient solution which allows you make and receive payments easily through a simple link, whether you have a physical store or sell online.<br />");
            mailBuilder.AppendLine("<br />");
            mailBuilder.AppendLine("Here are some of the benefits you get on social pay:<br/>");
            mailBuilder.AppendLine("<ul>");
            mailBuilder.AppendLine("<li>Setting up is absolutely free.</li><br/>");
            mailBuilder.AppendLine("<li>You can start receiving payments instantly, no stories!</li><br/>");
            mailBuilder.AppendLine("<li>Your customers can spread out their payments using PayWithSpecta while you still get paid in full, so you get to sell even more.</li><br />");
            mailBuilder.AppendLine("<li>All your transactions can be tracked and you can generate periodic reports</li>.<br/>");
            mailBuilder.AppendLine("<li>Access to a free invoicing system which eliminates the challenge of creating invoices for your customers.</li><br />");
            mailBuilder.AppendLine("</ul>");
            mailBuilder.AppendLine("<b>" + getuserInfo.FullName + "</b>" + " you no longer need to shout to collect payments, we are here to make your life easier.<br />");
            mailBuilder.AppendLine("<br />");
            mailBuilder.AppendLine("Once again thank you for signing up, you will be hearing from me as we go on this journey with you.<br />");
            mailBuilder.AppendLine("<br />");
            mailBuilder.AppendLine("Your Friend");
            mailBuilder.AppendLine("</div>");
            mailBuilder.AppendLine("</div>");
            mailBuilder.AppendLine("</div>");
            emailModal.EmailBody = mailBuilder.ToString();
            var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

            return sendMail;
        }


        public async Task<WebApiResponse> ConfirmSignUp(SignUpConfirmationRequestDto model)
        {
            try
            {
                _log4net.Info("Initiating confirm signup" + " | " + model.Pin + " | " + DateTime.Now);

                //model.Token = "1At4AGMX7HISvClyC2/mfnc2e/hp6n3gI4yoH7tAej+H+4UQTmnnyG5Rklpqjl02fgj2zoMbDv+ipMNeyDdrTin+/mcQ38u8L+HizkA1CKpAvf1Pxryz+nRB6UbsxhgA";
                var encryptPin = model.Pin.Encrypt(_appSettings.appKey);
                // var token = model.Token.Trim().Replace(" ", "+");
                //1At4AGMX7HISvClyC2/mfnc2e/hp6n3gI4yoH7tAej+H+4UQTmnnyG5Rklpqjl02fgj2zoMbDv+ipMNeyDdrTin+/mcQ38u8L+HizkA1CKpAvf1Pxryz+nRB6UbsxhgA
                //var validateToken = await _context.PinRequest.SingleOrDefaultAsync(x => x.Pin == encryptPin
                //&& x.Status == false && x.TokenSecret == model.Token);

                var validateToken = await _context.PinRequest.SingleOrDefaultAsync(x => x.Pin == encryptPin
                && x.Status == false);

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        if (validateToken == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = ResponseMessage.RecordNotFound };

                        if (validateToken.LastDateModified.AddMinutes(Convert.ToInt32(_appSettings.otpSession)) < DateTime.Now)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.OtpExpired, Message = ResponseMessage.OtpExpired };

                        var getuserInfo = await _context.ClientAuthentication.
                           SingleOrDefaultAsync(x => x.ClientAuthenticationId == validateToken.ClientAuthenticationId);

                        if (getuserInfo == null)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = ResponseMessage.RecordNotFound };

                        getuserInfo.StatusCode = MerchantOnboardingProcess.SignUp;

                        if (!getuserInfo.HasRegisteredCompany)
                            getuserInfo.StatusCode = MerchantOnboardingProcess.BusinessInfo;

                        _context.ClientAuthentication.Update(getuserInfo);
                        await _context.SaveChangesAsync();
                        validateToken.Status = true;

                        _context.PinRequest.Update(validateToken);

                        var cacheKey = Convert.ToString(getuserInfo.ClientAuthenticationId);
                        string serializedCustomerList;
                        var userInfo = new UserInfoViewModel { };
                        var redisCustomerList = await _distributedCache.GetAsync(cacheKey);

                        var eventLog = new EventRequestDto();

                        if (redisCustomerList != null)
                        {
                            await _distributedCache.RemoveAsync(cacheKey);
                            userInfo.Email = getuserInfo.Email;
                            userInfo.StatusCode = getuserInfo.StatusCode;
                            serializedCustomerList = JsonConvert.SerializeObject(userInfo);
                            redisCustomerList = Encoding.UTF8.GetBytes(serializedCustomerList);

                            var options1 = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(DateTime.Now.AddMinutes(3))
                            .SetSlidingExpiration(TimeSpan.FromMinutes(1));

                            await _distributedCache.SetAsync(cacheKey, redisCustomerList, options1);

                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();


                            eventLog.ModuleAccessed = EventLogProcess.MerchantSignUpConfirmation;
                            eventLog.Description = "Merchant Signup confirmation was successful";
                            eventLog.UserId = getuserInfo.Email;
                            eventLog.ClientAuthenticationId = getuserInfo.ClientAuthenticationId;

                            await _eventLogService.ActivityRequestLog(eventLog);

                            var sendMail = await SendWelcomeEmail(getuserInfo, _emailService);

                            if (sendMail == AppResponseCodes.Success)
                            {
                                _log4net.Info("Welcome email have been sent" + " | " + model.Pin + " | " + DateTime.Now);
                            }
                            else
                            {
                                _log4net.Info("Unable to send Welcome email" + " | " + model.Pin + " | " + DateTime.Now);

                            }
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = ResponseMessage.Success, Data = getuserInfo.Email };



                            // _log4net.Info("Initiating confirm signup was successful" + " | " + model.Pin + " | " + DateTime.Now);

                            // return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = ResponseMessage.Success, Data = getuserInfo.Email };
                        }

                       else
                        {
                            await _distributedCache.RemoveAsync(cacheKey);
                            userInfo.Email = getuserInfo.Email;
                            userInfo.StatusCode = getuserInfo.StatusCode;
                            serializedCustomerList = JsonConvert.SerializeObject(userInfo);
                            redisCustomerList = Encoding.UTF8.GetBytes(serializedCustomerList);

                            var options = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(DateTime.Now.AddMinutes(30))
                            .SetSlidingExpiration(TimeSpan.FromMinutes(15));
                            await _distributedCache.SetAsync(cacheKey, redisCustomerList, options);

                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();

                            eventLog.ModuleAccessed = EventLogProcess.MerchantSignUpConfirmation;
                            eventLog.Description = "Merchant Signup confirmation was successful";
                            eventLog.UserId = getuserInfo.Email;
                            eventLog.ClientAuthenticationId = getuserInfo.ClientAuthenticationId;

                            await _eventLogService.ActivityRequestLog(eventLog);
                            _log4net.Info("Initiating confirm signup was successful" + " | " + model.Pin + " | " + DateTime.Now);

                            var sendMail = await SendWelcomeEmail(getuserInfo, _emailService);

                            if (sendMail == AppResponseCodes.Success)
                            {
                                _log4net.Info("Welcome email have been sent" + " | " + model.Pin + " | " + DateTime.Now);
                            }
                            else
                            {
                                _log4net.Info("Unable to send Welcome email" + " | " + model.Pin + " | " + DateTime.Now);

                            }

                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = ResponseMessage.Success, Data = getuserInfo.Email };
                        }

                    }
                    catch (Exception ex)
                    {
                        _log4net.Error("Error occured" + " | " + "ConfirmSignup" + " | " + model.Pin + " | " + ex + " | " + DateTime.Now);

                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ResponseMessage.InternalError };
                    }
                }

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "ConfirmSignup" + " | " + model.Pin + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ResponseMessage.InternalError };
            }
        }

        public async Task<WebApiResponse> RequestNewToken(SignUpConfirmationRequestDto model)
        {
            try
            {
                _log4net.Info("Initiating new token request" + " | " + model.Token + " | " + DateTime.Now);

                // var encryptPin = model.Pin.Encrypt(_appSettings.appKey);
                //var token = model.Token.Trim().Replace(" ", "+");

                var validateToken = await _context.PinRequest
                    .SingleOrDefaultAsync(x => x.TokenSecret == model.Token);


                if (validateToken == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = ResponseMessage.RecordNotFound };

                var newPin = Utilities.GeneratePin();

                validateToken.Pin = newPin.Encrypt(_appSettings.appKey);
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {

                        var newToken = DateTime.Now.ToString() + Guid.NewGuid().ToString() + DateTime.Now.AddMilliseconds(120) + Utilities.GeneratePin();
                        var encryptedToken = newToken.Encrypt(_appSettings.appKey);
                        var resetUrl = _appSettings.WebportalUrl + encryptedToken;
                        string urlPath = "<a href=\"" + resetUrl + "\">Click to confirm your sign up process</a>";
                        var userInfo = await _context.ClientAuthentication
                             .SingleOrDefaultAsync(x => x.ClientAuthenticationId == validateToken.ClientAuthenticationId);

                        validateToken.TokenSecret = encryptedToken;
                        validateToken.LastDateModified = DateTime.Now;
                        _context.PinRequest.Update(validateToken);
                        await _context.SaveChangesAsync();

                        var emailModal = new EmailRequestDto
                        {
                            Subject = "Merchant Signed Up",
                            SourceEmail = "info@sterling.ng",
                            DestinationEmail = userInfo.Email,
                        };
                        var mailBuilder = new StringBuilder();
                        mailBuilder.AppendLine("Dear" + " " + userInfo.Email + "," + "<br />");
                        mailBuilder.AppendLine("<br />");
                        mailBuilder.AppendLine("You have successfully sign up. Please confirm your sign up by clicking the link below.<br />");
                        mailBuilder.AppendLine("Kindly use this token" + "  " + newPin + "  " + "and" + " " + urlPath + "<br />");
                        // mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                        mailBuilder.AppendLine("Best Regards,");
                        emailModal.EmailBody = mailBuilder.ToString();

                        //var sendMail = await _sendGridEmailService.SendMail(mailBuilder.ToString(), emailModal.DestinationEmail, emailModal.Subject);

                        var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

                        if (sendMail != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Message = ResponseMessage.SendMailFailed };


                        await transaction.CommitAsync();


                        _log4net.Info("Initiating RequestNewToken was successful" + " | " + DateTime.Now);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = ResponseMessage.Success };
                    }
                    catch (Exception ex)
                    {
                        _log4net.Error("Error occured" + " | " + "RequestNewToken" + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                        await transaction.RollbackAsync();

                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ResponseMessage.Success };
                    }
                }

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "RequestNewToken" + " | " + ex + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ResponseMessage.InternalError };
            }
        }

        public async Task<WebApiResponse> OnboardMerchantBusinessInfo(MerchantOnboardingInfoRequestDto model, long clientId)
        {
            try
            {

                _log4net.Info("Initiating OnboardMerchantBusinessInfo request" + " | " + model.BusinessName + " | " + model.BusinessEmail + " | " + model.BusinessPhoneNumber + " | " + DateTime.Now);

                if (!string.IsNullOrEmpty(model.Tin))
                {
                    var validateTin = await _tinService.ValidateTin(model.Tin);

                    if (validateTin.ResponseCode != AppResponseCodes.Success)
                        return validateTin;
                }

                if (await _context.MerchantBusinessInfo.AnyAsync(x => x.BusinessEmail == model.BusinessEmail ||
                 x.BusinessPhoneNumber == model.BusinessPhoneNumber || x.Chargebackemail == model.Chargebackemail))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateMerchantDetails, Message = ResponseMessage.DuplicateRecord };

                var getUserInfo = await _context.ClientAuthentication
                    .Include(x => x.MerchantBusinessInfo).SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);

                if (getUserInfo.MerchantBusinessInfo.Count > 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.MerchantInfoAlreadyExist, Message = ResponseMessage.DuplicateRecord };


                string fileName = string.Empty;
                var merchantId = Guid.NewGuid().ToString("N").Substring(22);
                var newFileName = string.Empty;
                var filePath = string.Empty;
                var fileExtension = string.Empty;

                if (model.Logo != null)
                {
                    fileName = (model.Logo.FileName);

                    fileExtension = Path.GetExtension(fileName);
                    fileName = Path.Combine(_hostingEnvironment.WebRootPath, "MerchantLogo") + $@"\{newFileName}";

                    // concating  FileName + FileExtension
                    newFileName = merchantId + fileExtension;
                    filePath = Path.Combine(fileName, newFileName);
                }


                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {

                        var businessInfoModel = new MerchantBusinessInfo
                        {
                            BusinessEmail = model.BusinessEmail,
                            BusinessName = model.BusinessName,
                            BusinessPhoneNumber = model.BusinessPhoneNumber,
                            Chargebackemail = model.Chargebackemail,
                            ClientAuthenticationId = clientId,
                            Country = model.Country,
                            Tin = model.Tin,
                            MerchantReferenceId = merchantId,
                            FileLocation = "MerchantLogo",
                            Logo = newFileName,
                            HasSpectaMerchantID = string.IsNullOrEmpty(model.SpectaMerchantID) ? false : true,
                            SpectaMerchantID = string.IsNullOrEmpty(model.SpectaMerchantID) ? string.Empty : model.SpectaMerchantID,
                            SpectaMerchantKey = string.IsNullOrEmpty(model.SpectaMerchantKey) ? string.Empty : model.SpectaMerchantKey,
                            SpectaMerchantKeyValue = string.IsNullOrEmpty(model.SpectaMerchantKeyValue) ? string.Empty : model.SpectaMerchantKeyValue
                        };


                        await _context.MerchantBusinessInfo.AddAsync(businessInfoModel);
                        await _context.SaveChangesAsync();

                        getUserInfo.StatusCode = MerchantOnboardingProcess.BusinessInfo;
                        getUserInfo.LastDateModified = DateTime.Now;
                        await _context.SaveChangesAsync();

                        var cacheKey = Convert.ToString(clientId);
                        string serializedCustomerList;
                        var userInfo = new UserInfoViewModel { };

                        var redisCustomerList = await _distributedCache.GetAsync(cacheKey);

                        if (redisCustomerList != null)
                        {
                            await _distributedCache.RemoveAsync(cacheKey);
                            userInfo.Email = getUserInfo.Email;
                            userInfo.StatusCode = getUserInfo.StatusCode;
                            serializedCustomerList = JsonConvert.SerializeObject(userInfo);
                            redisCustomerList = Encoding.UTF8.GetBytes(serializedCustomerList);
                            var options1 = new DistributedCacheEntryOptions()

                            .SetAbsoluteExpiration(DateTime.Now.AddMinutes(30))
                            .SetSlidingExpiration(TimeSpan.FromMinutes(15));

                            await _distributedCache.SetAsync(cacheKey, redisCustomerList, options1);
                        }

                        await _distributedCache.RemoveAsync(cacheKey);
                        userInfo.Email = getUserInfo.Email;
                        userInfo.StatusCode = getUserInfo.StatusCode;
                        serializedCustomerList = JsonConvert.SerializeObject(userInfo);
                        redisCustomerList = Encoding.UTF8.GetBytes(serializedCustomerList);

                        var options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(30))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(15));

                        await _distributedCache.SetAsync(cacheKey, redisCustomerList, options);

                        var eventLog = new EventRequestDto();
                        eventLog.ModuleAccessed = EventLogProcess.MerchantBusinessInfo;
                        eventLog.Description = "Merchant business info setup";
                        eventLog.UserId = getUserInfo.Email;
                        eventLog.ClientAuthenticationId = getUserInfo.ClientAuthenticationId;

                        if (model.Logo == null)
                        {
                            await transaction.CommitAsync();
                            _log4net.Info("Initiating OnboardMerchantBusinessInfo request was successful" + " | " + model.BusinessName + " | " + model.BusinessEmail + " | " + model.BusinessPhoneNumber + " | " + DateTime.Now);


                            await _eventLogService.ActivityRequestLog(eventLog);

                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, UserStatus = MerchantOnboardingProcess.BusinessInfo, Message = ResponseMessage.Success };
                        }

                        model.Logo.CopyTo(new FileStream(filePath, FileMode.Create));

                        await transaction.CommitAsync();

                        await _eventLogService.ActivityRequestLog(eventLog);

                        _log4net.Info("Initiating OnboardMerchantBusinessInfo request was successful" + " | " + model.BusinessName + " | " + model.BusinessEmail + " | " + model.BusinessPhoneNumber + " | " + DateTime.Now);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = ResponseMessage.Success, UserStatus = MerchantOnboardingProcess.BusinessInfo };
                    }
                    catch (Exception ex)
                    {
                        _log4net.Error("An error ocuured while saving merchant business info" + model.BusinessEmail + " | " + ex + " | " + DateTime.Now);
                        await transaction.RollbackAsync();

                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ResponseMessage.InternalError };
                    }

                }

            }
            catch (Exception ex)
            {
                _log4net.Error("An error ocuured while saving merchant business info" + model.BusinessEmail + " | " + ex + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ResponseMessage.InternalError };
            }
        }


        public async Task<WebApiResponse> OnboardMerchantBankInfo(MerchantBankInfoRequestDto model, long clientId)
        {
            try
            {
                //  clientId = 96;
                _log4net.Info("Initiating OnboardMerchantBankInfo request" + " | " + model.BankCode + " | " + model.BankName + " | " + clientId + " | " + DateTime.Now);

                if (await _context.MerchantBankInfo.AnyAsync(x => x.Nuban == model.Nuban && x.ClientAuthenticationId == clientId))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateMerchantDetails, Message = ResponseMessage.DuplicateRecord };

                //var nibsRequestModel = new IBSNameEnquiryRequestDto
                //{
                //    ReferenceID = Guid.NewGuid().ToString(),
                //    ToAccount = model.Nuban,
                //    DestinationBankCode = model.BankCode,
                //    RequestType = _appSettings.nameEnquiryRequestType,
                //};

                //var ibsRequest = await _iBSReposervice.InitiateNameEnquiry(nibsRequestModel); ;


                var getUserInfo = await _context.ClientAuthentication
                    .Include(x => x.MerchantBankInfo)
                    .Include(x => x.MerchantWallet)
                    .Include(x => x.MerchantBusinessInfo)
                    .SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);


                if (getUserInfo.MerchantBusinessInfo.Count == 0 && getUserInfo.HasRegisteredCompany)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.MerchantBusinessInfoRequired, Message = ResponseMessage.BusinessInfoRequired };

                if (getUserInfo.MerchantBankInfo.Count > 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.MerchantInfoAlreadyExist, Message = ResponseMessage.DuplicateRecord };

                var bankInfoModel = new MerchantBankInfo
                {
                    ClientAuthenticationId = clientId,
                    Currency = model.Currency,
                    BankName = model.BankName,
                    BVN = getUserInfo.Bvn,
                    Country = model.Country,
                    Nuban = model.Nuban,
                    DefaultAccount = model.DefaultAccount,
                    BankCode = model.BankCode
                };

                ////var validateUser = await _bankServiceRepository.BvnValidation(model.BVN, getUserInfo.MerchantWallet.Select(x=>x.DoB).FirstOrDefault());

                ////if (validateUser.ResponseCode != AppResponseCodes.Success)
                ////    return new WebApiResponse { ResponseCode = validateUser.ResponseCode };

                if (model.BankCode == _appSettings.SterlingBankCode)
                {
                    _log4net.Info("Initiating OnboardMerchantBankInfo intrabank request" + " | " + model.BankCode + " | " + model.BankName + " | " + getUserInfo.Bvn + " | " + DateTime.Now);

                    var result = await _bankServiceRepository.GetAccountFullInfoAsync(model.Nuban, getUserInfo.Bvn);

                    if (result.ResponseCode != AppResponseCodes.Success)
                        return new WebApiResponse { ResponseCode = result.ResponseCode, Data = result.NUBAN, Message = ResponseMessage.BvnValidation };

                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            bankInfoModel.AccountName = result.CUS_SHO_NAME;
                            bankInfoModel.BranchCode = result.BRA_CODE;
                            bankInfoModel.LedCode = result.T24_BRA_CODE;
                            bankInfoModel.CusNum = result.CUS_NUM;

                            await _context.MerchantBankInfo.AddAsync(bankInfoModel);
                            await _context.SaveChangesAsync();

                            getUserInfo.StatusCode = MerchantOnboardingProcess.BankInfo;
                            getUserInfo.LastDateModified = DateTime.Now;

                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();

                            _log4net.Info("Initiating OnboardMerchantBankInfo intrabank request was successful" + " | " + model.BankCode + " | " + model.BankName + " | " + DateTime.Now);

                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = ResponseMessage.Success, UserStatus = MerchantOnboardingProcess.BankInfo };
                        }
                        catch (Exception)
                        {
                            await transaction.RollbackAsync();
                            return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ResponseMessage.InternalError };
                        }

                    }

                }

                _log4net.Info("Initiating OnboardMerchantBankInfo intrabank request" + " | " + model.BankCode + " | " + model.BankName + " | " + DateTime.Now);

                var nibsRequestModel = new IBSNameEnquiryRequestDto
                {
                    ReferenceID = Guid.NewGuid().ToString(),
                    ToAccount = model.Nuban,
                    DestinationBankCode = model.BankCode,
                    RequestType = _appSettings.nameEnquiryRequestType,
                };

                var ibsRequest = await _iBSReposervice.InitiateNameEnquiry(nibsRequestModel);

                if (ibsRequest.ResponseCode != AppResponseCodes.Success)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InterBankNameEnquiryFailed, Message = ResponseMessage.InterBankNameEnquiryFailed };

                if (ibsRequest.BVN != getUserInfo.Bvn)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidBVN, Message = ResponseMessage.BvnValidation };

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        bankInfoModel.AccountName = ibsRequest.AccountName;
                        bankInfoModel.KycLevel = ibsRequest.KYCLevel;
                        bankInfoModel.BankCode = model.BankCode;

                        await _context.MerchantBankInfo.AddAsync(bankInfoModel);
                        await _context.SaveChangesAsync();

                        getUserInfo.StatusCode = MerchantOnboardingProcess.BankInfo;
                        getUserInfo.LastDateModified = DateTime.Now;

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _log4net.Info("Initiating OnboardMerchantBankInfo interbank request was successful" + " | " + model.BankCode + " | " + model.BankName + " | " + getUserInfo.Bvn + " | " + DateTime.Now);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = ResponseMessage.Success };
                    }
                    catch (Exception ex)
                    {
                        _log4net.Error("Error occured" + " | " + "OnboardMerchantBankInfo" + " | " + model.Nuban + " | " + model.BankName + " | " + ex + " | " + DateTime.Now);
                        await transaction.RollbackAsync();

                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ResponseMessage.InternalError };
                    }

                }
            }
            catch (Exception ex)
            {
                _log4net.Error("An error ocuured while OnboardMerchantBankInfo merchant info" + model.Nuban + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ResponseMessage.InternalError };
            }
        }

        public async Task<WebApiResponse> TransactionSetupRequest(MerchantActivitySetupRequestDto model, long clientId)
        {
            try
            {

                // clientId = 18;
                _log4net.Info("Initiating TransactionSetupRequest request" + " | " + model.ReceiveEmail + " | " + clientId + " | " + model.OutSideLagos + " | " + DateTime.Now);

                var getUserInfo = await _context.ClientAuthentication
                    .Include(x => x.MerchantWallet)
                    .Include(x => x.MerchantBankInfo)
                    .Include(x => x.MerchantActivitySetup).SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);
                if (getUserInfo.MerchantBankInfo.Count == 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.MerchantBankInfoRequired, Message = ResponseMessage.BusinessInfoRequired };

                if (getUserInfo.MerchantActivitySetup.Count > 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.MerchantInfoAlreadyExist, Message = ResponseMessage.DuplicateRecord };

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var accountSetupModel = new MerchantActivitySetup
                        {
                            ClientAuthenticationId = clientId,
                            WithinLagos = model.WithinLagos,
                            PayOrchargeMe = model.PayOrchargeMe,
                            ReceiveEmail = model.ReceiveEmail,
                            OutSideLagos = model.OutSideLagos,
                            OutSideNigeria = model.OutSideNigeria

                        };
                        var walletModel = new MerchantWalletRequestDto
                        {
                            CURRENCYCODE = _appSettings.currencyCode,
                            DOB = getUserInfo.MerchantWallet.Select(x => x.DoB).FirstOrDefault(),
                            firstname = getUserInfo.MerchantWallet.Select(x => x.Firstname).FirstOrDefault(),
                            lastname = getUserInfo.MerchantWallet.Select(x => x.Lastname).FirstOrDefault(),
                            Gender = getUserInfo.MerchantWallet.Select(x => x.Gender).FirstOrDefault(),
                            mobile = getUserInfo.PhoneNumber,
                        };

                        var walletResponse = new CreateWalletResponse { };
                        getUserInfo.StatusCode = MerchantOnboardingProcess.Wallet;

                        _context.ClientAuthentication.Update(getUserInfo);
                        await _context.SaveChangesAsync();
                        //_context.MerchantWallet.Update(walletInfo);
                        //await _context.SaveChangesAsync();
                        await _context.MerchantActivitySetup.AddAsync(accountSetupModel);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        var cacheKey = Convert.ToString(clientId);
                        string serializedCustomerList;
                        var userInfo = new UserInfoViewModel { };

                        var redisCustomerList = await _distributedCache.GetAsync(cacheKey);

                        if (redisCustomerList != null)
                        {
                            await _distributedCache.RemoveAsync(cacheKey);
                            userInfo.Email = getUserInfo.Email;
                            userInfo.StatusCode = getUserInfo.StatusCode;
                            serializedCustomerList = JsonConvert.SerializeObject(userInfo);
                            redisCustomerList = Encoding.UTF8.GetBytes(serializedCustomerList);

                            var options1 = new DistributedCacheEntryOptions()
                            .SetAbsoluteExpiration(DateTime.Now.AddMinutes(30))
                            .SetSlidingExpiration(TimeSpan.FromMinutes(15));

                            await _distributedCache.SetAsync(cacheKey, redisCustomerList, options1);

                            _log4net.Info("Initiating TransactionSetupRequest request saved" + " | " + model.ReceiveEmail + " | " + clientId + " | " + model.OutSideLagos + " | " + DateTime.Now);
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = ResponseMessage.Success, UserStatus = MerchantOnboardingProcess.Wallet };
                        }

                        await _distributedCache.RemoveAsync(cacheKey);
                        userInfo.Email = getUserInfo.Email;
                        userInfo.StatusCode = getUserInfo.StatusCode;
                        serializedCustomerList = JsonConvert.SerializeObject(userInfo);
                        redisCustomerList = Encoding.UTF8.GetBytes(serializedCustomerList);

                        var options = new DistributedCacheEntryOptions()
                        .SetAbsoluteExpiration(DateTime.Now.AddMinutes(30))
                        .SetSlidingExpiration(TimeSpan.FromMinutes(15));

                        await _distributedCache.SetAsync(cacheKey, redisCustomerList, options);

                        _log4net.Info("Initiating TransactionSetupRequest request saved" + " | " + model.ReceiveEmail + " | " + clientId + " | " + model.OutSideLagos + " | " + DateTime.Now);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = ResponseMessage.Success, UserStatus = MerchantOnboardingProcess.Wallet };
                        ////var createWallet = await _walletRepoService.CreateMerchantWallet(walletModel);
                        ////if(createWallet.response == AppResponseCodes.Success)
                        ////{
                        ////    var walletInfo = await _context.MerchantWallet.SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);
                        ////    walletResponse.ClientAuthenticationId = getUserInfo.ClientAuthenticationId;
                        ////    walletResponse.Message = createWallet.responsedata.ToString();
                        ////    await _context.CreateWalletResponse.AddAsync(walletResponse);
                        ////    await _context.SaveChangesAsync();

                        ////    getUserInfo.StatusCode = AppResponseCodes.Success;
                        ////    walletInfo.status = MerchantWalletProcess.Processed;
                        ////    walletInfo.LastDateModified = DateTime.Now;
                        ////    _context.ClientAuthentication.Update(getUserInfo);
                        ////    await _context.SaveChangesAsync();
                        ////    _context.MerchantWallet.Update(walletInfo);
                        ////    await _context.SaveChangesAsync();
                        ////    await _context.MerchantActivitySetup.AddAsync(accountSetupModel);
                        ////    await _context.SaveChangesAsync();
                        ////    await transaction.CommitAsync();
                        ////    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                        //// }
                        ////walletResponse.ClientAuthenticationId = getUserInfo.ClientAuthenticationId;
                        ////walletResponse.Message = createWallet.responsedata.ToString();
                        ////await _context.CreateWalletResponse.AddAsync(walletResponse);
                        ////await _context.SaveChangesAsync();
                        ////await transaction.CommitAsync();
                        // return new WebApiResponse { ResponseCode = AppResponseCodes.FailedCreatingWallet };  
                    }
                    catch (Exception ex)
                    {
                        _log4net.Error("Error occured" + " | " + model.ReceiveEmail + " | " + clientId + " | " + ex + " | " + DateTime.Now);
                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ResponseMessage.InternalError };
                    }
                }


            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + model.ReceiveEmail + " | " + clientId + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = ResponseMessage.InternalError };
            }
        }

        public async Task<WebApiResponse> AddNewMerchantBankInfo(MerchantBankInfoRequestDto model, long clientId)
        {
            try
            {
                _log4net.Info("Initiating AddNewMerchantBankInfo request" + " | " + model.BankCode + " | " + model.BankName + " | " + clientId + " | " + DateTime.Now);

                if (await _context.OtherMerchantBankInfo.AnyAsync(x => x.Nuban == model.Nuban && x.ClientAuthenticationId == clientId))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateMerchantDetails };

                var getUserInfo = await _context.ClientAuthentication
                   .Include(x => x.MerchantBankInfo)
                   .Include(x => x.MerchantWallet)
                   .Include(x => x.MerchantBusinessInfo)
                   .SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);

                var bankInfoModel = new OtherMerchantBankInfo
                {
                    ClientAuthenticationId = clientId,
                    Currency = model.Currency,
                    BankName = model.BankName,
                    BVN = getUserInfo.Bvn,
                    Country = model.Country,
                    Nuban = model.Nuban,
                    DefaultAccount = model.DefaultAccount,
                    BankCode = model.BankCode
                };

                if (model.BankCode == _appSettings.SterlingBankCode)
                {
                    _log4net.Info("Initiating OnboardOtherMerchantBankInfo intrabank request" + " | " + model.BankCode + " | " + model.BankName + " | " + getUserInfo.Bvn + " | " + DateTime.Now);

                    var result = await _bankServiceRepository.GetAccountFullInfoAsync(model.Nuban, getUserInfo.Bvn);

                    if (result.ResponseCode != AppResponseCodes.Success)
                        return new WebApiResponse { ResponseCode = result.ResponseCode, Data = result.NUBAN };

                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            bankInfoModel.AccountName = result.CUS_SHO_NAME;
                            bankInfoModel.BranchCode = result.BRA_CODE;
                            bankInfoModel.LedCode = result.T24_BRA_CODE;
                            bankInfoModel.CusNum = result.CUS_NUM;

                            await _context.OtherMerchantBankInfo.AddAsync(bankInfoModel);
                            await _context.SaveChangesAsync();

                            getUserInfo.LastDateModified = DateTime.Now;

                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();

                            _log4net.Info("Initiating OnboardOtherMerchantBankInfo intrabank request was successful" + " | " + model.BankCode + " | " + model.BankName + " | " + DateTime.Now);

                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, UserStatus = MerchantOnboardingProcess.BankInfo };
                        }
                        catch (Exception ex)
                        {
                            _log4net.Error("An error ocuured while AddNewMerchantBankInfo merchant info" + clientId + " | " + ex + " | " + DateTime.Now);

                            await transaction.RollbackAsync();
                            return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                        }

                    }

                }

                _log4net.Info("Initiating OnboardMerchantBankInfo intrabank request" + " | " + model.BankCode + " | " + model.BankName + " | " + DateTime.Now);


                var nibsRequestModel = new IBSNameEnquiryRequestDto
                {
                    ReferenceID = Guid.NewGuid().ToString(),
                    ToAccount = model.Nuban,
                    DestinationBankCode = model.BankCode,
                    RequestType = _appSettings.nameEnquiryRequestType,
                };

                var ibsRequest = await _iBSReposervice.InitiateNameEnquiry(nibsRequestModel);

                if (ibsRequest.ResponseCode != AppResponseCodes.Success)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InterBankNameEnquiryFailed };

                if (ibsRequest.BVN != getUserInfo.Bvn)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidBVN };

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        bankInfoModel.AccountName = ibsRequest.AccountName;
                        bankInfoModel.KycLevel = ibsRequest.KYCLevel;
                        bankInfoModel.BankCode = model.BankCode;

                        await _context.OtherMerchantBankInfo.AddAsync(bankInfoModel);
                        await _context.SaveChangesAsync();

                        // getUserInfo.StatusCode = MerchantOnboardingProcess.BankInfo;
                        getUserInfo.LastDateModified = DateTime.Now;
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        _log4net.Info("Initiating OnboardMerchantBankInfo interbank request was successful" + " | " + model.BankCode + " | " + model.BankName + " | " + getUserInfo.Bvn + " | " + DateTime.Now);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        _log4net.Error("Error occured" + " | " + "OnboardMerchantBankInfo" + " | " + model.Nuban + " | " + model.BankName + " | " + ex + " | " + DateTime.Now);
                        await transaction.RollbackAsync();

                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                    }

                }

            }
            catch (Exception ex)
            {
                _log4net.Error("An error ocuured while AddNewMerchantBankInfo merchant info" + clientId + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> UpdateMerchantBankInfo(long clientId, long MerchantOtherBankInfoId)
        {
            try
            {

                if (!await _context.OtherMerchantBankInfo.AnyAsync(x => x.MerchantOtherBankInfoId == MerchantOtherBankInfoId && x.ClientAuthenticationId == clientId))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                if (!await _context.MerchantBankInfo.AnyAsync(x => x.ClientAuthenticationId == clientId))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.MerchantDefaultBankInfoNotFound };

                var getMerchantDefaultBankInfo = await _context.MerchantBankInfo
                .SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);

                var getMerchantOtherBank = await _context.OtherMerchantBankInfo
              .SingleOrDefaultAsync(x => x.MerchantOtherBankInfoId == MerchantOtherBankInfoId);

                getMerchantDefaultBankInfo.BankName = getMerchantOtherBank.BankName;
                getMerchantDefaultBankInfo.BankCode = getMerchantOtherBank.BankCode;
                getMerchantDefaultBankInfo.BranchCode = getMerchantOtherBank.BranchCode;
                getMerchantDefaultBankInfo.LedCode = getMerchantOtherBank.LedCode;
                getMerchantDefaultBankInfo.Nuban = getMerchantOtherBank.Nuban;
                getMerchantDefaultBankInfo.AccountName = getMerchantOtherBank.AccountName;
                getMerchantDefaultBankInfo.Currency = getMerchantOtherBank.Currency;
                getMerchantDefaultBankInfo.BVN = getMerchantOtherBank.BVN;
                getMerchantDefaultBankInfo.Country = getMerchantOtherBank.Country;
                getMerchantDefaultBankInfo.CusNum = getMerchantOtherBank.CusNum;
                getMerchantDefaultBankInfo.KycLevel = getMerchantOtherBank.KycLevel;
                getMerchantDefaultBankInfo.DefaultAccount = getMerchantOtherBank.DefaultAccount;
                getMerchantDefaultBankInfo.DateEntered = DateTime.Now;

                await _context.SaveChangesAsync();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _log4net.Error("An error ocuured while UpdateMerchantBankInfo merchant info" + clientId + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        //public async Task<WebApiResponse> UpdateMerchantPersonalInfo(long clientId, UpdateMerchantPersonalInfoRequestDto model)
        //{
        //    _log4net.Info("Initiating UpdateMerchantPersonalInfo request" + clientId + " | " + DateTime.Now);
        //    try
        //    {

        //        if (!await _context.ClientAuthentication.AnyAsync(x => x.ClientAuthenticationId == clientId))
        //            return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

        //        var MerchantPersonalInfo = await _context.ClientAuthentication
        //            .SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);

        //        if (!string.IsNullOrEmpty(model.PhoneNumber))
        //        {
        //            MerchantPersonalInfo.PhoneNumber = model.PhoneNumber;
        //        }
        //        if (!string.IsNullOrEmpty(model.Bvn))
        //        {
        //            MerchantPersonalInfo.Bvn = model.Bvn;
        //        }
        //        if (!string.IsNullOrEmpty(model.FullName))
        //        {
        //            MerchantPersonalInfo.FullName = model.FullName;
        //        }


        //        MerchantPersonalInfo.LastDateModified = DateTime.Now;
        //        await _context.SaveChangesAsync();

        //        return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
        //    }
        //}


        public async Task<WebApiResponse> UpdateMerchantBusinessInfo(long clientId, MerchantUpdateInfoRequestDto model)
        {
            //  clientId = 90;

            try
            {
                _log4net.Info("Initiating UpdateMerchantBusinessInfo request" + clientId + " | " + DateTime.Now);

                var getMerchant = await _context.MerchantBusinessInfo
                  .SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);

                if (getMerchant == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Data = "Record not found" };

                model.BusinessName = model.BusinessName == string.Empty ? getMerchant.BusinessName : getMerchant.BusinessName;

                //if (!string.IsNullOrEmpty(model.BusinessName))
                //{
                //    MerchantBankInfo.BusinessName = model.BusinessName;
                //}
                if (!string.IsNullOrEmpty(model.BusinessPhoneNumber))
                {
                    getMerchant.BusinessPhoneNumber = model.BusinessPhoneNumber;
                }
                if (!string.IsNullOrEmpty(model.BusinessEmail))
                {
                    getMerchant.BusinessEmail = model.BusinessEmail;
                }
                if (!string.IsNullOrEmpty(model.Tin))
                {
                    getMerchant.Tin = model.Tin;
                }
                if (!string.IsNullOrEmpty(model.Country))
                {
                    getMerchant.Country = model.Country;
                }
                if (!string.IsNullOrEmpty(model.Chargebackemail))
                {
                    getMerchant.Chargebackemail = model.Chargebackemail;
                }

                await _context.SaveChangesAsync();

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };

            }
            catch (Exception ex)
            {
                _log4net.Error("An error ocuured while getting merchant other business info" + clientId + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetOtherMerchantsBankInfo(long clientId)
        {
            try
            {
                var result = await _context.OtherMerchantBankInfo
                .Where(x => x.ClientAuthenticationId == clientId && x.Deleted == false)
                .ToListAsync();

                var OtherMerchantsBanksDTO = new List<MerchantResponseDTO>();

                var getMerchantDefaultBankInfo = await _context.MerchantBankInfo
                .SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);

                OtherMerchantsBanksDTO.Add(new MerchantResponseDTO
                {
                    MerchantOtherBankInfoId = 0,
                    BankName = getMerchantDefaultBankInfo.BankName,
                    BankCode = getMerchantDefaultBankInfo.BankCode,
                    AccountName = getMerchantDefaultBankInfo.AccountName,
                    Nuban = getMerchantDefaultBankInfo.Nuban,
                    DateEntered = getMerchantDefaultBankInfo.DateEntered,
                    DefaultAccount = getMerchantDefaultBankInfo.DefaultAccount
                });

                foreach (var bankDetails in result)
                {
                    OtherMerchantsBanksDTO.Add(new MerchantResponseDTO
                    {
                        MerchantOtherBankInfoId = bankDetails.MerchantOtherBankInfoId,
                        BankName = bankDetails.BankName,
                        BankCode = bankDetails.BankCode,
                        AccountName = bankDetails.AccountName,
                        Nuban = bankDetails.Nuban,
                        DateEntered = bankDetails.DateEntered,
                        DefaultAccount = false
                    });
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = OtherMerchantsBanksDTO };
            }
            catch (Exception ex)
            {
                _log4net.Error("An error ocuured while getting merchant other business info" + clientId + " | " + ex + " | " + DateTime.Now);



                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
        public async Task<WebApiResponse> InitiateEnquiry()
        {
            try
            {
                _log4net.Info("Initiating name enquiry request" + " | " + DateTime.Now);


                var result = await _iBSReposervice.InitiateNameEnquiryTestService();
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetListOfBanks" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetListOfBanks()
        {
            try
            {
                _log4net.Info("Initiating GetListOfBanks request" + " | " + DateTime.Now);

                var nibsRequestModel = new IBSGetBanksRequestDto
                {
                    ReferenceID = Guid.NewGuid().ToString(),
                    RequestType = _appSettings.getBanksRequestType
                };
                // var result = await _iBSReposervice.GetParticipatingBanks(nibsRequestModel);
                var result = await _iBSReposervice.GetBanks();
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetListOfBanks" + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> ResendSignupEmailAsync(string email)
        {
            try
            {
                var validateRequest = await _context.ClientAuthentication
                    .SingleOrDefaultAsync(x => x.Email == email && x.StatusCode == MerchantOnboardingProcess.CreateAccount);

                if (validateRequest == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                var token = DateTime.Now.ToString() + Guid.NewGuid().ToString() + DateTime.Now.AddMilliseconds(120) + Utilities.GeneratePin();
                var encryptedToken = token.Encrypt(_appSettings.appKey);
                var newPin = Utilities.GeneratePin();// + DateTime.Now.Day;
                var encryptedPin = newPin.Encrypt(_appSettings.appKey);

                if (await _context.PinRequest.AnyAsync(x => x.Pin == encryptedPin))
                {
                    newPin = string.Empty;
                    newPin = Utilities.GeneratePin();

                    encryptedPin = newPin.Encrypt(_appSettings.appKey);
                }

                var resetUrl = _appSettings.WebportalUrl + encryptedToken;

                string urlPath = "<a href=\"" + resetUrl + "\">Click to confirm your sign up process</a>";

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var validatepinrequest = await _context.PinRequest
                            .SingleOrDefaultAsync(x => x.ClientAuthenticationId == validateRequest.ClientAuthenticationId);

                        validatepinrequest.Pin = encryptedPin;
                        validatepinrequest.TokenSecret = encryptedToken;
                        validatepinrequest.LastDateModified = DateTime.Now;
                        _context.Update(validatepinrequest);

                        await _context.SaveChangesAsync();

                        var emailModal = new EmailRequestDto
                        {
                            Subject = "Merchant Signed Up",
                            DestinationEmail = email,
                            // DestinationEmail = "festypat9@gmail.com",
                            //  EmailBody = "Your onboarding was successfully created. Kindly use your email as username and" + "   " + "" + "   " + "as password to login"
                        };

                        var mailBuilder = new StringBuilder();
                        mailBuilder.AppendLine("Dear" + " " + email + "," + "<br />");
                        mailBuilder.AppendLine("<br />");
                        mailBuilder.AppendLine("You have successfully sign up. Please confirm your sign up by clicking the link below.<br />");
                        mailBuilder.AppendLine("Kindly use this token" + "  " + newPin + "  " + "and" + " " + urlPath + "<br />");
                        // mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                        mailBuilder.AppendLine("Best Regards,");
                        emailModal.EmailBody = mailBuilder.ToString();

                        var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

                        //var sendMail = await _sendGridEmailService.SendMail(mailBuilder.ToString(), emailModal.DestinationEmail, emailModal.Subject);

                        if (sendMail != AppResponseCodes.Success)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Data = "Request Failed" };

                        ////if (sendMail != AppResponseCodes.Success)
                        ////    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed };

                        await transaction.CommitAsync();

                        _log4net.Info("Initiating create merchant account was successful" + " | " + email + " | " + DateTime.Now);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        _log4net.Error("Error occured" + " | " + email + " | " + ex + " | " + DateTime.Now);
                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                    }
                }

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + email + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }

    }
}
