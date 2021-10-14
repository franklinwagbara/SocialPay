using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Messaging;
using SocialPay.Core.Services.AzureBlob;
using SocialPay.Core.Services.Bill;
using SocialPay.Core.Services.EventLogs;
using SocialPay.Core.Services.QrCode;
using SocialPay.Core.Services.Specta;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Cryptography;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.Store;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Store
{
    public class StoreBaseRepository
    {
        private readonly SocialPayDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly BlobService _blobService;
        private readonly NibbsQrBaseService _nibbsQrBaseService;
        private readonly PayWithSpectaService _payWithSpectaService;
        private readonly EncryptDecryptAlgorithm _encryptDecryptAlgorithm;
        private readonly TransactionReceipt _transactionReceipt;
        private readonly EmailService _emailService;
        private readonly EventLogService _eventLogService;
        private readonly UssdService _ussdService;
        private readonly StoreLogger _storeLogger;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(StoreBaseRepository));
        public IConfiguration Configuration { get; }
        public StoreBaseRepository(SocialPayDbContext context, IOptions<AppSettings> appSettings,
            IHostingEnvironment environment, BlobService blobService,
            NibbsQrBaseService nibbsQrBaseService, PayWithSpectaService payWithSpectaService,
            EncryptDecryptAlgorithm encryptDecryptAlgorithm,
            TransactionReceipt transactionReceipt, EmailService emailService,
            EventLogService eventLogService, IConfiguration configuration,
            UssdService ussdService, StoreLogger storeLogger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _appSettings = appSettings.Value;
            _hostingEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
            _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));
            _nibbsQrBaseService = nibbsQrBaseService ?? throw new ArgumentNullException(nameof(nibbsQrBaseService));
            _payWithSpectaService = payWithSpectaService ?? throw new ArgumentNullException(nameof(payWithSpectaService));
            _encryptDecryptAlgorithm = encryptDecryptAlgorithm ?? throw new ArgumentNullException(nameof(encryptDecryptAlgorithm));
            _transactionReceipt = transactionReceipt ?? throw new ArgumentNullException(nameof(transactionReceipt));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));
            Configuration = configuration;
            _ussdService = ussdService ?? throw new ArgumentNullException(nameof(ussdService));
            _storeLogger = storeLogger ?? throw new ArgumentNullException(nameof(storeLogger));
        }

        public async Task<WebApiResponse> CreateNewStore(StoreRequestDto request, UserDetailsViewModel userModel)
        {
            try
            {
                //userModel.ClientId = 90;

                _storeLogger.LogRequest($"{"Creating merchant store"}{" "}{userModel.Email}{" - "}{request.StoreName}{"  - "}", false);

                if (await _context.MerchantStore.AnyAsync(x => x.StoreName == request.StoreName && x.ClientAuthenticationId == userModel.ClientId))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateStoreName, Message = "Duplicate Store Name", StatusCode = ResponseCodes.Duplicate };

                if (await _context.MerchantStore.AnyAsync(x => x.StoreLink == request.StoreLink && x.ClientAuthenticationId == userModel.ClientId))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateLinkName, Message = "Duplicate Link Name", StatusCode = ResponseCodes.Duplicate };

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var options = Configuration.GetSection(nameof(AzureBlobConfiguration)).Get<AzureBlobConfiguration>();

                        var storeModel = new MerchantStore
                        {
                            Description = request.Description,
                            ClientAuthenticationId = userModel.ClientId,
                            StoreName = request.StoreName,
                            StoreLink = request.StoreLink
                        };

                        var reference = $"{userModel.ClientId}{"-"}{"ST-"}{Guid.NewGuid().ToString().Substring(18)}";

                        var blobRequest = new BlobStoreRequest
                        {
                            RequestType = "Store",
                            ClientId = userModel.ClientId,
                            Image = request.Image,
                            ImageGuidId = reference,
                            StoreName = request.StoreName,
                        };

                        blobRequest.FileLocation = $"{blobRequest.RequestType}/{Convert.ToString(blobRequest.ClientId)}/{request.StoreName}/{blobRequest.ImageGuidId}.jpg";

                        //storeModel.Image = newFileName;
                        storeModel.FileLocation = $"{options.blobBaseUrl}{options.containerName}{"/"}{blobRequest.FileLocation}";

                        await _context.MerchantStore.AddAsync(storeModel);
                        await _context.SaveChangesAsync();

                        var model = new MerchantPaymentSetup { };

                        model.PaymentLinkName = request.StoreLink == null ? string.Empty : request.StoreLink;
                        model.MerchantDescription = request.Description == null ? string.Empty : request.Description;
                        model.CustomUrl = request.StoreLink == null ? string.Empty : request.StoreLink;
                        model.ClientAuthenticationId = userModel.ClientId;
                        model.MerchantStoreId = storeModel.MerchantStoreId;
                        model.LinkCategory = MerchantLinkCategory.Store;

                        var newGuid = $"{"So-Pay-"}{Guid.NewGuid().ToString("N")}";

                        var token = model.MerchantAmount + "," + model.PaymentCategory + "," + model.PaymentLinkName + "," + newGuid;
                        var encryptedToken = token.Encrypt(_appSettings.appKey);

                        if (await _context.MerchantPaymentSetup.AnyAsync(x => x.CustomUrl == request.StoreName || x.PaymentLinkName == request.StoreName))
                            return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateLinkName, Data = "Duplicate link name", StatusCode = ResponseCodes.Duplicate };

                        if (await _context.MerchantPaymentSetup.AnyAsync(x => x.TransactionReference == newGuid))
                        {
                            newGuid = $"{"So-Pay-"}{Guid.NewGuid().ToString("N")}";
                            token = string.Empty;
                            encryptedToken = string.Empty;
                            token = $"{model.MerchantAmount}{model.PaymentCategory}{model.PaymentLinkName}{newGuid}";
                            encryptedToken = token.Encrypt(_appSettings.appKey);
                        }

                        model.TransactionReference = newGuid;

                        model.PaymentLinkUrl = $"{_appSettings.paymentstorelinkUrl}{model.CustomUrl}";

                        if (string.IsNullOrEmpty(model.CustomUrl))
                        {
                            model.PaymentLinkUrl = $"{_appSettings.paymentstorelinkUrl}{model.TransactionReference}";
                            model.CustomUrl = model.TransactionReference;
                        }

                        await _context.MerchantPaymentSetup.AddAsync(model);
                        await _context.SaveChangesAsync();

                        var linkCatModel = new LinkCategory
                        {
                            ClientAuthenticationId = userModel.ClientId,
                            Channel = "",
                            TransactionReference = newGuid
                        };

                        await _context.LinkCategory.AddAsync(linkCatModel);
                        await _context.SaveChangesAsync();

                        await _blobService.UploadStore(blobRequest);

                        await transaction.CommitAsync();

                        var eventLog = new EventRequestDto
                        {
                            ModuleAccessed = EventLogProcess.CreateStore,
                            Description = "Create new store",
                            UserId = userModel.Email,
                            ClientAuthenticationId = model.ClientAuthenticationId
                        };

                        await _eventLogService.ActivityRequestLog(eventLog);

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Store was successfully saved", StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, StatusCode = ResponseCodes.InternalError };
                    }
                }

                //return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _log4net.Error("An error occured while trying to create store" + " | " + request.StoreName + " | " + userModel.ClientId + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<InitiatePaymentResponse> InitiatePayment(CustomerStorePaymentRequestDto model)
        {
            _log4net.Info("Task starts to save store payments MakePayment info" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

            try
            {

                // var totalAmount1 = model.Items.Sum(x => x.TotalAmount * x.Quantity);

                long customerId = 0;
                string encryptedText = string.Empty;
                string encryptData = string.Empty;
                string paymentData = string.Empty;
                var paymentResponse = new CustomerResponseDto { };

                var paymentRef = $"{"SP-ST"}{Guid.NewGuid().ToString()}";

                var getLinkType = await _context.LinkCategory.SingleOrDefaultAsync(x => x.TransactionReference == model.TransactionReference);

                var getPaymentDetails = await _context.MerchantPaymentSetup.Include(x => x.CustomerTransaction)
                    .SingleOrDefaultAsync(x => x.TransactionReference == model.TransactionReference);

                if (getPaymentDetails == null)
                    return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.InvalidPaymentReference };

                var totalAmount = model.Items.Sum(x => x.TotalAmount * x.Quantity);

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {

                    try
                    {
                        var logCustomerInfo = new CustomerOtherPaymentsInfo
                        {
                            ClientAuthenticationId = getPaymentDetails.ClientAuthenticationId,
                            CustomerDescription = model.CustomerDescription,
                            MerchantPaymentSetupId = getPaymentDetails.MerchantPaymentSetupId,
                            Channel = model.Channel,
                            Email = model.Email,
                            PhoneNumber = model.PhoneNumber,
                            Fullname = model.Fullname,
                            TransactionReference = model.TransactionReference,
                            PaymentReference = paymentRef,
                            Category = CustomerPaymentCategory.StoreLink
                        };

                        var transactionLog = new StoreTransactionLog
                        {
                            ClientAuthenticationId = getPaymentDetails.ClientAuthenticationId,
                            PaymentReference = paymentRef,
                            TransactionStatus = StoreTransactionRequestProcess.InitiateRequest,
                            TotalAmount = totalAmount,
                            LastDateModified = DateTime.Now
                        };

                        await _context.StoreTransactionLog.AddAsync(transactionLog);
                        await _context.SaveChangesAsync();

                        var transactionLogDetails = new List<StoreTransactionLogDetails>();

                        foreach (var item in model.Items)
                        {
                            transactionLogDetails.Add(
                                 new StoreTransactionLogDetails
                                 {
                                     StoreTransactionLogId = transactionLog.StoreTransactionLogId,
                                     Color = item.Color,
                                     Size = item.Size,
                                     ProductId = item.ProductId,
                                     Quantity = item.Quantity,
                                     TotalAmount = item.TotalAmount,
                                     TransactionStatus = StoreTransactionRequestProcess.InitiateRequest,
                                 });
                        }

                        if (getPaymentDetails.PaymentCategory == MerchantPaymentLinkCategory.OneOffBasicLink)
                        {

                            logCustomerInfo.Amount = totalAmount;
                            logCustomerInfo.CustomerId = customerId;

                            await _context.CustomerOtherPaymentsInfo.AddAsync(logCustomerInfo);
                            await _context.SaveChangesAsync();

                            //await _context.StoreTransactionLog.AddAsync(transactionLog);
                            //await _context.SaveChangesAsync();

                            await _context.StoreTransactionLogDetails.AddRangeAsync(transactionLogDetails);
                            await _context.SaveChangesAsync();


                            _log4net.Info("save successfully" + " | " + model.TransactionReference + " | " + DateTime.Now);

                            //decimal CustomerTotalAmount = model.CustomerAmount;// + getPaymentDetails.ShippingFee;
                            if (getPaymentDetails.PaymentCategory == MerchantPaymentLinkCategory.OneOffBasicLink)
                            {
                                if (model.Channel == PaymentChannel.USSD)
                                {
                                    var requestModel = new GenerateReferenceDTO
                                    {
                                        amount = Convert.ToString(totalAmount)
                                    };

                                    var ussdRequest = await _ussdService.GeneratePaymentReference(requestModel, getPaymentDetails.ClientAuthenticationId, paymentRef);

                                    return new InitiatePaymentResponse { ResponseCode = ussdRequest.ResponseCode, Data = ussdRequest, PaymentRef = paymentRef, TransactionReference = model.TransactionReference };
                                }

                                if (model.Channel == PaymentChannel.NibbsQR)
                                {
                                    var qrRequest = new DynamicPaymentRequestDto
                                    {
                                        amount = Convert.ToString(totalAmount),
                                    };

                                    var initiateQrPayment = await _nibbsQrBaseService.DynamicPaymentAsync(qrRequest, getPaymentDetails.ClientAuthenticationId);

                                    await transaction.CommitAsync();

                                    return new InitiatePaymentResponse
                                    {
                                        ResponseCode = initiateQrPayment.ResponseCode,
                                        Data = initiateQrPayment,
                                        PaymentRef = paymentRef,
                                        TransactionReference = model.TransactionReference
                                    };
                                }

                                if (model.Channel == PaymentChannel.PayWithSpecta)
                                {
                                    _log4net.Info("Task starts to initiate pay with specta" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                                    var getMerchantId = await _context.MerchantBusinessInfo
                                     .SingleOrDefaultAsync(x => x.ClientAuthenticationId == getLinkType.ClientAuthenticationId);

                                    var generateToken = await _payWithSpectaService
                                        .InitiatePayment(totalAmount, "Social pay", paymentRef);

                                    if (generateToken.ResponseCode != AppResponseCodes.Success)
                                        return new InitiatePaymentResponse { ResponseCode = generateToken.ResponseCode };

                                    paymentResponse.CustomerId = customerId;
                                    paymentResponse.PaymentLink = Convert.ToString(generateToken.Data);

                                    _log4net.Info("MakePayment info was successful" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                                    await transaction.CommitAsync();

                                    return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse, PaymentRef = paymentRef, TransactionReference = model.TransactionReference };
                                }

                                encryptedText = $"{_appSettings.mid}{_appSettings.paymentCombination}{totalAmount}{_appSettings.paymentCombination}{paymentRef}";
                                encryptData = _encryptDecryptAlgorithm.EncryptAlt(encryptedText);
                                paymentData = $"{_appSettings.sterlingpaymentGatewayRequestUrl}{encryptData}";

                                paymentResponse.CustomerId = customerId;
                                paymentResponse.PaymentLink = paymentData;

                                _log4net.Info("MakePayment info was successful" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                                await transaction.CommitAsync();

                                return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse, PaymentRef = paymentRef, TransactionReference = model.TransactionReference };
                            }

                            if (model.Channel == PaymentChannel.PayWithSpecta)
                            {
                                var generateToken = await _payWithSpectaService.InitiatePayment(totalAmount, "Social pay", paymentRef);

                                if (generateToken.ResponseCode != AppResponseCodes.Success)
                                    return new InitiatePaymentResponse { ResponseCode = generateToken.ResponseCode };

                                paymentResponse.CustomerId = customerId;
                                paymentResponse.PaymentLink = Convert.ToString(generateToken.Data);

                                await transaction.CommitAsync();

                                return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse };
                            }

                            if (model.Channel == PaymentChannel.NibbsQR)
                            {
                                var qrRequest = new DynamicPaymentRequestDto
                                {
                                    amount = Convert.ToString(totalAmount),
                                };

                                var initiateQrPayment = await _nibbsQrBaseService.DynamicPaymentAsync(qrRequest, getPaymentDetails.ClientAuthenticationId);

                                await transaction.CommitAsync();

                                return new InitiatePaymentResponse
                                {
                                    ResponseCode = initiateQrPayment.ResponseCode,
                                    Data = initiateQrPayment,
                                    PaymentRef = paymentRef,
                                    TransactionReference = model.TransactionReference
                                };
                            }

                            if (model.Channel == PaymentChannel.USSD)
                            {
                                var requestModel = new GenerateReferenceDTO
                                {
                                    amount = Convert.ToString(totalAmount)
                                };

                                var ussdRequest = await _ussdService.GeneratePaymentReference(requestModel, getPaymentDetails.ClientAuthenticationId, paymentRef);

                                return new InitiatePaymentResponse { ResponseCode = ussdRequest.ResponseCode, Data = ussdRequest, PaymentRef = paymentRef, TransactionReference = model.TransactionReference };
                            }


                            encryptedText = $"{_appSettings.mid}{_appSettings.paymentCombination}{totalAmount}{_appSettings.paymentCombination}{paymentRef}";
                            encryptData = _encryptDecryptAlgorithm.EncryptAlt(encryptedText);
                            paymentData = $"{_appSettings.sterlingpaymentGatewayRequestUrl}{encryptData}";

                            paymentResponse.CustomerId = customerId;
                            paymentResponse.PaymentLink = paymentData;

                            await transaction.CommitAsync();

                            _log4net.Info("Initiate store payment was successful" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                            return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse, PaymentRef = paymentRef, TransactionReference = model.TransactionReference };
                            // }
                        }
                        if (model.Channel == PaymentChannel.PayWithSpecta && getPaymentDetails.PaymentCategory != MerchantPaymentLinkCategory.Escrow)
                        {
                            _log4net.Info("Task starts to initiate pay with specta" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                            logCustomerInfo.Amount = totalAmount;
                            await _context.CustomerOtherPaymentsInfo.AddAsync(logCustomerInfo);
                            await _context.SaveChangesAsync();

                            //await _context.StoreTransactionLog.AddAsync(transactionLog);
                            //await _context.SaveChangesAsync();

                            await _context.StoreTransactionLogDetails.AddRangeAsync(transactionLogDetails);
                            await _context.SaveChangesAsync();

                            var getMerchantId = await _context.MerchantBusinessInfo
                                   .SingleOrDefaultAsync(x => x.ClientAuthenticationId == getLinkType.ClientAuthenticationId);

                            var generateToken = await _payWithSpectaService
                                .InitiatePayment(totalAmount, "Social pay", paymentRef);

                            if (generateToken.ResponseCode != AppResponseCodes.Success)
                                return new InitiatePaymentResponse { ResponseCode = generateToken.ResponseCode };

                            paymentResponse.CustomerId = customerId;
                            paymentResponse.PaymentLink = Convert.ToString(generateToken.Data);

                            await transaction.CommitAsync();

                            _log4net.Info("MakePayment info was successful" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                            return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse, PaymentRef = paymentRef, TransactionReference = model.TransactionReference };

                        }

                        if (model.Channel == PaymentChannel.NibbsQR)
                        {
                            var qrRequest = new DynamicPaymentRequestDto
                            {
                                amount = Convert.ToString(totalAmount),
                            };

                            logCustomerInfo.Amount = totalAmount;
                            await _context.CustomerOtherPaymentsInfo.AddAsync(logCustomerInfo);
                            await _context.SaveChangesAsync();


                            await _context.StoreTransactionLogDetails.AddRangeAsync(transactionLogDetails);
                            await _context.SaveChangesAsync();

                            var initiateQrPayment = await _nibbsQrBaseService.DynamicPaymentAsync(qrRequest, getPaymentDetails.ClientAuthenticationId);

                            await transaction.CommitAsync();

                            return new InitiatePaymentResponse
                            {
                                ResponseCode = initiateQrPayment.ResponseCode,
                                Data = initiateQrPayment,
                                PaymentRef = paymentRef,
                                TransactionReference = model.TransactionReference
                            };

                        }

                        if (model.Channel == PaymentChannel.USSD)
                        {
                            var requestModel = new GenerateReferenceDTO
                            {
                                amount = Convert.ToString(totalAmount)
                            };

                            var ussdRequest = await _ussdService.GeneratePaymentReference(requestModel, getPaymentDetails.ClientAuthenticationId, paymentRef);

                            return new InitiatePaymentResponse { ResponseCode = ussdRequest.ResponseCode, Data = ussdRequest, PaymentRef = paymentRef, TransactionReference = model.TransactionReference };
                        }


                        logCustomerInfo.Amount = totalAmount;

                        await _context.CustomerOtherPaymentsInfo.AddAsync(logCustomerInfo);
                        await _context.SaveChangesAsync();

                        //await _context.StoreTransactionLog.AddAsync(transactionLog);
                        //await _context.SaveChangesAsync();

                        await _context.StoreTransactionLogDetails.AddRangeAsync(transactionLogDetails);
                        await _context.SaveChangesAsync();

                        encryptedText = $"{_appSettings.mid}{_appSettings.paymentCombination}{totalAmount}{_appSettings.paymentCombination}{paymentRef}";
                        encryptData = _encryptDecryptAlgorithm.EncryptAlt(encryptedText);
                        paymentData = $"{_appSettings.sterlingpaymentGatewayRequestUrl}{encryptData}";

                        paymentResponse.CustomerId = customerId;
                        paymentResponse.PaymentLink = paymentData;

                        await transaction.CommitAsync();
                        _log4net.Info("MakePayment info was successful" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                        return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse, PaymentRef = paymentRef, TransactionReference = model.TransactionReference };
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.InternalError };
                    }

                }

            }
            catch (Exception ex)
            {
                _log4net.Error("An error occured while trying to initiate payment MakePayment" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + ex + " | " + DateTime.Now);
                return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> PaymentConfirmation(PaymentValidationRequestDto model)
        {
            try
            {

                var otherPaymentReference = string.Empty;

                _log4net.Info("PaymentConfirmation request" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + model.Message + " | " + model.Channel + " - " + DateTime.Now);

                var logResponse = new PaymentResponse
                {
                    Message = model.Message,
                    PaymentReference = model.PaymentReference,
                    TransactionReference = model.TransactionReference
                };

                await _context.PaymentResponse.AddAsync(logResponse);
                await _context.SaveChangesAsync();

                var validateLinkType = await _context.LinkCategory
                    .SingleOrDefaultAsync(x => x.TransactionReference == model.TransactionReference);

                if (model.Channel == PaymentChannel.Card || model.Channel == PaymentChannel.OneBank || model.Channel == PaymentChannel.PayWithSpecta)
                {
                    if (model.Channel == PaymentChannel.Card)
                    {
                        var decodeMessage = Uri.UnescapeDataString(model.Message);

                        if (decodeMessage.Contains(" "))
                            decodeMessage = decodeMessage.Replace(" ", "+");

                        var decryptResponse = StringExtensions.DecryptAlt(decodeMessage);

                        model.Message = decryptResponse;

                        var newreference = model.Message.Split("^");

                        foreach (var item in newreference)
                        {
                            if (item.Contains("SBP") || item.Contains("sbp"))
                            {
                                otherPaymentReference = item;
                            }
                        }

                        _log4net.Info("PaymentConfirmation response was successful" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + DateTime.Now);

                        return await LogPaymentResponse(model, otherPaymentReference, validateLinkType.Channel);

                        // return result;
                    }

                    if (model.Channel == PaymentChannel.PayWithSpecta)
                    {
                        var result = await _payWithSpectaService.PaymentVerification(model.Message);

                        if (result.ResponseCode != AppResponseCodes.Success)
                        {
                            _log4net.Info("PaymentConfirmation failed" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + DateTime.Now);

                            return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionFailed };
                        }

                        model.Message = $"{"success"}{result.Message}";

                        return await LogPaymentResponse(model, string.Empty, validateLinkType.Channel);
                    }

                    return await LogPaymentResponse(model, string.Empty, validateLinkType.Channel);
                    // _log4net.Info("PaymentConfirmation response was successful" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + DateTime.Now);

                    // return oneBankRequest;

                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidPamentChannel };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "PaymentConfirmation" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> LogPaymentResponse(PaymentValidationRequestDto model, string reference, string chanel)
        {
            try
            {

                var validateDuplicateTransaction = await _context.TransactionLog
                    .SingleOrDefaultAsync(x => x.Message == model.Message);

                if (validateDuplicateTransaction != null)
                {
                    _log4net.Info("LogPaymentResponse" + " - " + model.PaymentReference + " - " + "validate Duplicate Transaction. DuplicateTransaction" + " - " + DateTime.Now);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateTransaction };
                }

                var logconfirmation = new TransactionLog { };

                var paymentSetupInfo = await _context.MerchantPaymentSetup
               .SingleOrDefaultAsync(x => x.TransactionReference == model.TransactionReference);

                if (paymentSetupInfo == null)
                {
                    _log4net.Info("LogPaymentResponse" + " - " + model.PaymentReference + " - " + "paymentSetupInfo. RecordNotFound" + " - " + DateTime.Now);
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };
                }

                var merchantInfo = await _context.ClientAuthentication.Include(x => x.MerchantBusinessInfo)
                    .SingleOrDefaultAsync(x => x.ClientAuthenticationId == paymentSetupInfo.ClientAuthenticationId);

                var getCustomerInfo = await _context.CustomerOtherPaymentsInfo
                    .SingleOrDefaultAsync(x => x.PaymentReference == model.PaymentReference);

                var getStoreRequest = await _context.StoreTransactionLog
                    .SingleOrDefaultAsync(x => x.PaymentReference == model.PaymentReference);

                /// var productInvestory = await _context.ProductInventory.SingleOrDefaultAsync(x=>x.ProductInventoryId)

                //for escrow if need to activate
                ////if (linkInfo != null && linkInfo.Channel == MerchantPaymentLinkCategory.Escrow || linkInfo.Channel == MerchantPaymentLinkCategory.OneOffEscrowLink)
                ////{
                ////    var customerInfo = await _context.ClientAuthentication
                ////    .SingleOrDefaultAsync(x => x.ClientAuthenticationId == model.CustomerId);
                ////    logconfirmation.Category = linkInfo.Channel;
                ////    logconfirmation.LinkCategory = paymentSetupInfo.PaymentCategory;
                ////    logconfirmation.PaymentChannel = model.Channel;
                ////    logconfirmation.ClientAuthenticationId = paymentSetupInfo.ClientAuthenticationId;
                ////    logconfirmation.CustomerInfo = model.CustomerId;
                ////    logconfirmation.CustomerEmail = customerInfo.Email;
                ////    logconfirmation.CustomerTransactionReference = Guid.NewGuid().ToString();
                ////    logconfirmation.TransactionReference = model.TransactionReference;
                ////    logconfirmation.OrderStatus = TransactionJourneyStatusCodes.Pending;
                ////    logconfirmation.Message = model.Message;
                ////    logconfirmation.LastDateModified = DateTime.Now;
                ////    logconfirmation.TotalAmount = getCustomerInfo.Amount;
                ////    logconfirmation.DeliveryDayTransferStatus = TransactionJourneyStatusCodes.Pending;
                ////    logconfirmation.PaymentReference = model.PaymentReference;
                ////    logconfirmation.TransactionStatus = TransactionJourneyStatusCodes.Pending;
                ////    logconfirmation.TransactionJourney = TransactionJourneyStatusCodes.Pending;
                ////    logconfirmation.ActivityStatus = TransactionJourneyStatusCodes.Pending;
                ////    logconfirmation.OtherPaymentReference = reference;

                ////    if (model.Message.Contains("approve") || model.Message.Contains("success") || model.Message.Contains("Approve"))
                ////    {
                ////        logconfirmation.Status = true;
                ////        logconfirmation.LastDateModified = DateTime.Now;

                ////        using (var transaction = await _context.Database.BeginTransactionAsync())
                ////        {
                ////            try
                ////            {
                ////                logconfirmation.DeliveryDate = DateTime.Now.AddDays(paymentSetupInfo.DeliveryTime);
                ////                logconfirmation.DeliveryFinalDate = logconfirmation.DeliveryDate.AddDays(2);
                ////                await _context.TransactionLog.AddAsync(logconfirmation);
                ////                await _context.SaveChangesAsync();
                ////                await transaction.CommitAsync();
                ////                //Send mail
                ////                await _transactionReceipt.ReceiptTemplate(logconfirmation.CustomerEmail, paymentSetupInfo.TotalAmount,
                ////                    logconfirmation.TransactionDate, model.TransactionReference, merchantInfo == null ? string.Empty : merchantInfo.BusinessName);

                ////                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                ////            }
                ////            catch (Exception ex)
                ////            {
                ////                await transaction.RollbackAsync();
                ////                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                ////            }
                ////        }
                ////    }

                ////    return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionFailed };
                ////}


                if (model.Message.Contains("approve") || model.Message.Contains("success") || model.Message.Contains("Approve"))
                {
                    logconfirmation.Category = chanel;
                    logconfirmation.LinkCategory = paymentSetupInfo.PaymentCategory;
                    logconfirmation.PaymentChannel = model.Channel;
                    logconfirmation.ClientAuthenticationId = paymentSetupInfo.ClientAuthenticationId;
                    logconfirmation.CustomerInfo = model.CustomerId;
                    logconfirmation.CustomerTransactionReference = Guid.NewGuid().ToString();
                    logconfirmation.TransactionReference = model.TransactionReference;
                    logconfirmation.OrderStatus = TransactionJourneyStatusCodes.Pending;
                    logconfirmation.Message = model.Message;
                    logconfirmation.LastDateModified = DateTime.Now;
                    logconfirmation.TotalAmount = getCustomerInfo.Amount;
                    logconfirmation.DeliveryDayTransferStatus = TransactionJourneyStatusCodes.Pending;
                    logconfirmation.PaymentReference = model.PaymentReference;
                    logconfirmation.TransactionStatus = TransactionJourneyStatusCodes.Approved;
                    logconfirmation.TransactionJourney = TransactionJourneyStatusCodes.Approved;
                    logconfirmation.ActivityStatus = TransactionJourneyStatusCodes.Approved;
                    logconfirmation.TransactionType = TransactionType.StorePayment;
                    // logconfirmation.CustomerEmail = model.e;
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try
                        {
                            logconfirmation.Status = true;
                            logconfirmation.DeliveryDate = DateTime.Now.AddDays(paymentSetupInfo.DeliveryTime);
                            logconfirmation.DeliveryFinalDate = logconfirmation.DeliveryDate.AddDays(2);

                            await _context.TransactionLog.AddAsync(logconfirmation);
                            await _context.SaveChangesAsync();

                            getStoreRequest.TransactionStatus = StoreTransactionRequestProcess.ConfirmedTransaction;
                            getStoreRequest.LastDateModified = DateTime.Now;
                            _context.Update(getStoreRequest);
                            await _context.SaveChangesAsync();
                            //Send mail


                            await _transactionReceipt.ReceiptTemplate(logconfirmation.CustomerEmail, paymentSetupInfo.TotalAmount,
                             logconfirmation.TransactionDate, model.TransactionReference, merchantInfo == null ? string.Empty : merchantInfo.FullName);


                            var emailModal = new EmailRequestDto
                            {
                                Subject = $"{_appSettings.successfulTransactionEmailSubject}{"-"}{model.TransactionReference}{"-"}",
                                DestinationEmail = merchantInfo.MerchantBusinessInfo.Count() == 0 ? merchantInfo.MerchantBusinessInfo.Select(x => x.BusinessEmail).FirstOrDefault() : merchantInfo.Email,
                            };

                            var mailBuilder = new StringBuilder();

                            mailBuilder.AppendLine("Dear" + " " + merchantInfo.MerchantBusinessInfo.Select(x => x.BusinessEmail).FirstOrDefault() + "," + "<br />");
                            mailBuilder.AppendLine("<br />");
                            mailBuilder.AppendLine("Customer was able to make payment successfully. See details below.<br />");
                            mailBuilder.AppendLine("<br />");
                            mailBuilder.AppendLine("Customer Name:" + "  " + getCustomerInfo.Fullname + "<br />");
                            mailBuilder.AppendLine("<br />");
                            mailBuilder.AppendLine("Customer Phone number:" + "  " + getCustomerInfo.PhoneNumber + "<br />");
                            mailBuilder.AppendLine("<br />");
                            mailBuilder.AppendLine("Transaction Amount:" + "  " + logconfirmation.TotalAmount + "<br />");
                            mailBuilder.AppendLine("<br />");
                            // mailBuilder.AppendLine("Best Regards,");
                            emailModal.EmailBody = mailBuilder.ToString();

                            //var sendMail = await _sendGridEmailService.SendMail(mailBuilder.ToString(), emailModal.DestinationEmail, emailModal.Subject);
                            await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

                            //if (sendMail != AppResponseCodes.Success)
                            //    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Data = "Request Failed" };

                            //  await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);


                            emailModal.DestinationEmail = getCustomerInfo.Email;
                            mailBuilder.AppendLine("Dear" + " " + getCustomerInfo.Fullname + "," + "<br />");
                            mailBuilder.AppendLine("<br />");
                            mailBuilder.AppendLine("Your payment was successful. See details below.<br />");
                            mailBuilder.AppendLine("<br />");
                            mailBuilder.AppendLine("Transaction Amount:" + "  " + logconfirmation.TotalAmount + "<br />");
                            mailBuilder.AppendLine("<br />");
                            // mailBuilder.AppendLine("Best Regards,");
                            emailModal.EmailBody = mailBuilder.ToString();

                            await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

                            //var sendCustomerMail = await _sendGridEmailService.SendMail(mailBuilder.ToString(), emailModal.DestinationEmail, emailModal.Subject);

                            //if (sendCustomerMail.ResponseCode != AppResponseCodes.Success)
                            //    return new WebApiResponse { ResponseCode = AppResponseCodes.Failed, Data = "Request Failed" };

                            _log4net.Info("Emails was successfully sent" + " | " + "LogPaymentResponse" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + DateTime.Now);

                            await transaction.CommitAsync();

                            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Request Successful" };
                        }
                        catch (Exception ex)
                        {
                            _log4net.Error("Database Error occured" + " | " + "LogPaymentResponse" + " | " + model.PaymentReference + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                            await transaction.RollbackAsync();

                            return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                        }
                    }
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    _log4net.Info("Transaction failed" + " | " + "LogPaymentResponse" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + model.Message + " - " + DateTime.Now);

                    try
                    {
                        var logFailedResponse = new FailedTransactions();

                        //logFailedResponse.CustomerTransactionReference = model.CustomerTransactionReference;
                        logFailedResponse.TransactionReference = model.TransactionReference;
                        logFailedResponse.Message = model.Message;
                        await _context.FailedTransactions.AddAsync(logFailedResponse);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        if (model.Message.Contains("Incorrect PIN"))
                        {
                            _log4net.Info("Transaction failed" + " | " + "Incorrect PIN" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + model.Message + " - " + DateTime.Now);

                            return new WebApiResponse { ResponseCode = AppResponseCodes.IncorrectTransactionPin, Data = "Incorrect Transaction Pin" };
                        }

                        if (model.Message.Contains("Insufficient"))
                        {
                            _log4net.Info("Transaction failed" + " | " + "Insufficient" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + model.Message + " - " + DateTime.Now);
                            return new WebApiResponse { ResponseCode = AppResponseCodes.InsufficientFunds, Data = "Insufficient Funds" };
                        }

                        return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionFailed };
                    }
                    catch (Exception ex)
                    {
                        _log4net.Error("Error occured" + " | " + "LogPaymentResponse" + " | " + model.PaymentReference + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                    }
                }

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "LogPaymentResponse" + " | " + model.PaymentReference + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
