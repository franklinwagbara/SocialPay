using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Services.AzureBlob;
using SocialPay.Core.Services.QrCode;
using SocialPay.Core.Services.Specta;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Cryptography;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
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
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(StoreBaseRepository));

        public StoreBaseRepository(SocialPayDbContext context, IOptions<AppSettings> appSettings,
            IHostingEnvironment environment, BlobService blobService, 
            NibbsQrBaseService nibbsQrBaseService, PayWithSpectaService payWithSpectaService,
            EncryptDecryptAlgorithm encryptDecryptAlgorithm)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _appSettings = appSettings.Value;
            _hostingEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
            _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));
            _nibbsQrBaseService = nibbsQrBaseService ?? throw new ArgumentNullException(nameof(nibbsQrBaseService));
            _payWithSpectaService = payWithSpectaService ?? throw new ArgumentNullException(nameof(payWithSpectaService));
            _encryptDecryptAlgorithm = encryptDecryptAlgorithm ?? throw new ArgumentNullException(nameof(encryptDecryptAlgorithm));
        }

        public async Task<WebApiResponse> CreateNewStore(StoreRequestDto request, UserDetailsViewModel userModel)
        {
            try
            {
                //userModel.ClientId = 90;

                if(await _context.MerchantStore.AnyAsync(x=>x.StoreName == request.StoreName && x.ClientAuthenticationId == userModel.ClientId))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateStoreName, Message = "Duplicate Store Name" };

                if (await _context.MerchantStore.AnyAsync(x => x.StoreLink == request.StoreLink && x.ClientAuthenticationId == userModel.ClientId))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateLinkName, Message = "Duplicate Link Name" };

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {

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
                        storeModel.FileLocation = blobRequest.FileLocation;

                        await _context.MerchantStore.AddAsync(storeModel);
                        await _context.SaveChangesAsync();

                        var model = new MerchantPaymentSetup { };

                        model.PaymentLinkName = request.StoreLink == null ? string.Empty : request.StoreLink;
                        model.MerchantDescription = request.Description == null ? string.Empty : request.Description;
                        model.CustomUrl = request.StoreName == null ? string.Empty : request.StoreName;
                       // model.RedirectAfterPayment = paymentModel.RedirectAfterPayment == false ? false : paymentModel.RedirectAfterPayment;
                       // model.DeliveryMethod = paymentModel.DeliveryMethod == null ? string.Empty : paymentModel.DeliveryMethod;
                        model.ClientAuthenticationId = userModel.ClientId;
                        model.MerchantStoreId = storeModel.MerchantStoreId;
                        model.LinkCategory = MerchantLinkCategory.Store;
                       // model.PaymentCategory = paymentModel.PaymentCategory == null ? string.Empty : paymentModel.PaymentCategory;
                       // model.ShippingFee = paymentModel.ShippingFee < 1 ? 0 : paymentModel.ShippingFee;
                      //  model.TotalAmount = model.MerchantAmount + model.ShippingFee;
                       // model.DeliveryTime = paymentModel.DeliveryTime < 1 ? 0 : paymentModel.DeliveryTime;
                        //model.PaymentMethod = paymentModel.PaymentMethod == null ? string.Empty : paymentModel.PaymentMethod;

                        var newGuid = $"{"So-Pay-"}{Guid.NewGuid().ToString("N")}";

                        var token = model.MerchantAmount + "," + model.PaymentCategory + "," + model.PaymentLinkName + "," + newGuid;
                        var encryptedToken = token.Encrypt(_appSettings.appKey);

                        if (await _context.MerchantPaymentSetup.AnyAsync(x => x.CustomUrl == request.StoreName || x.PaymentLinkName == request.StoreName))
                            return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateLinkName, Data = "Duplicate link name" };

                        if (await _context.MerchantPaymentSetup.AnyAsync(x => x.TransactionReference == newGuid))
                        {
                            newGuid = $"{"So-Pay-"}{Guid.NewGuid().ToString("N")}";
                            token = string.Empty;
                            encryptedToken = string.Empty;
                            token = $"{model.MerchantAmount}{model.PaymentCategory}{model.PaymentLinkName}{newGuid}";
                            encryptedToken = token.Encrypt(_appSettings.appKey);
                        }

                        model.TransactionReference = newGuid;

                        model.PaymentLinkUrl = $"{_appSettings.paymentlinkUrl}{model.CustomUrl}";

                        if (string.IsNullOrEmpty(model.CustomUrl))
                        {
                            model.PaymentLinkUrl = $"{_appSettings.paymentlinkUrl}{model.TransactionReference}";
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
                        //request.Image.CopyTo(new FileStream(filePath, FileMode.Create));

                        await transaction.CommitAsync();

                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                    }
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<MerchantPaymentSetup> GetTransactionReference(string refId)
        {
            return await _context.MerchantPaymentSetup
                .Include(x => x.CustomerTransaction)
                .SingleOrDefaultAsync(p => p.TransactionReference
              == refId
            );
        }
        public async Task<InitiatePaymentResponse> InitiatePayment(CustomerStorePaymentRequestDto model)
        {
            _log4net.Info("Task starts to save store payments MakePayment info" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

            try
            {
                //model.TransactionReference = "So-Pay-7e33d9c740da465faafad4b0dbf67fe1";

                long customerId = 0;
                string encryptedText = string.Empty;
                string encryptData = string.Empty;
                string paymentData = string.Empty;
                var paymentResponse = new CustomerResponseDto { };

                var paymentRef = $"{"SP-ST"}{Guid.NewGuid().ToString()}";

                var getLinkType = await _context.LinkCategory.SingleOrDefaultAsync(x => x.TransactionReference == model.TransactionReference);

                // var getPaymentDetails = await _customerService.GetTransactionReference(model.TransactionReference);

                var getPaymentDetails = await _context.MerchantPaymentSetup.Include(x => x.CustomerTransaction)
                    .SingleOrDefaultAsync(x => x.TransactionReference == model.TransactionReference);

                if (getPaymentDetails == null)
                    return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.InvalidPaymentReference };


                var totalAmount = model.Items.Sum(x => x.TotalAmount);

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
                    PaymentReference = paymentRef
                };

                ////if (getLinkType.Channel == MerchantPaymentLinkCategory.Escrow || getLinkType.Channel == MerchantPaymentLinkCategory.OneOffEscrowLink)
                ////{
                ////    var getClient = await _customerService.GetClientDetails(model.Email);

                ////    customerId = Convert.ToInt32(getClient.Data);
                ////    if (getClient.ResponseCode != AppResponseCodes.Success)
                ////    {
                ////        var newCustomerAccess = Guid.NewGuid().ToString("N").Substring(0, 10);
                ////        var createCustomer = await _customerService.CreateNewCustomer(model.Email, newCustomerAccess, model.Fullname,
                ////           model.PhoneNumber);

                ////        if (createCustomer.ResponseCode != AppResponseCodes.Success)
                ////            return new InitiatePaymentResponse { ResponseCode = createCustomer.ResponseCode };

                ////        customerId = Convert.ToInt32(createCustomer.Data);

                ////        var emailModal = new EmailRequestDto
                ////        {
                ////            Subject = "Guest Account Access",
                ////            SourceEmail = _appSettings.senderEmailInfo,
                ////            DestinationEmail = model.Email,
                ////            // DestinationEmail = "festypat9@gmail.com",
                ////        };

                ////        var mailBuilder = new StringBuilder();
                ////        mailBuilder.AppendLine("Dear" + " " + model.Email + "," + "<br />");
                ////        mailBuilder.AppendLine("<br />");
                ////        mailBuilder.AppendLine("You have successfully sign up as a Guest.<br />");
                ////        mailBuilder.AppendLine("Kindly use this token" + "  " + newCustomerAccess + "  " + "to login" + " " + "" + "<br />");
                ////        mailBuilder.AppendLine("Best Regards,");
                ////        emailModal.EmailBody = mailBuilder.ToString();

                ////        var sendMail = await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);
                ////    }
                ////}


                if (getPaymentDetails.PaymentCategory == MerchantPaymentLinkCategory.OneOffBasicLink)
                {
                   

                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        logCustomerInfo.Amount = model.CustomerAmount;
                        logCustomerInfo.CustomerId = customerId;

                        await _context.CustomerOtherPaymentsInfo.AddAsync(logCustomerInfo);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        _log4net.Info("save successfully" + " | " + model.TransactionReference + " | " + DateTime.Now);

                        //decimal CustomerTotalAmount = model.CustomerAmount;// + getPaymentDetails.ShippingFee;
                        if (getPaymentDetails.PaymentCategory == MerchantPaymentLinkCategory.OneOffBasicLink)
                        {
                            if (model.Channel == PaymentChannel.NibbsQR)
                            {
                                var qrRequest = new DynamicPaymentRequestDto
                                {
                                    amount = Convert.ToString(getPaymentDetails.TotalAmount),
                                };

                                var initiateQrPayment = await _nibbsQrBaseService.DynamicPaymentAsync(qrRequest, getPaymentDetails.ClientAuthenticationId);

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
                                    .InitiatePayment(logCustomerInfo.Amount, "Social pay", paymentRef);

                                if (generateToken.ResponseCode != AppResponseCodes.Success)
                                    return new InitiatePaymentResponse { ResponseCode = generateToken.ResponseCode };

                                paymentResponse.CustomerId = customerId;
                                paymentResponse.PaymentLink = Convert.ToString(generateToken.Data);

                                _log4net.Info("MakePayment info was successful" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                                return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse, PaymentRef = paymentRef, TransactionReference = model.TransactionReference };
                            }

                            encryptedText = $"{_appSettings.mid}{_appSettings.paymentCombination}{logCustomerInfo.Amount}{_appSettings.paymentCombination}{paymentRef}";
                            encryptData = _encryptDecryptAlgorithm.EncryptAlt(encryptedText);
                            paymentData = $"{_appSettings.sterlingpaymentGatewayRequestUrl}{encryptData}";
                            paymentResponse.CustomerId = customerId;
                            paymentResponse.PaymentLink = paymentData;

                            _log4net.Info("MakePayment info was successful" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                            return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse, PaymentRef = paymentRef, TransactionReference = model.TransactionReference };
                        }

                        if (model.Channel == PaymentChannel.PayWithSpecta)
                        {
                            var generateToken = await _payWithSpectaService.InitiatePayment(logCustomerInfo.Amount, "Social pay", paymentRef);
                            
                            if (generateToken.ResponseCode != AppResponseCodes.Success)
                                return new InitiatePaymentResponse { ResponseCode = generateToken.ResponseCode };
                           
                            paymentResponse.CustomerId = customerId;
                            paymentResponse.PaymentLink = Convert.ToString(generateToken.Data);

                            return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse };
                        }

                        if (model.Channel == PaymentChannel.NibbsQR)
                        {
                            var qrRequest = new DynamicPaymentRequestDto
                            {
                                amount = Convert.ToString(getPaymentDetails.TotalAmount),
                            };

                            var initiateQrPayment = await _nibbsQrBaseService.DynamicPaymentAsync(qrRequest, getPaymentDetails.ClientAuthenticationId);

                            return new InitiatePaymentResponse
                            {
                                ResponseCode = initiateQrPayment.ResponseCode,
                                Data = initiateQrPayment,
                                PaymentRef = paymentRef,
                                TransactionReference = model.TransactionReference
                            };
                        }

                        encryptedText = $"{_appSettings.mid}{_appSettings.paymentCombination}{logCustomerInfo.Amount}{_appSettings.paymentCombination}{paymentRef}";
                        encryptData = _encryptDecryptAlgorithm.EncryptAlt(encryptedText);
                        paymentData = $"{_appSettings.sterlingpaymentGatewayRequestUrl}{encryptData}";
                        paymentResponse.CustomerId = customerId; 
                        paymentResponse.PaymentLink = paymentData;

                        _log4net.Info("MakePayment info was successful" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                        return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse, PaymentRef = paymentRef, TransactionReference = model.TransactionReference };
                    }
                }
                if (model.Channel == PaymentChannel.PayWithSpecta && getPaymentDetails.PaymentCategory != MerchantPaymentLinkCategory.Escrow)
                {
                    _log4net.Info("Task starts to initiate pay with specta" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                    logCustomerInfo.Amount = getPaymentDetails.TotalAmount;
                    await _context.CustomerOtherPaymentsInfo.AddAsync(logCustomerInfo);
                    await _context.SaveChangesAsync();

                    var getMerchantId = await _context.MerchantBusinessInfo
                           .SingleOrDefaultAsync(x => x.ClientAuthenticationId == getLinkType.ClientAuthenticationId);

                    var generateToken = await _payWithSpectaService
                        .InitiatePayment(getPaymentDetails.TotalAmount, "Social pay", paymentRef);

                    if (generateToken.ResponseCode != AppResponseCodes.Success)
                    {
                        return new InitiatePaymentResponse { ResponseCode = generateToken.ResponseCode };
                    }

                    paymentResponse.CustomerId = customerId; paymentResponse.PaymentLink = Convert.ToString(generateToken.Data);

                    _log4net.Info("MakePayment info was successful" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                    return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse, PaymentRef = paymentRef, TransactionReference = model.TransactionReference };
                }

                if (model.Channel == PaymentChannel.NibbsQR)
                {
                    var qrRequest = new DynamicPaymentRequestDto
                    {
                        amount = Convert.ToString(getPaymentDetails.TotalAmount),
                    };

                    var initiateQrPayment = await _nibbsQrBaseService.DynamicPaymentAsync(qrRequest, getPaymentDetails.ClientAuthenticationId);

                    return new InitiatePaymentResponse
                    {
                        ResponseCode = initiateQrPayment.ResponseCode,
                        Data = initiateQrPayment,
                        PaymentRef = paymentRef,
                        TransactionReference = model.TransactionReference
                    };
                }

                logCustomerInfo.Amount = getPaymentDetails.TotalAmount;
                await _context.CustomerOtherPaymentsInfo.AddAsync(logCustomerInfo);
                await _context.SaveChangesAsync();

                encryptedText = $"{_appSettings.mid}{_appSettings.paymentCombination}{getPaymentDetails.TotalAmount}{_appSettings.paymentCombination}{paymentRef}";
                encryptData = _encryptDecryptAlgorithm.EncryptAlt(encryptedText);
                //var initiatepayment = Process.Start("cmd", "/C start " + _appSettings.sterlingpaymentGatewayRequestUrl + encryptData);
                paymentData = $"{_appSettings.sterlingpaymentGatewayRequestUrl}{encryptData}";
                paymentResponse.CustomerId = customerId; 
                paymentResponse.PaymentLink = paymentData;
                _log4net.Info("MakePayment info was successful" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse, PaymentRef = paymentRef, TransactionReference = model.TransactionReference };
            }
            catch (Exception ex)
            {
                _log4net.Error("An error occured while trying to initiate payment MakePayment" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
