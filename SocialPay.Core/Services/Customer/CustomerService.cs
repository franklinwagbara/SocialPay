﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Utilities;
using SocialPay.Core.Messaging;
using SocialPay.Core.Repositories.Customer;
using SocialPay.Core.Services.Specta;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Cryptography;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Customer
{
    public class CustomerRepoService
    {

        private readonly SocialPayDbContext _context;
        private readonly ICustomerService _customerService;
        private readonly AppSettings _appSettings;
        private readonly EmailService _emailService;
        private readonly EncryptDecryptAlgorithm _encryptDecryptAlgorithm;
        private readonly EncryptDecrypt _encryptDecrypt;
        private readonly PayWithSpectaService _payWithSpectaService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly TransactionReceipt _transactionReceipt;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(CustomerRepoService));
        public CustomerRepoService(ICustomerService customerService, IOptions<AppSettings> appSettings,
            EncryptDecryptAlgorithm encryptDecryptAlgorithm, EncryptDecrypt encryptDecrypt,
            EmailService emailService, IHostingEnvironment environment,
            SocialPayDbContext context, PayWithSpectaService payWithSpectaService,
            TransactionReceipt transactionReceipt)
        {
            _customerService = customerService;
            _appSettings = appSettings.Value;
            _encryptDecrypt = encryptDecrypt;
            _encryptDecryptAlgorithm = encryptDecryptAlgorithm;
            _emailService = emailService;
            _hostingEnvironment = environment;
            _context = context;
            _payWithSpectaService = payWithSpectaService;
            _transactionReceipt = transactionReceipt;
        }

        public async Task<WebApiResponse> GetLinkDetails(string transactionReference)
        {
            try
            {
                _log4net.Info("GetLinkDetails" + " | " + transactionReference + " | " +  DateTime.Now);

                //////var decodeUrl = System.Uri.UnescapeDataString(transactionReference);
                //////if (decodeUrl.Contains(" "))
                //////{
                //////    decodeUrl = decodeUrl.Replace(" ", "+");
                //////}
                //////var dd = DecryptAlt(decodeUrl);

                // var decryptedReference = transactionReference.Replace(" ", "+").Decrypt(_appSettings.appKey).Split(",")[3];
                //var res = t1.Decrypt(_appSettings.appKey).Split(",")[3];
                //var decryptedReference1 = transactionReference.Decrypt(_appSettings.appKey);
                //string dv = decryptedReference1.Split(",")[3];
                // var decryptedReference = transactionReference.Decrypt(_appSettings.appKey).Split(",")[3];

                var result = await _customerService.GetTransactionDetails(transactionReference);
                if (result == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidPaymentLink};

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetLinkDetails" + " | " + transactionReference + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetAllCustomerOrders(long clientId, string category)
        {
            try
            {
                _log4net.Info("GetAllCustomerOrders" + " | " + clientId + " | " +  category + " | " + DateTime.Now);

                //clientId = 40074;
                //category = "02";
                return await _customerService.GetCustomerOrders(clientId, category);
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "GetAllCustomerOrders" + " | " + clientId + " | " +  ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<InitiatePaymentResponse> MakePayment(CustomerPaymentRequestDto model)
        {
            _log4net.Info("Task starts to save payments MakePayment info" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | "+ DateTime.Now);

            try
            {
                long customerId = 0;
                string encryptedText = string.Empty;
                string encryptData = string.Empty;
                string paymentData = string.Empty;
                var paymentResponse = new CustomerResponseDto { };
                var paymentRef = $"{"SP-"}{Guid.NewGuid().ToString()}";
                var getLinkType = await _customerService.GetLinkCategorybyTranref(model.TransactionReference);

                if(getLinkType != null && getLinkType.Channel == MerchantPaymentLinkCategory.InvoiceLink)
                {
                    var getInvoiceInfo = await _customerService.GetInvoicePaymentAsync(model.TransactionReference);
                    if (getInvoiceInfo == null)                 
                        return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.InvalidPaymentReference };
                    var customerReference = Guid.NewGuid().ToString();

                    var invoicePayment = new InvoicePaymentInfo
                    {
                        TransactionReference = model.TransactionReference, Channel = model.Channel,
                        Email = model.Email, Fullname = model.Fullname, PhoneNumber = model.PhoneNumber,
                        InvoicePaymentLinkId = getInvoiceInfo.InvoicePaymentLinkId, LastDateModified = DateTime.Now,
                        CustomerTransactionReference = customerReference, PaymentReference = paymentRef
                    };
                    decimal CustomerTotalAmount = getInvoiceInfo.TotalAmount;

                    if (model.Channel == PaymentChannel.PayWithSpecta)
                    {
                        _log4net.Info("Task starts to initiate pay with specta" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                        var getMerchantId = await _context.MerchantBusinessInfo
                            .SingleOrDefaultAsync(x => x.ClientAuthenticationId == getLinkType.ClientAuthenticationId);

                        var generateToken = await _payWithSpectaService
                            .InitiatePayment(CustomerTotalAmount, "Social pay", model.TransactionReference,
                            getMerchantId.SpectaMerchantID, getMerchantId.SpectaMerchantKey);
                        
                        if (generateToken.ResponseCode != AppResponseCodes.Success)
                        {
                            return new InitiatePaymentResponse { ResponseCode = generateToken.ResponseCode };
                        }

                        paymentResponse.InvoiceReference = paymentRef; paymentResponse.PaymentLink = Convert.ToString(generateToken.Data);
                        await _context.InvoicePaymentInfo.AddAsync(invoicePayment);
                        await _context.SaveChangesAsync();

                        return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse, PaymentRef = paymentRef };
                    }

                    encryptedText =$"{_appSettings.mid}{_appSettings.paymentCombination}{CustomerTotalAmount}{_appSettings.paymentCombination}{paymentRef}";
                    encryptData = _encryptDecryptAlgorithm.EncryptAlt(encryptedText);
                    paymentData = _appSettings.sterlingpaymentGatewayRequestUrl + encryptData;
                    paymentResponse.InvoiceReference = paymentRef; 
                    paymentResponse.PaymentLink = paymentData;
                    await _context.InvoicePaymentInfo.AddAsync(invoicePayment);
                    await _context.SaveChangesAsync();

                    return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse, PaymentRef = paymentRef };                   
                }
               
                var getPaymentDetails = await _customerService.GetTransactionReference(model.TransactionReference);
               
                if (getPaymentDetails == null)
                    return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.InvalidPaymentReference };

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
                
               
                if(getPaymentDetails.PaymentCategory == MerchantPaymentLinkCategory.OneOffBasicLink)
                {
                    string fileName = string.Empty;
                    var newFileName = string.Empty;
                    var fileExtension = string.Empty;
                    var filePath = string.Empty;

                    if (model.Document != null)
                    {
                        fileName = (model.Document.FileName);
                        var documentId = Guid.NewGuid().ToString("N").Substring(22);
                        fileExtension = Path.GetExtension(fileName);
                        fileName = Path.Combine(_hostingEnvironment.WebRootPath, "CustomerDocuments") + $@"\{newFileName}";

                        // concating  FileName + FileExtension
                        newFileName = documentId + fileExtension;
                        filePath = Path.Combine(fileName, newFileName);
                    }
                   
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        logCustomerInfo.Amount = model.CustomerAmount;
                        logCustomerInfo.CustomerId = customerId;
                        logCustomerInfo.Document = newFileName;
                        logCustomerInfo.FileLocation = "CustomerDocuments";
                        await _context.CustomerOtherPaymentsInfo.AddAsync(logCustomerInfo);
                        await _context.SaveChangesAsync();
                        _log4net.Info("About to save uploaded document" + " | " + model.TransactionReference + " | " + DateTime.Now);

                        if (model.Document != null)
                        {
                            model.Document.CopyTo(new FileStream(filePath, FileMode.Create));
                            await transaction.CommitAsync();
                            _log4net.Info("Uploaded document was successfully saved" + " | " + model.TransactionReference + " | " + DateTime.Now);
                        }
                        else
                        {
                            await transaction.CommitAsync();
                            _log4net.Info("Uploaded document was successfully saved" + " | " + model.TransactionReference + " | " + DateTime.Now);
                        }
                        
                        //decimal CustomerTotalAmount = model.CustomerAmount;// + getPaymentDetails.ShippingFee;
                        if(getPaymentDetails.PaymentCategory == MerchantPaymentLinkCategory.OneOffBasicLink)
                        {
                            if (model.Channel == PaymentChannel.PayWithSpecta)
                            {
                                _log4net.Info("Task starts to initiate pay with specta" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                                var getMerchantId = await _context.MerchantBusinessInfo
                                 .SingleOrDefaultAsync(x => x.ClientAuthenticationId == getLinkType.ClientAuthenticationId);

                                var generateToken = await _payWithSpectaService
                                    .InitiatePayment(logCustomerInfo.Amount, "Social pay", paymentRef, 
                                    getMerchantId.SpectaMerchantID, getMerchantId.SpectaMerchantKey);
                               
                                if (generateToken.ResponseCode != AppResponseCodes.Success)
                                {
                                    return new InitiatePaymentResponse { ResponseCode = generateToken.ResponseCode };
                                }
                                
                                paymentResponse.CustomerId = customerId; paymentResponse.PaymentLink = Convert.ToString(generateToken.Data);
                                _log4net.Info("MakePayment info was successful" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                                return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse };
                            }

                            encryptedText = $"{_appSettings.mid}{_appSettings.paymentCombination}{logCustomerInfo.Amount}{_appSettings.paymentCombination}{paymentRef}";
                            encryptData = _encryptDecryptAlgorithm.EncryptAlt(encryptedText);
                            paymentData = _appSettings.sterlingpaymentGatewayRequestUrl + encryptData;
                            paymentResponse.CustomerId = customerId; paymentResponse.PaymentLink = paymentData;
                            _log4net.Info("MakePayment info was successful" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                            return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse, PaymentRef = paymentRef };
                        }

                        ////if (model.Channel == PaymentChannel.PayWithSpecta)
                        ////{
                        ////    var generateToken = await _payWithSpectaService.InitiatePayment(logCustomerInfo.Amount, "Social pay", paymentRef);
                        ////    if (generateToken.ResponseCode != AppResponseCodes.Success)
                        ////    {
                        ////        return new InitiatePaymentResponse { ResponseCode = generateToken.ResponseCode };
                        ////    }
                        ////    paymentResponse.CustomerId = customerId; paymentResponse.PaymentLink = Convert.ToString(generateToken.Data);
                        ////    return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse };
                        ////}
                        
                        encryptedText = $"{_appSettings.mid}{_appSettings.paymentCombination}{logCustomerInfo.Amount}{_appSettings.paymentCombination}{paymentRef}";
                        encryptData = _encryptDecryptAlgorithm.EncryptAlt(encryptedText);
                        paymentData = _appSettings.sterlingpaymentGatewayRequestUrl + encryptData;
                        paymentResponse.CustomerId = customerId; paymentResponse.PaymentLink = paymentData;
                        _log4net.Info("MakePayment info was successful" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);

                        return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse, PaymentRef = paymentRef };
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
                        .InitiatePayment(getPaymentDetails.TotalAmount, "Social pay", paymentRef,
                        getMerchantId.SpectaMerchantID, getMerchantId.SpectaMerchantKey);
                   
                    if (generateToken.ResponseCode != AppResponseCodes.Success)
                    {
                        return new InitiatePaymentResponse { ResponseCode = generateToken.ResponseCode};
                    }
                   
                    paymentResponse.CustomerId = customerId; paymentResponse.PaymentLink = Convert.ToString(generateToken.Data);
                   
                    _log4net.Info("MakePayment info was successful" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | "+ DateTime.Now);

                    return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse, PaymentRef = paymentRef };
                }
                logCustomerInfo.Amount = getPaymentDetails.TotalAmount;
                await _context.CustomerOtherPaymentsInfo.AddAsync(logCustomerInfo);
                await _context.SaveChangesAsync();
                encryptedText =$"{_appSettings.mid}{_appSettings.paymentCombination}{getPaymentDetails.TotalAmount}{_appSettings.paymentCombination}{paymentRef}";
                encryptData = _encryptDecryptAlgorithm.EncryptAlt(encryptedText);
                //var initiatepayment = Process.Start("cmd", "/C start " + _appSettings.sterlingpaymentGatewayRequestUrl + encryptData);
                paymentData =  _appSettings.sterlingpaymentGatewayRequestUrl + encryptData;
                paymentResponse.CustomerId = customerId; paymentResponse.PaymentLink = paymentData;
                _log4net.Info("MakePayment info was successful" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | " + DateTime.Now);
                return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.Success, Data = paymentResponse, PaymentRef = paymentRef };
            }
            catch (Exception ex)
            {
                _log4net.Error("An error occured while trying to initiate payment MakePayment" + " | " + model.TransactionReference + " | " + model.PhoneNumber + " | "+ ex.Message.ToString() + " | "+ DateTime.Now);
                return new InitiatePaymentResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> PaymentConfirmation(PaymentValidationRequestDto model)
        {
            try
            {

                var otherPaymentReference = string.Empty;
                _log4net.Info("PaymentConfirmation request" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + model.Message + " | "+ DateTime.Now);

                var logResponse = new PaymentResponse
                {
                    Message = model.Message, PaymentReference = model.PaymentReference,
                    TransactionReference = model.TransactionReference
                };
                await _context.PaymentResponse.AddAsync(logResponse);
                await _context.SaveChangesAsync();

                var validateLinkType = await _customerService.GetLinkCategorybyTranref(model.TransactionReference);

                if(validateLinkType.Channel == MerchantPaymentLinkCategory.InvoiceLink)
                {
                    if (model.Channel == PaymentChannel.Card || model.Channel == PaymentChannel.OneBank)
                    {
                        if (model.Channel == PaymentChannel.Card)
                        {
                            var decodeMessage = System.Uri.UnescapeDataString(model.Message);
                            if (decodeMessage.Contains(" "))
                            {
                                decodeMessage = decodeMessage.Replace(" ", "+");
                            }
                            var decryptResponse = DecryptAlt(decodeMessage);
                            model.Message = decryptResponse;
                            _log4net.Info("PaymentConfirmation decrypted message" + " | " + model.PaymentReference + " | " + model.Message + " | " + DateTime.Now);

                            var newreference = model.Message.Split("^");

                            foreach (var item in newreference)
                            {
                                if (item.Contains("SBP") || item.Contains("sbp"))
                                {
                                    otherPaymentReference = item;
                                }
                            }

                            var result = await _customerService.LogInvoicePaymentResponse(model, otherPaymentReference);
                            _log4net.Info("PaymentConfirmation response was successful" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + DateTime.Now);

                            return result;
                        }

                        var oneBankRequest = await _customerService.LogInvoicePaymentResponse(model, otherPaymentReference);
                        _log4net.Info("PaymentConfirmation response was successful" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + DateTime.Now);

                        return oneBankRequest;

                    }
                    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidPamentChannel };
                }
               
                if (model.Channel == PaymentChannel.Card || model.Channel == PaymentChannel.OneBank || model.Channel == PaymentChannel.PayWithSpecta)
                {
                    if(model.Channel == PaymentChannel.Card)
                    {
                        var decodeMessage = System.Uri.UnescapeDataString(model.Message);
                        if (decodeMessage.Contains(" "))
                        {
                            decodeMessage = decodeMessage.Replace(" ", "+");
                        }
                        var decryptResponse = DecryptAlt(decodeMessage);

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

                        return await _customerService.LogPaymentResponse(model, otherPaymentReference);

                       // return result;
                    }

                    if (model.Channel == PaymentChannel.PayWithSpecta)
                    {
                        var result = await _payWithSpectaService.PaymentVerification(model.Message);

                        if(result.ResponseCode != AppResponseCodes.Success)
                        {
                            _log4net.Info("PaymentConfirmation failed" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | " + DateTime.Now);
                            return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionFailed };
                        }

                        model.Message = $"{"success"}{result.Message}";

                        return await _customerService.LogPaymentResponse(model, string.Empty);
                    }

                    var oneBankRequest = await _customerService.LogPaymentResponse(model, string.Empty);
                    _log4net.Info("PaymentConfirmation response was successful" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | "  + DateTime.Now);

                    return oneBankRequest;

                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidPamentChannel };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "PaymentConfirmation" + " | " + model.PaymentReference + " | " + model.TransactionReference + " | "+ ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> AcceptOrRejectItem(AcceptRejectRequestDto model, long clientId)
        {
            _log4net.Info("AcceptOrRejectItem" + " | " + clientId + " | " + model.PaymentReference + "" + model.TransactionReference + " | " + DateTime.Now);

            try
            {
                var validateReference = await _customerService.AcceptRejectOrderRequest(model, clientId);
                return validateReference;
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "AcceptOrRejectItem" + " | " + clientId + " | " + model.PaymentReference + ""+ model.TransactionReference + " | "+ ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> DecryptMessage(string message)
        {
            try
            {
                var decodeMessage = System.Uri.UnescapeDataString(message);
                if (decodeMessage.Contains(" "))
                {
                    decodeMessage = decodeMessage.Replace(" ", "+");
                }
                var decryptResponse = DecryptAlt(decodeMessage);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = decryptResponse };

            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> DecryptSpectaMessage(string message)
        {
            try
            {
                var apiResponse = new WebApiResponse { };

                var decodeMessage = Uri.UnescapeDataString(message);

                var model = new PayWithSpectaVerificationRequestDto
                {
                    verificationToken = decodeMessage,
                };
                var request = JsonConvert.SerializeObject(model);

                var client = new HttpClient();
                client.BaseAddress = new Uri(_appSettings.paywithSpectaBaseUrl);
                var response = await client.PostAsync(_appSettings.paywithSpectaverifyPaymentUrl,
                    new StringContent(request, Encoding.UTF8, "application/json"));

                var result = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var successfulResponse = JsonConvert.DeserializeObject<PaymentVerificationResponseDto>(result);
                    apiResponse.Data = successfulResponse.Result;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    apiResponse.Message = Convert.ToString(successfulResponse.Result.Data.PaymentReference);
                    _log4net.Info("PaymentVerification was successful" + " | " + apiResponse.Message + " | " + result + " | " + DateTime.Now);

                    return apiResponse;
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };

            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> ValidateSpectaResponse(string paymentReference, string transactionReference)
        {
            try
            {
                var apiResponse = new WebApiResponse { };

                var confirmTransaction = await _context.TransactionLog
                   .SingleOrDefaultAsync(x => x.PaymentReference == paymentReference
                   && x.TransactionReference == transactionReference);

                if (confirmTransaction == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };

                var validateRequest = await _context.PaymentResponse
                    .SingleOrDefaultAsync(x => x.PaymentReference == paymentReference
                    && x.TransactionReference == transactionReference);
                
                if (validateRequest == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };             

                var decodeMessage = Uri.UnescapeDataString(validateRequest.Message);

                var model = new PayWithSpectaVerificationRequestDto
                {
                    verificationToken = decodeMessage,
                };
                var request = JsonConvert.SerializeObject(model);

                var client = new HttpClient();
                client.BaseAddress = new Uri(_appSettings.paywithSpectaBaseUrl);
                var response = await client.PostAsync(_appSettings.paywithSpectaverifyPaymentUrl,
                    new StringContent(request, Encoding.UTF8, "application/json"));

                var result = await response.Content.ReadAsStringAsync();

                var message = string.Empty;
                if (response.IsSuccessStatusCode)
                {
                    var successfulResponse = JsonConvert.DeserializeObject<PaymentVerificationResponseDto>(result);
                    apiResponse.Data = successfulResponse.Result;
                    apiResponse.ResponseCode = AppResponseCodes.Success;
                    apiResponse.Message = Convert.ToString(successfulResponse.Result.Data.PaymentReference);

                    message = $"{"success"}{apiResponse.Message}";

                    var linkInfo = await GetLinkCategorybyTranref(transactionReference);

                    var paymentSetupInfo = await _context.MerchantPaymentSetup
                   .SingleOrDefaultAsync(x => x.TransactionReference == transactionReference);

                    if (paymentSetupInfo == null)
                    {
                        _log4net.Info("LogPaymentResponse" + " - " + paymentReference + " - " + "paymentSetupInfo. RecordNotFound" + " - " + DateTime.Now);
                        return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };
                    }

                    var merchantInfo = await GetMerchantInfo(paymentSetupInfo.ClientAuthenticationId);

                    var getCustomerInfo = await _context.CustomerOtherPaymentsInfo
                        .SingleOrDefaultAsync(x => x.PaymentReference == paymentReference);


                    if (message.Contains("approve") || message.Contains("success") || message.Contains("Approve"))
                    {
                        var logconfirmation = new TransactionLog { };
                        logconfirmation.Category = linkInfo.Channel;
                        logconfirmation.LinkCategory = paymentSetupInfo.PaymentCategory;
                        logconfirmation.PaymentChannel = "03";
                        logconfirmation.ClientAuthenticationId = paymentSetupInfo.ClientAuthenticationId;
                        logconfirmation.CustomerTransactionReference = Guid.NewGuid().ToString();
                        logconfirmation.TransactionReference = transactionReference;
                        logconfirmation.OrderStatus = TransactionJourneyStatusCodes.Pending;
                        logconfirmation.Message = message;
                        logconfirmation.LastDateModified = DateTime.Now;
                        logconfirmation.TotalAmount = getCustomerInfo.Amount;
                        logconfirmation.DeliveryDayTransferStatus = TransactionJourneyStatusCodes.Pending;
                        logconfirmation.PaymentReference = paymentReference;
                        logconfirmation.TransactionStatus = TransactionJourneyStatusCodes.Approved;
                        logconfirmation.TransactionJourney = TransactionJourneyStatusCodes.Approved;
                        logconfirmation.ActivityStatus = TransactionJourneyStatusCodes.Approved;
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
                                await transaction.CommitAsync();
                                //Send mail
                                await _transactionReceipt.ReceiptTemplate(logconfirmation.CustomerEmail, paymentSetupInfo.TotalAmount,
                                    logconfirmation.TransactionDate, transactionReference, merchantInfo == null ? string.Empty : merchantInfo.BusinessName);

                                var emailModal = new EmailRequestDto
                                {
                                    Subject = $"{_appSettings.successfulTransactionEmailSubject}{"-"}{transactionReference}{"-"}",
                                    DestinationEmail = merchantInfo.BusinessEmail,
                                };

                                var mailBuilder = new StringBuilder();
                                mailBuilder.AppendLine("Dear" + " " + merchantInfo.BusinessName + "," + "<br />");
                                mailBuilder.AppendLine("<br />");
                                mailBuilder.AppendLine("Customer was able to make payment successfully. See details below.<br />");
                                mailBuilder.AppendLine();
                                mailBuilder.AppendLine("Customer Name" + "  " + getCustomerInfo.Fullname + "<br />");
                                mailBuilder.AppendLine();
                                mailBuilder.AppendLine("Customer Phone number" + "  " + getCustomerInfo.PhoneNumber + "<br />");
                                // mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                                mailBuilder.AppendLine("Best Regards,");
                                emailModal.EmailBody = mailBuilder.ToString();

                                await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);


                                emailModal.DestinationEmail = getCustomerInfo.Email;
                                mailBuilder.AppendLine("Dear" + " " + getCustomerInfo.Fullname + "," + "<br />");
                                mailBuilder.AppendLine("<br />");
                                mailBuilder.AppendLine("Your payment was successful. See details below.<br />");
                                mailBuilder.AppendLine();
                                // mailBuilder.AppendLine("Token will expire in" + "  " + _appSettings.TokenTimeout + "  " + "Minutes" + "<br />");
                                mailBuilder.AppendLine("Best Regards,");
                                emailModal.EmailBody = mailBuilder.ToString();
                                await _emailService.SendMail(emailModal, _appSettings.EwsServiceUrl);

                                _log4net.Info("Emails was successfully sent" + " | " + "LogPaymentResponse" + " | " + paymentReference + " | " + transactionReference + " | " + DateTime.Now);

                                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                            }
                            catch (Exception ex)
                            {
                                _log4net.Error("Database Error occured" + " | " + "LogPaymentResponse" + " | " + paymentReference + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                                await transaction.RollbackAsync();
                                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                            }
                        }
                    }

                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        _log4net.Info("Transaction failed" + " | " + "LogPaymentResponse" + " | " + paymentReference + " | " + transactionReference + " | " + message + " - " + DateTime.Now);

                        try
                        {
                            var logFailedResponse = new FailedTransactions();

                            //logFailedResponse.CustomerTransactionReference = model.CustomerTransactionReference;
                            logFailedResponse.TransactionReference = transactionReference;
                            logFailedResponse.Message = message;
                            await _context.FailedTransactions.AddAsync(logFailedResponse);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                         
                            return new WebApiResponse { ResponseCode = AppResponseCodes.TransactionFailed };
                        }
                        catch (Exception ex)
                        {
                            _log4net.Error("Error occured" + " | " + "LogPaymentResponse" + " | " + paymentReference + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                            await transaction.RollbackAsync();
                            return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
                        }
                    }
                }
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = result };

            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
        public async Task<MerchantBusinessInfo> GetMerchantInfo(long clientId)
        {
            return await _context.MerchantBusinessInfo.SingleOrDefaultAsync(p => p.ClientAuthenticationId
             == clientId
           );
        }
        public async Task<LinkCategory> GetLinkCategorybyTranref(string tranRef)
        {
            return await _context.LinkCategory.SingleOrDefaultAsync(p => p.TransactionReference
             == tranRef
           );
        }

        public static String DecryptAlt(String val)
        {
            MemoryStream ms = new MemoryStream();
            byte[] sharedkey = { 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11, 0x12, 0x11, 0x0D, 0x0B, 0x07, 0x02, 0x04, 0x08, 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11 };
            byte[] sharedvector = { 0x01, 0x02, 0x03, 0x05, 0x07, 0x0B, 0x0D, 0x11 };
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            byte[] toDecrypt = Convert.FromBase64String(val);
            CryptoStream cs = new CryptoStream(ms, tdes.CreateDecryptor(sharedkey, sharedvector), CryptoStreamMode.Write);

            cs.Write(toDecrypt, 0, toDecrypt.Length);
            cs.FlushFinalBlock();

            return Encoding.UTF8.GetString(ms.ToArray());
        }
    }
}
