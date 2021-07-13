﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Services.AzureBlob;
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

namespace SocialPay.Core.Services.Store
{
    public class StoreBaseRepository
    {
        private readonly SocialPayDbContext _context;
        private readonly AppSettings _appSettings;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly BlobService _blobService;
        public StoreBaseRepository(SocialPayDbContext context, IOptions<AppSettings> appSettings,
            IHostingEnvironment environment, BlobService blobService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _appSettings = appSettings.Value;
            _hostingEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
            _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));
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
    }
}