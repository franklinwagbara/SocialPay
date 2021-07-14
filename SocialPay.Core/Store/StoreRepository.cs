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

namespace SocialPay.Core.Store
{
    public class StoreRepository
    {
        private readonly IStoreService _storeService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductsService _productsService;
        private readonly StoreBaseRepository _storeBaseRepository;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly AppSettings _appSettings;
        private readonly BlobService _blobService;

        public IConfiguration Configuration { get; }
      
        public StoreRepository(IStoreService storeService, IProductCategoryService productCategoryService,
            IProductsService productsService, StoreBaseRepository storeBaseRepository,
            IHostingEnvironment environment, IOptions<AppSettings> appSettings,
            BlobService blobService, IConfiguration configuration)
        {
            _storeService = storeService ?? throw new ArgumentNullException(nameof(storeService));
            _productCategoryService = productCategoryService ?? throw new ArgumentNullException(nameof(productCategoryService));
            _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
            _storeBaseRepository = storeBaseRepository ?? throw new ArgumentNullException(nameof(storeBaseRepository));
            _hostingEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
            _appSettings = appSettings.Value;
            _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }
        public async Task<WebApiResponse> CreateNewStoreAsync(StoreRequestDto request, UserDetailsViewModel userModel)
        {
            return await _storeBaseRepository.CreateNewStore(request, userModel);
        }

        public async Task<WebApiResponse> GetStoreInfoAsync(UserDetailsViewModel userModel)
        {
            try
            {
                //userModel.ClientId = 167;

                var options = Configuration.GetSection(nameof(AzureBlobConfiguration)).Get<AzureBlobConfiguration>();

                var store = await _storeService.GetStoresByClientId(userModel.ClientId);

                if (store == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found" };

                foreach (var item in store)
                {
                    //item.Image = item.Image == null ? string.Empty : _appSettings.BaseApiUrl + item.FileLocation + "/" + item.Image;

                   // var fileDescription = "Store/90/Pat/90-ST--85fb-6cef773338fd.jpg";
                    ///https://monthlystatement.blob.core.windows.net/socialpay/Store/90/Pat/90-ST--85fb-6cef773338fd.jpg
                    var storageAccount = CloudStorageAccount.Parse(options.blobConnectionstring);
                    var blobClient = storageAccount.CreateCloudBlobClient();

                    CloudBlobContainer container = blobClient.GetContainerReference(options.containerName);
                    CloudBlockBlob blob = container.GetBlockBlobReference(item.FileLocation);

                    item.Image = blob.Uri.AbsoluteUri;
                }

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = store };
            }
            catch (Exception ex)
            {
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

                foreach (var item in request.Image)
                {
                    productImages.Add(new DefaultDocumentRequest { Image = request.Image.Select(x => x.Image).FirstOrDefault(), ImageGuidId = $"{blobRequest.ClientId}{"-"}{"PR-"}{Guid.NewGuid().ToString().Substring(18)}" });
                }

                blobRequest.ClientId = 90;
                blobRequest.RequestType = "Product";
                blobRequest.ImageDetail = productImages;
                blobRequest.ProductName = request.ProductName;

                await _blobService.UploadProducts(blobRequest);

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

                string path = Path.Combine(this._hostingEnvironment.WebRootPath, _appSettings.ProductsImage);

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string fileName = string.Empty;
                var newFileName = string.Empty;

               // fileName = (request.Image.FileName);

                var reference = $"{"So-"}{Guid.NewGuid().ToString("N")}";

                var FileExtension = Path.GetExtension(fileName);

                fileName = Path.Combine(_hostingEnvironment.WebRootPath, _appSettings.ProductsImage) + $@"\{newFileName}";

                newFileName = $"{reference}{FileExtension}";

                var filePath = Path.Combine(fileName, newFileName);

                var color = string.Empty;
                var size = string.Empty;

                color = request.Color.Aggregate((a, b) => a + ", " + b);

                size = string.Join(",", request.Size.ToArray());

                var model = new ProductsViewModel
                {
                    Description = request.Description,
                    Color = color,
                    Price = request.Price,
                    ProductCategoryId = request.ProductCategoryId,
                    ProductName = request.ProductName,
                    ProductReference = reference,
                    Size = size,
                    Options = request.Options,
                    StoreId = request.StoreId,
                    Image = newFileName,
                    FileLocation = _appSettings.ProductsImage
                };

                //request.Image.CopyTo(new FileStream(filePath, FileMode.Create));

                await _productsService.AddAsync(model);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Product was successfully saved" };
            }
            catch (Exception ex)
            {
                return new WebApiResponse
                {
                    ResponseCode = AppResponseCodes.InternalError
                };
            }
        }

    }
}
