using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SocialPay.Core.Services.Store;
using Microsoft.AspNetCore.Hosting;
using SocialPay.Core.Configurations;
using Microsoft.Extensions.Options;
using SocialPay.Core.Services.AzureBlob;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;
using SocialPay.Core.Services.Products;

namespace SocialPay.Core.Store
{
    public class StoreRepository
    {
        private readonly IStoreService _storeService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductsService _productsService;
        private readonly IMerchantPaymentSetupService _merchantPaymentSetupService;
        private readonly StoreBaseRepository _storeBaseRepository;
        private readonly ProductsRepository _productsRepository;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AppSettings _appSettings;
        private readonly BlobService _blobService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(StoreRepository));

        public IConfiguration Configuration { get; }

        public StoreRepository(IStoreService storeService, IProductCategoryService productCategoryService,
            IProductsService productsService, StoreBaseRepository storeBaseRepository,
            IHostingEnvironment environment, IOptions<AppSettings> appSettings,
            BlobService blobService, IConfiguration configuration,
            ProductsRepository productsRepository, IMerchantPaymentSetupService merchantPaymentSetupService)
        {
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            _productCategoryService = productCategoryService ?? throw new ArgumentNullException(nameof(productCategoryService));
            _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
            _storeBaseRepository = storeBaseRepository ?? throw new ArgumentNullException(nameof(storeBaseRepository));
            _hostingEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
            _appSettings = appSettings.Value;
            _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));
            Configuration = configuration;
            _productsRepository = productsRepository ?? throw new ArgumentNullException(nameof(productsRepository));
            _merchantPaymentSetupService = merchantPaymentSetupService ?? throw new ArgumentNullException(nameof(merchantPaymentSetupService));
        }
        public async Task<WebApiResponse> CreateNewStoreAsync(StoreRequestDto request, UserDetailsViewModel userModel)
        {
            return await _storeBaseRepository.CreateNewStore(request, userModel);
        }

        public async Task<WebApiResponse> GetStoreInfoAsync(UserDetailsViewModel userModel)
        {
            _log4net.Info("Task starts to get stores" + " | " + userModel.UserID + " | " + DateTime.Now);

            try
            {
               // userModel.ClientId = 238;

                var options = Configuration.GetSection(nameof(AzureBlobConfiguration)).Get<AzureBlobConfiguration>();

                var store = await _storeService.GetStoresByClientId(userModel.ClientId);

                if (store == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found", StatusCode = ResponseCodes.RecordNotFound };

                //var storageAccount = CloudStorageAccount.Parse(options.blobConnectionstring);

                //var blobClient = storageAccount.CreateCloudBlobClient();

                //CloudBlobContainer container = blobClient.GetContainerReference(options.containerName);

                foreach (var item in store)
                {

                    var linkName = await _merchantPaymentSetupService.GetPaymentLinksId(item.MerchantStoreId);

                    if (linkName != null)
                    {
                        item.StoreLink = linkName.PaymentLinkUrl;
                    }
                    // CloudBlockBlob blob = container.GetBlockBlobReference(item.FileLocation);

                    //item.Image = blob.Uri.AbsoluteUri;
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = store, StatusCode = ResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "Getting store" + " | " + ex + " | " + userModel.UserID + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetStoreInfobyStoreIdAsync(long storeId, string transactionReference)
        {
            _log4net.Info("Task starts to get stores" + " | " + storeId + " | " + DateTime.Now);

            try
            {

                var options = Configuration.GetSection(nameof(AzureBlobConfiguration)).Get<AzureBlobConfiguration>();

                return await GetProductsByStoreIdAsync(storeId, transactionReference);
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "Getting store" + " | " + ex + " | " + storeId + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> UpdateStoreStoreAsync(StoreViewModel request, long clientId)
        {
            try
            {
                var store = await _storeService.GetStoreById(request.MerchantStoreId, clientId);

                if (store == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found" };

                var model = new StoreViewModel
                {
                    Description = request.Description,
                    StoreName = request.StoreName,
                    ClientAuthenticationId = clientId
                };

                await _storeService.UpdateAsync(model);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Store was successfully updated" };
            }
            catch (Exception ex)
            {
                _log4net.Error("An error occured while trying to update store" + " | " + request.StoreName + " | " + clientId + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> CreateProductCategoryAsync(ProductcategoryDto request, UserDetailsViewModel userModel)
        {
            try
            {
                _log4net.Info("Task starts to create store product category" + " | " + request.CategoryName + " | " + userModel.ClientId + " | " + DateTime.Now);

                var category = await _productCategoryService.GetCategoryByNameAndClientId(request.CategoryName, userModel.ClientId);

                if (category != null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateCategoryName, Message = "Duplicate Category Name", StatusCode = ResponseCodes.Duplicate };

                var model = new ProductCategoryViewModel
                {
                    CategoryName = request.CategoryName,
                    ClientAuthenticationId = userModel.ClientId,
                    Description = request.Description
                };

                await _productCategoryService.AddAsync(model);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "New product category was successfully created", StatusCode = ResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _log4net.Error("An error occured while trying to create store product category" + " | " + request.CategoryName + " | " + userModel.ClientId + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetProductCategoryAsync(UserDetailsViewModel userModel)
        {
            try
            {

                var categories = await _productCategoryService.GetAllByClientId(userModel.ClientId);

                if (categories == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found", StatusCode = ResponseCodes.RecordNotFound };

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = categories, StatusCode = ResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _log4net.Error("An error occured while trying to get product" + " | " + userModel.ClientId + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> CreateNewProductAsync(ProductRequestDto request, UserDetailsViewModel userModel)
        {

            try
            {
                _log4net.Info("Task starts to create product" + " | " + request.ProductName + " | " + userModel.ClientId + " | " + DateTime.Now);

                var blobRequest = new BlobProductsRequest();

                var productImages = new List<DefaultDocumentRequest>();

                blobRequest.ClientId = userModel.ClientId;
                blobRequest.RequestType = "Product";
                blobRequest.ProductName = request.ProductName;

                var validateCategory = await _productCategoryService.GetCategoryByIdAndCatId(request.ProductCategoryId, userModel.ClientId);

                if (validateCategory == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found", StatusCode = ResponseCodes.RecordNotFound };

                var validateStore = await _storeService.GetStoreById(request.StoreId, userModel.ClientId);

                if (validateStore == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found", StatusCode = ResponseCodes.RecordNotFound };

                var validatProductDetails = await _productsService
                    .GetProductByNameCatIdAndClientId(request.ProductName, request.ProductCategoryId, request.StoreId);

                if (validatProductDetails != null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateProductName, Message = "Duplicate Product Name", StatusCode = ResponseCodes.RecordNotFound };

                return await _productsRepository.CreateNewProduct(request, userModel);
            }
            catch (Exception ex)
            {
                _log4net.Error("An error occured while trying to create new product" + " | " + request.ProductName + " | " + userModel.ClientId + " | " + ex + " | " + DateTime.Now);

                return new WebApiResponse
                {
                    ResponseCode = AppResponseCodes.InternalError,
                    StatusCode = ResponseCodes.InternalError
                };
            }
        }

        public async Task<WebApiResponse> GetProductsAsync(UserDetailsViewModel userModel, long storeId)
        {
            try
            {
                //userModel.ClientId = 238;
                //storeId = 60;

                var options = Configuration.GetSection(nameof(AzureBlobConfiguration)).Get<AzureBlobConfiguration>();

                return await _productsRepository.GetProductsByClientIdStoreId(userModel.ClientId, storeId, options.blobConnectionstring, options.containerName);

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "Getting store" + " | " + ex + " | " + userModel.UserID + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }
        public async Task<WebApiResponse> GetProductsByIdAsync(UserDetailsViewModel userModel, long productId)
        {
            try
            {

                var options = Configuration.GetSection(nameof(AzureBlobConfiguration)).Get<AzureBlobConfiguration>();

                return await _productsRepository.GetProductsByClientIdProductId(productId,
                    options.blobConnectionstring, options.containerName);

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "Getting store" + " | " + ex + " | " + userModel.UserID + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, StatusCode = ResponseCodes.InternalError };
            }

        }


        public async Task<WebApiResponse> GetProductsByStoreIdAsync(long storeId, string transactionReference)
        {
            try
            {
                //userModel.ClientId = 167;

                var options = Configuration.GetSection(nameof(AzureBlobConfiguration)).Get<AzureBlobConfiguration>();

                return await _productsRepository.GetProductsByStoreId(storeId, transactionReference,
                    options.blobConnectionstring, options.containerName);
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "Getting store" + " | " + ex + " | " + storeId + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }

    }
}
