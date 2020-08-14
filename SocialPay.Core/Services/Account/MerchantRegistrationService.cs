using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Messaging;
using SocialPay.Core.Services.IBS;
using SocialPay.Core.Services.Validations;
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
using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
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
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(MerchantRegistrationService));
        public MerchantRegistrationService(SocialPayDbContext context,
            IOptions<AppSettings> appSettings, EmailService emailService,
            Utilities utilities, IHostingEnvironment environment,
            BankServiceRepository bankServiceRepository,
            IBSReposervice iBSReposervice) : base(context)
        {
            _context = context;
            _appSettings = appSettings.Value;
            _emailService = emailService;
            _utilities = utilities;
            _hostingEnvironment = environment;
            _bankServiceRepository = bankServiceRepository;
            _iBSReposervice = iBSReposervice;
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
                var newPin = Utilities.GeneratePin();// + DateTime.Now.Day;
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
                          //  EmailBody = "Your onboarding was successfully created. Kindly use your email as username and" + "   " + "" + "   " + "as password to login"
                        };
                        var mailBuilder = new StringBuilder();
                        mailBuilder.AppendLine("Dear" + " " + signUpRequestDto.Email + "," + "<br />");
                        mailBuilder.AppendLine("<br />");
                        mailBuilder.AppendLine("You have successfully sign up. Please confirm your sign up by clicking the link below.<br />");
                        mailBuilder.AppendLine("Kindly use this token" + "  " + newPin + "  " + "and" + " " + urlPath + "<br />");
                       // mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                        mailBuilder.AppendLine("Best Regards,");
                        emailModal.EmailBody = mailBuilder.ToString();

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
                var token = model.Token.Trim().Replace("%", "+");

                var validateToken = await _context.PinRequest.SingleOrDefaultAsync(x => x.Pin == encryptPin
                && x.Status == false);

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
     

        public async Task<WebApiResponse> OnboardMerchantBusinessInfo(MerchantOnboardingInfoRequestDto model, long clientId)
        {
            try
            {
               

                if(await _context.MerchantBusinessInfo.AnyAsync(x=>x.BusinessEmail == model.BusinessEmail || 
                x.BusinessPhoneNumber == model.BusinessPhoneNumber || x.Chargebackemail == model.Chargebackemail))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateMerchantDetails };

                var getUserInfo = await _context.ClientAuthentication
                    .Include(x=>x.MerchantBusinessInfo).SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);
                if(getUserInfo.MerchantBusinessInfo.Count > 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.MerchantInfoAlreadyExist };
                string fileName = string.Empty;
                var merchantId = Guid.NewGuid().ToString("N").Substring(22);
                var newFileName = string.Empty;
                fileName = (model.Logo.FileName);

                var FileExtension = Path.GetExtension(fileName);
                fileName = Path.Combine(_hostingEnvironment.WebRootPath, "MerchantLogo") + $@"\{newFileName}";
                
                // concating  FileName + FileExtension
                newFileName = merchantId + FileExtension;
                var filePath = Path.Combine(fileName, newFileName);
                using(var transaction = await _context.Database.BeginTransactionAsync())
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
                            MerchantReferenceId = merchantId,
                            FileLocation = "MerchantLogo",
                            Logo = newFileName
                        };
                        await _context.MerchantBusinessInfo.AddAsync(businessInfoModel);
                        await _context.SaveChangesAsync();
                        getUserInfo.StatusCode = AppResponseCodes.BusinessInfo;
                        getUserInfo.LastDateModified = DateTime.Now;
                        await _context.SaveChangesAsync();
                        model.Logo.CopyTo(new FileStream(filePath, FileMode.Create));
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
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> OnboardMerchantBankInfo(MerchantBankInfoRequestDto model, long clientId)
        {
            try
            {
              

                if (await _context.MerchantBankInfo.AnyAsync(x => x.Nuban == model.Nuban ||
                 x.BVN == model.BVN))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateMerchantDetails };

                var getUserInfo = await _context.ClientAuthentication
                    .Include(x => x.MerchantBankInfo).Include(x=>x.MerchantBusinessInfo)
                    .SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);
                if (getUserInfo.MerchantBusinessInfo.Count == 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.MerchantBusinessInfoRequired };

                if (getUserInfo.MerchantBusinessInfo.Count > 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.MerchantInfoAlreadyExist };

                var bankInfoModel = new MerchantBankInfo
                {
                    ClientAuthenticationId = clientId,
                    Currency = model.Currency,
                    BankName = model.BankName,
                    BVN = model.BVN,
                    Country = model.Country,
                    Nuban = model.Nuban,
                    DefaultAccount = model.DefaultAccount,
                };

                if (model.BankCode == _appSettings.SterlingBankCode)
                {
                    var result = await _bankServiceRepository.GetAccountFullInfoAsync(model.Nuban);
                    if (result.ResponseCode != AppResponseCodes.Success)
                        return new WebApiResponse { ResponseCode = result.ResponseCode, Data = result.NUBAN };

                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            bankInfoModel.AccountName = result.CUS_SHO_NAME;
                            await _context.MerchantBankInfo.AddAsync(bankInfoModel);
                            await _context.SaveChangesAsync();
                            getUserInfo.StatusCode = AppResponseCodes.Success;
                            getUserInfo.LastDateModified = DateTime.Now;
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

                if(ibsRequest.BVN != model.BVN)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidBVN };
                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        bankInfoModel.AccountName = ibsRequest.AccountName;
                        await _context.MerchantBankInfo.AddAsync(bankInfoModel);
                        await _context.SaveChangesAsync();
                        getUserInfo.StatusCode = AppResponseCodes.Success;
                        getUserInfo.LastDateModified = DateTime.Now;
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
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> TransactionSetupRequest(MerchantActivitySetupRequestDto model, long clientId)
        {
            try
            {

                var getUserInfo = await _context.ClientAuthentication
                    .Include(x => x.MerchantActivitySetup).SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId);
                if (getUserInfo.MerchantBusinessInfo.Count > 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.MerchantInfoAlreadyExist };
            
                        var accountSetupModel = new MerchantActivitySetup
                        {
                            ClientAuthenticationId = clientId, DeliveryFees = model.DeliveryFees,
                            PayOrchargeMe = model.PayOrchargeMe, ReceiveEmail = model.ReceiveEmail
                            
                        };
                        await _context.MerchantActivitySetup.AddAsync(accountSetupModel);
                        await _context.SaveChangesAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                   
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetListOfBanks()
        {
            try
            {
                var nibsRequestModel = new IBSGetBanksRequestDto
                {
                    ReferenceID = Guid.NewGuid().ToString(),                   
                    RequestType = _appSettings.getBanksRequestType
                };
                var result = await _iBSReposervice.GetParticipatingBanks(nibsRequestModel);               
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
