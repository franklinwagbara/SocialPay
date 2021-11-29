using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Messaging;
using SocialPay.Core.Services.Account;
using SocialPay.Core.Services.AzureBlob;
using SocialPay.Core.Services.Validations;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Merchant
{
    public class CreateBulkMerchantService
    {
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(CreateBulkMerchantService));
        private readonly BlobService _blobService;
        private List<BulkSignUpRequestDto> requestDto = new List<BulkSignUpRequestDto>();
        private readonly SocialPayDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly EmailService _emailService;
        private readonly Utilities _utilities;
        //private readonly IHostingEnvironment hostingEnvironment;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly MerchantRegistrationService _merchantRegistrationService;
        private readonly BankServiceRepository _bankServiceRepository;
        public CreateBulkMerchantService(SocialPayDbContext context, IOptions<AppSettings> appSettings,
            EmailService emailService, Utilities utilities, MerchantRegistrationService merchantRegistrationService,
            BankServiceRepository bankServiceRepository,
            IWebHostEnvironment webHostEnvironment,
            BlobService blobService

            )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _utilities = utilities ?? throw new ArgumentNullException(nameof(utilities));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _bankServiceRepository = bankServiceRepository ?? throw new ArgumentNullException(nameof(bankServiceRepository));
            _merchantRegistrationService = merchantRegistrationService ?? throw new ArgumentNullException(nameof(merchantRegistrationService));
            _appSettings = appSettings.Value;
            _webHostEnvironment = webHostEnvironment;
            _blobService = blobService;
        }

        private async Task<string> GetReferCode()
        {
            try
            {
                Random generator = new Random();
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

        private async Task<WebApiResponse> ProcessCSVFile(IFormFile doc, string documentName)
        {
            try
            {
                if (doc == null || doc.Length == 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidCSVFormat, Message = "Invalid CSV" };

                string fileExtension = Path.GetExtension(doc.FileName);

                if (fileExtension != ".csv")
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidCSVFormat, Message = "Invalid CSV" };

                var rootFolder = _webHostEnvironment.WebRootPath;
                var fileName = doc.FileName;
                var filePath = Path.Combine(rootFolder, "CustomerDocuments");
                var ext = Path.GetExtension(doc.FileName);
                var filename = DateTime.Now.Ticks.ToString() + ext;
                var fullpath = Path.Combine(filePath, filename);

                var reference = $"uploadBoardingCSV{"-"}{"ST-"}{Guid.NewGuid().ToString().Substring(18)}";

                var blobRequest = new BlobOnboardingCSVRequest
                {
                    RequestType = "Documents",
                    Image = doc,
                    ImageGuidId = reference,
                    FileType = documentName,
                };

                blobRequest.FileLocation = $"{blobRequest.RequestType}/{blobRequest.FileType}/{blobRequest.ImageGuidId}.csv";

                return await _blobService.UploadCSV(blobRequest);

            }
            catch (Exception ex)
            {
                _log4net.Error("ProcessCSVFile" + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Internal error" };
            }
        }

        public async Task<WebApiResponse> BulkCreateMerchantBankInfo(IFormFile doc)
        {
            _log4net.Info("BulkCreateMerchantBankInfo" + " | " + DateTime.Now);
            try
            {
                var processCSV = await ProcessCSVFile(doc, "BulkCreateMerchantBankInfo");

                if (processCSV.ResponseCode != AppResponseCodes.Success)
                    return processCSV;

                string responseCode;
                List<BulkSignUpResponseDto> res = new List<BulkSignUpResponseDto>();

                using (var sreader = new StreamReader(doc.OpenReadStream()))
                {
                    string[] headers = sreader.ReadLine().Split(',');

                    while (!sreader.EndOfStream)
                    {

                        string[] rows = sreader.ReadLine().Split(',');
                        if (rows.Length < 1)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidCSVFormat };
                        try
                        {

                            if (await _context.ClientAuthentication.AnyAsync(x => x.Email == rows[0].ToString().Trim()))
                            {
                                var getUserInfo = await _context.ClientAuthentication
                                   .SingleOrDefaultAsync(x => x.Email == rows[0].ToString().Trim());


                                var model = new MerchantBankInfoRequestDto()
                                {
                                    BankName = rows[1].ToString().Trim(),
                                    BankCode = Regex.Replace(rows[2], @"[^0-9a-zA-Z\._]", string.Empty),
                                    Nuban = rows[3].ToString().Trim(),
                                    Currency = rows[4].ToString().Trim(),
                                    Country = rows[5].ToString().Trim(),
                                    DefaultAccount = true
                                };
                                var result = await _merchantRegistrationService.OnboardMerchantBankInfo(model, Convert.ToInt32(getUserInfo.ClientAuthenticationId));

                                if (result.ResponseCode == "00")
                                {
                                    string emailaddress = rows[0].ToString().Trim();

                                    var token = $"{DateTime.Now.ToString()}{Guid.NewGuid().ToString()}{DateTime.Now.AddMilliseconds(120)}{Utilities.GeneratePin()}";
                                    var encryptedToken = token.Encrypt(_appSettings.appKey);
                                    var newPin = Utilities.GeneratePin();// + DateTime.Now.Day;
                                    var encryptedPin = newPin.Encrypt(_appSettings.appKey);
                                    var resetUrl = _appSettings.WebportalUrl + encryptedToken;
                                    string urlPath = "<a href=\"" + resetUrl + "\">Click to confirm your sign up process</a>";


                                    var pinRequestModel = new PinRequest
                                    {
                                        ClientAuthenticationId = getUserInfo.ClientAuthenticationId,
                                        TokenSecret = encryptedToken,
                                        Status = false,
                                        LastDateModified = DateTime.Now,
                                        Pin = encryptedPin
                                    };

                                    await _context.PinRequest.AddAsync(pinRequestModel);
                                    await _context.SaveChangesAsync();



                                    string Password = emailaddress + "@socialpay";
                                    var emailModal = new EmailRequestDto
                                    {
                                        Subject = "Merchant Signed Up",
                                        DestinationEmail = emailaddress,
                                        // DestinationEmail = "festypat9@gmail.com",
                                        //  EmailBody = "Your onboarding was successfully created. Kindly use your email as username and" + "   " + "" + "   " + "as password to login"
                                    };
                                    //
                                    var mailBuilder = new StringBuilder();
                                    mailBuilder.AppendLine("Dear" + " " + rows[0].ToString().Trim() + "," + "<br />");
                                    mailBuilder.AppendLine("<br />");
                                    mailBuilder.AppendLine("You have successfully sign up and your password is " + Password + " .<br />");
                                    mailBuilder.AppendLine("<br />");
                                    mailBuilder.AppendLine("Please confirm your sign up by clicking the link below.<br />");
                                    mailBuilder.AppendLine("Kindly use this token" + "  " + newPin + "  " + "and" + " " + urlPath + "<br />");
                                    // mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                                    mailBuilder.AppendLine("Best Regards,");
                                    emailModal.EmailBody = mailBuilder.ToString();

                                    //var sendMail = await _sendGridEmailService.SendMail(mailBuilder.ToString(), emailModal.DestinationEmail, emailModal.Subject);

                                    var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

                                    if (sendMail != AppResponseCodes.Success)
                                        return new WebApiResponse { ResponseCode = AppResponseCodes.Failed };

                                }

                                responseCode = result.ResponseCode;
                            }
                            else
                            {
                                responseCode = AppResponseCodes.InvalidCSVFormat;
                            }

                        }
                        catch (Exception ex)
                        {
                            responseCode = AppResponseCodes.InvalidCSVFormat;
                        }
                        if (String.IsNullOrEmpty(rows[0].ToString().Trim())) continue;
                        res.Add(new BulkSignUpResponseDto
                        {
                            email = rows[0].ToString().Trim(),
                            ResponseCode = responseCode

                        });
                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = res };
            }
            catch (Exception ex)
            {
                _log4net.Error("BulkCreateMerchantBankInfo" + ex + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> BulkCreateMerchantBusinessInfo(IFormFile doc)
        {
            _log4net.Info("BulkCreateMerchantBusinessInfo" + " | " + DateTime.Now);
            try
            {
                var processCSV = await ProcessCSVFile(doc, "BulkCreateMerchantBusinessInfo");

                if (processCSV.ResponseCode != AppResponseCodes.Success)
                    return processCSV;

                string responseCode;

                var res = new List<BulkSignUpResponseDto>();

                using (var sreader = new StreamReader(doc.OpenReadStream()))
                {
                    string[] headers = sreader.ReadLine().Split(',');
                    while (!sreader.EndOfStream)
                    {

                        string[] rows = sreader.ReadLine().Split(',');
                        if (rows.Length < 1)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidCSVFormat };
                        try
                        {
                            responseCode = AppResponseCodes.RecordNotFound;

                            if (await _context.ClientAuthentication.AnyAsync(x => x.Email == rows[0].ToString()))
                            {
                                var getUserInfo = await _context.ClientAuthentication
                                .SingleOrDefaultAsync(x => x.Email == rows[0].ToString());

                                var businessInfoModel = new MerchantOnboardingInfoRequestDto
                                {
                                    BusinessEmail = rows[1].ToString().Trim(),
                                    BusinessName = rows[2].ToString().Trim(),
                                    BusinessPhoneNumber = rows[3].ToString().Trim(),
                                    Chargebackemail = rows[4].ToString().Trim(),
                                    Country = rows[5].ToString().Trim(),
                                    Tin = rows[6].ToString().Trim(),
                                    //Logo = newFileName,
                                    SpectaMerchantID = "",
                                    SpectaMerchantKey = "",
                                    SpectaMerchantKeyValue = ""
                                };

                                var result = await _merchantRegistrationService.OnboardMerchantBusinessInfo(businessInfoModel, Convert.ToInt32(getUserInfo.ClientAuthenticationId));

                                responseCode = result.ResponseCode;
                            }

                        }
                        catch (Exception ex)
                        {
                            responseCode = AppResponseCodes.InvalidCSVFormat;
                        }
                        if (String.IsNullOrEmpty(rows[0].ToString().Trim())) continue;
                        res.Add(new BulkSignUpResponseDto
                        {
                            email = rows[0].ToString().Trim(),
                            ResponseCode = responseCode
                        });
                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = res };
            }
            catch (Exception ex)
            {
                _log4net.Error("BulkCreateMerchantBusinessInfo" + ex.Message.ToString() + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> BulkCreateMerchant(IFormFile doc)
        {
            _log4net.Info("BulkCreateMerchant" + " | " + DateTime.Now);
            try
            {

                ////var validateUser = await _bankServiceRepository.BvnValidation("22156670025",
                ////   "1990-10-10", "",
                ////   "", "");

                string responseCode;
                if (doc == null || doc.Length == 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidCSVFormat };

                string fileExtension = Path.GetExtension(doc.FileName);

                if (fileExtension != ".csv")
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidCSVFormat };

                var rootFolder = _webHostEnvironment.WebRootPath;
                var fileName = doc.FileName;
                var filePath = Path.Combine(rootFolder, "CustomerDocuments");
                var ext = Path.GetExtension(doc.FileName);
                var filename = DateTime.Now.Ticks.ToString() + ext;
                var fullpath = Path.Combine(filePath, filename);

                var reference = $"uploadBoardingCSV{"-"}{"ST-"}{Guid.NewGuid().ToString().Substring(18)}";

                var blobRequest = new BlobOnboardingCSVRequest
                {
                    RequestType = "Documents",
                    Image = doc,
                    ImageGuidId = reference,
                    FileType = "CSVMerchantOnboarding",
                };

                blobRequest.FileLocation = $"{blobRequest.RequestType}/{blobRequest.FileType}/{blobRequest.ImageGuidId}.csv";

                //storeModel.Image = newFileName;
                await _blobService.UploadCSV(blobRequest);

                List<BulkSignUpResponseDto> res = new List<BulkSignUpResponseDto>();
                using (var sreader = new StreamReader(doc.OpenReadStream()))
                {
                    string[] headers = sreader.ReadLine().Split(',');

                    while (!sreader.EndOfStream)
                    {

                        string[] rows = sreader.ReadLine().Split(',');

                        if (rows.Length < 1)
                            return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidCSVFormat };
                        try
                        {
                            var item = new BulkSignUpRequestDto()
                            {
                                Email = rows[0].ToString().Trim(),
                                PhoneNumber = Regex.Replace(rows[1], @"[^0-9a-zA-Z\._]", string.Empty),
                                Bvn = rows[2].ToString().Trim(),
                                FirstName = rows[3].ToString().Trim(),
                                LastName = rows[4].ToString().Trim(),
                                DateOfBirth = rows[5].ToString().Trim(),
                                Gender = rows[6].ToString().Trim()

                            };

                            var response = await CreateNewMerchant(item);
                            responseCode = response.ResponseCode;
                        }
                        catch (Exception ex)
                        {
                            responseCode = AppResponseCodes.InvalidCSVFormat;
                        }
                        if (String.IsNullOrEmpty(rows[0].ToString().Trim())) continue;
                        res.Add(new BulkSignUpResponseDto
                        {
                            email = rows[0].ToString().Trim(),
                            phoneNumber = rows[1].ToString().Trim(),
                            ResponseCode = responseCode

                        });
                    }
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = res };
            }
            catch (Exception ex)

            {
                _log4net.Error("Create bulk merchant" + ex + " | " + DateTime.Now);
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
        private async Task<WebApiResponse> CreateNewMerchant(BulkSignUpRequestDto signUpRequestDto)
        {
            _log4net.Info("Initiating create merchant account" + " | " + signUpRequestDto.Email + " | " + DateTime.Now);

            try
            {
                if (await _context.ClientAuthentication.AnyAsync(x => x.Email == signUpRequestDto.Email
                || x.PhoneNumber == signUpRequestDto.PhoneNumber || x.Bvn == signUpRequestDto.Bvn))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateMerchantDetails };


                var validateUser = await _bankServiceRepository.BvnValidation(signUpRequestDto.Bvn,
                    signUpRequestDto.DateOfBirth, signUpRequestDto.FirstName,
                    signUpRequestDto.LastName, signUpRequestDto.Email);

                if (validateUser.ResponseCode != AppResponseCodes.Success)
                    return new WebApiResponse { ResponseCode = validateUser.ResponseCode };

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

                //string urlPath = "<a href=\"" + resetUrl + "\">Click to confirm your sign up process</a>";
                string Password = signUpRequestDto.Email + "@socialpay";
                _utilities.CreatePasswordHash(Password.Encrypt(_appSettings.appKey), out passwordHash, out passwordSalt);

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        string referCode = await GetReferCode();

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
                            ReferralCode = null,
                            ReferCode = $"{"SP-"}{referCode}"
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




                        //var pinRequestModel = new PinRequest
                        //{
                        //    ClientAuthenticationId = model.ClientAuthenticationId,
                        //    TokenSecret = encryptedToken,
                        //    Status = false,
                        //    LastDateModified = DateTime.Now,
                        //    Pin = encryptedPin
                        //};

                        //await _context.PinRequest.AddAsync(pinRequestModel);
                        //await _context.SaveChangesAsync();



                        //var emailModal = new EmailRequestDto
                        //{
                        //    Subject = "Merchant Signed Up",
                        //    DestinationEmail = signUpRequestDto.Email,
                        //    // DestinationEmail = "festypat9@gmail.com",
                        //    //  EmailBody = "Your onboarding was successfully created. Kindly use your email as username and" + "   " + "" + "   " + "as password to login"
                        //};
                        ////
                        //var mailBuilder = new StringBuilder();
                        //mailBuilder.AppendLine("Dear" + " " + signUpRequestDto.Email + "," + "<br />");
                        //mailBuilder.AppendLine("<br />");
                        //mailBuilder.AppendLine("You have successfully sign up and your password is " + Password + " .<br />");
                        //mailBuilder.AppendLine("<br />");
                        //mailBuilder.AppendLine("Please confirm your sign up by clicking the link below.<br />");
                        //mailBuilder.AppendLine("Kindly use this token" + "  " + newPin + "  " + "and" + " " + urlPath + "<br />");
                        //// mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                        //mailBuilder.AppendLine("Best Regards,");
                        //emailModal.EmailBody = mailBuilder.ToString();

                        ////var sendMail = await _sendGridEmailService.SendMail(mailBuilder.ToString(), emailModal.DestinationEmail, emailModal.Subject);

                        //var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

                        //if (sendMail != AppResponseCodes.Success)
                        //    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed };

                        await transaction.CommitAsync();

                        _log4net.Info("Initiating create merchant account was successful" + " | " + signUpRequestDto.Email + " | " + DateTime.Now);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        _log4net.Error("Error occured" + " | " + signUpRequestDto.Email + " | " + ex + " | " + DateTime.Now);
                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                    }
                }

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + signUpRequestDto.Email + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }

}
