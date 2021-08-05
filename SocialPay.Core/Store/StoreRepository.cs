using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Services.Store;
using Microsoft.AspNetCore.Hosting;
using System.IO;
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
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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
              //  userModel.ClientId = 167;

                var options = Configuration.GetSection(nameof(AzureBlobConfiguration)).Get<AzureBlobConfiguration>();

                var store = await _storeService.GetStoresByClientId(userModel.ClientId);

                if (store == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found" };

                foreach (var item in store)
                {

                    var linkName = await _merchantPaymentSetupService.GetPaymentLinksId(item.MerchantStoreId);
                    //item.Image = item.Image == null ? string.Empty : _appSettings.BaseApiUrl + item.FileLocation + "/" + item.Image;

                   // var fileDescription = "Store/90/Pat/90-ST--85fb-6cef773338fd.jpg";
                    ///https://monthlystatement.blob.core.windows.net/socialpay/Store/90/Pat/90-ST--85fb-6cef773338fd.jpg
                   // var storageAccount = CloudStorageAccount.Parse(options.blobConnectionstring);
                    var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=monthlystatement;AccountKey=TiB4RbTOMBFU85N3icORuByCenohH4zhVW644VYYW4O+fCJh8jBxzIE6l9hhlCwCb9lJq0jFDHdQtGe+xl0iAg==;EndpointSuffix=core.windows.net");
                    var blobClient = storageAccount.CreateCloudBlobClient();

                    //CloudBlobContainer container = blobClient.GetContainerReference(options.containerName);
                    CloudBlobContainer container = blobClient.GetContainerReference("socialpay");
                    CloudBlockBlob blob = container.GetBlockBlobReference(item.FileLocation);

                    item.Image = blob.Uri.AbsoluteUri;
                    item.StoreLink = linkName.PaymentLinkUrl;
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = store };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "Getting store" + " | " + ex + " | " + userModel.UserID + " | "+ DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }


        public async Task<WebApiResponse> GetStoreInfobyStoreIdAsync(long storeId)
        {
            _log4net.Info("Task starts to get stores" + " | " + storeId + " | " + DateTime.Now);

            try
            {

                var options = Configuration.GetSection(nameof(AzureBlobConfiguration)).Get<AzureBlobConfiguration>();

               // var products = await GetProductsByIdAsync(storeId);             
                return await GetProductsByIdAsync(storeId);
                // return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Store", Data = products };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "Getting store" + " | " + ex + " | " + storeId + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> UpdateStoreStoreAsync(StoreViewModel request, long clentId)
        {
            try
            {
                var store = await _storeService.GetStoreById(request.MerchantStoreId, clentId);

                if (store == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found" };

                var model = new StoreViewModel
                {
                    Description = request.Description,
                    StoreName = request.StoreName,
                    ClientAuthenticationId = clentId
                };

                await _storeService.UpdateAsync(model);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Store was successfully updated" };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> CreateProductCategoryAsync(ProductcategoryDto request, UserDetailsViewModel userModel)
        {
            try
            {

                var category = await _productCategoryService.GetCategoryByNameAndClientId(request.CategoryName, userModel.ClientId);

                if (category != null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateCategoryName, Message = "Duplicate Category Name" };

                var model = new ProductCategoryViewModel
                {
                    CategoryName = request.CategoryName,
                    ClientAuthenticationId = userModel.ClientId,
                    Description = request.Description
                };

                await _productCategoryService.AddAsync(model);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "New product category was successfully created" };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetProductCategoryAsync(UserDetailsViewModel userModel)
        {
            try
            {

                var categories = await _productCategoryService.GetAllByClientId(userModel.ClientId);

                if (categories == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found" };

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = categories };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> CreateNewProductAsync(ProductRequestDto request, UserDetailsViewModel userModel)
        {

            try
            {
                var blobRequest = new BlobProductsRequest();

                var productImages = new List<DefaultDocumentRequest>();

                blobRequest.ClientId = userModel.ClientId;
                blobRequest.RequestType = "Product";
                blobRequest.ProductName = request.ProductName;

                var validateCategory = await _productCategoryService.GetCategoryByIdAndCatId(request.ProductCategoryId, userModel.ClientId);

                if (validateCategory == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found" };

                var validateStore = await _storeService.GetStoreById(request.StoreId, userModel.ClientId);

                if (validateStore == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found" };

                var validatProductDetails = await _productsService
                    .GetProductByNameCatIdAndClientId(request.ProductName, request.ProductCategoryId, request.StoreId);

                if (validatProductDetails != null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateProductName, Message = "Duplicate Product Name" };

                return await _productsRepository.CreateNewProduct(request, userModel);


               // foreach (var item in request.Image)
               // {
               //     var imageGuidId = $"{blobRequest.ClientId}{"-"}{"PR-"}{Guid.NewGuid().ToString().Substring(18)}{".jpg"}";

               //     productImages.Add(new DefaultDocumentRequest { Image = item, ImageGuidId = imageGuidId,
               //         FileLocation = $"{blobRequest.RequestType}/{userModel.ClientId}/{blobRequest.ProductName}/{imageGuidId}"
               //     });
               // }

               // blobRequest.ImageDetail = productImages;

               // await _blobService.UploadProducts(blobRequest);
              
               // //string path = Path.Combine(this._hostingEnvironment.WebRootPath, _appSettings.ProductsImage);

               // //if (!Directory.Exists(path))
               // //    Directory.CreateDirectory(path);

               // //string fileName = string.Empty;
               // //var newFileName = string.Empty;

               //// fileName = (request.Image.FileName);

               // var reference = $"{"So-"}{Guid.NewGuid().ToString("N")}";

               // //var FileExtension = Path.GetExtension(fileName);

               // //fileName = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.ProductsImage) + $@"\{newFileName}";

               // //newFileName = $"{reference}{FileExtension}";

               // //var filePath = Path.Combine(fileName, newFileName);

               // var color = string.Empty;
               // var size = string.Empty;

               // color = request.Color.Aggregate((a, b) => a + ", " + b);

               // size = string.Join(",", request.Size.ToArray());

               // var model = new ProductsViewModel
               // {
               //     Description = request.Description,
               //     Color = color,
               //     Size = size,
               //     Price = request.Price,
               //     ProductCategoryId = request.ProductCategoryId,
               //     ProductName = request.ProductName,
               //     ProductReference = reference,
               //    // Options = request.Options,
               //     StoreId = request.StoreId,
               //     //Image = newFileName,
               //     FileLocation = _appSettings.ProductsImage
               // };

               // //request.Image.CopyTo(new FileStream(filePath, FileMode.Create));

               // await _productsService.AddAsync(model);

               // return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Product was successfully saved" };
            }
            catch (Exception ex)
            {
                return new WebApiResponse
                {
                    ResponseCode = AppResponseCodes.InternalError
                };
            }
        }

        public async Task<WebApiResponse> GetProductsAsync(UserDetailsViewModel userModel, long storeId)
        {
            try
            {
               // userModel.ClientId = 167;

                var options = Configuration.GetSection(nameof(AzureBlobConfiguration)).Get<AzureBlobConfiguration>();

                return  await _productsRepository.GetProductsByClientIdStoreId(userModel.ClientId, storeId);

                //if (store == null)
                //    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found" };

                //foreach (var item in store)
                //{
                //    //item.Image = item.Image == null ? string.Empty : _appSettings.BaseApiUrl + item.FileLocation + "/" + item.Image;

                //    // var fileDescription = "Store/90/Pat/90-ST--85fb-6cef773338fd.jpg";
                //    ///https://monthlystatement.blob.core.windows.net/socialpay/Store/90/Pat/90-ST--85fb-6cef773338fd.jpg
                //    // var storageAccount = CloudStorageAccount.Parse(options.blobConnectionstring);
                //    var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=monthlystatement;AccountKey=TiB4RbTOMBFU85N3icORuByCenohH4zhVW644VYYW4O+fCJh8jBxzIE6l9hhlCwCb9lJq0jFDHdQtGe+xl0iAg==;EndpointSuffix=core.windows.net");
                //    var blobClient = storageAccount.CreateCloudBlobClient();

                //    //CloudBlobContainer container = blobClient.GetContainerReference(options.containerName);
                //    CloudBlobContainer container = blobClient.GetContainerReference("socialpay");
                //    CloudBlockBlob blob = container.GetBlockBlobReference(item.FileLocation);

                //    item.Image = blob.Uri.AbsoluteUri;
                //}

                //return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = store };
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

                return await _productsRepository.GetProductsByClientIdProductId(productId);
               
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "Getting store" + " | " + ex + " | " + userModel.UserID + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }


        public async Task<WebApiResponse> GetProductsByIdAsync(long storeId)
        {
            try
            {
                // userModel.ClientId = 167;

                var options = Configuration.GetSection(nameof(AzureBlobConfiguration)).Get<AzureBlobConfiguration>();

                return await _productsRepository.GetProductsByStoreId(storeId);              
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "Getting store" + " | " + ex + " | " + storeId + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }

        }

    }
}
