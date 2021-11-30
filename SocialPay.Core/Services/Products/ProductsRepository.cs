using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SocialPay.Core.Configurations;
using SocialPay.Core.Services.AzureBlob;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.SerilogService.Store;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Products
{
    public class ProductsRepository
    {
        private readonly SocialPayDbContext _context;
        private readonly BlobService _blobService;
        private readonly StoreLogger _storeLogger;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(ProductsRepository));
        public IConfiguration Configuration { get; }
        public ProductsRepository(SocialPayDbContext context, BlobService blobService,
            IConfiguration configuration, StoreLogger storeLogger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));
            _storeLogger = storeLogger ?? throw new ArgumentNullException(nameof(storeLogger));
            Configuration = configuration;
        }

        public async Task<WebApiResponse> CreateNewProduct(ProductRequestDto request, UserDetailsViewModel userModel)
        {
            try
            {
                _storeLogger.LogRequest($"{"Task starts to create new product"}{" "}{request.ProductName}{" - "}{userModel.UserID}{" - "}{DateTime.Now}", false);

                //  _log4net.Info("Task starts to create new product" + " | " + request.ProductName + " - "+ userModel.UserID + " | " + DateTime.Now);

                // userModel.ClientId = 167;
                var reference = $"{"So-"}{Guid.NewGuid().ToString("N")}";

                var color = string.Empty;
                var size = string.Empty;

                color = request.Color.Aggregate((a, b) => a + ", " + b);

                size = string.Join(",", request.Size.ToArray());

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var options = Configuration.GetSection(nameof(AzureBlobConfiguration)).Get<AzureBlobConfiguration>();

                        var blobRequest = new BlobProductsRequest();

                        var productImages = new List<DefaultDocumentRequest>();

                        blobRequest.ClientId = userModel.ClientId;
                        blobRequest.RequestType = "Product";
                        blobRequest.ProductName = request.ProductName;

                        var model = new Product
                        {
                            Description = request.Description,
                            Color = color,
                            Size = size,
                            Price = request.Price,
                            ProductCategoryId = request.ProductCategoryId,
                            ProductName = request.ProductName,
                            ProductReference = reference,
                            MerchantStoreId = request.StoreId,
                            FileLocation = $"{blobRequest.RequestType}/{userModel.ClientId}/{blobRequest.ProductName}"
                        };

                        await _context.Products.AddAsync(model);
                        await _context.SaveChangesAsync();

                        var proDetails = new List<ProductItems>();

                        foreach (var item in request.Image)
                        {
                            var filePath = $"{blobRequest.ClientId}{"-"}{"PR-"}{Guid.NewGuid().ToString().Substring(18)}{".jpg"}";

                            var fileLocation = $"{blobRequest.RequestType}/{userModel.ClientId}/{blobRequest.ProductName}/{filePath}";

                            productImages.Add(new DefaultDocumentRequest
                            {
                                Image = item,
                                ImageGuidId = filePath,
                                FileLocation = fileLocation
                            });

                            blobRequest.ImageDetail = productImages;

                            proDetails.Add(new ProductItems { FileLocation = $"{options.blobBaseUrl}{options.containerName}{"/"}{fileLocation}", ProductId = model.ProductId });

                            await _blobService.UploadProducts(blobRequest);

                            productImages.Clear();
                            blobRequest.ImageDetail.Clear();
                        }

                        var productInventory = new ProductInventory
                        {
                            ProductId = model.ProductId,
                            Quantity = request.Quantity,
                            LastDateModified = DateTime.Now
                        };

                        await _context.ProductInventory.AddAsync(productInventory);
                        await _context.SaveChangesAsync();

                        var inventoryHistory = new ProductInventoryHistory
                        {
                            ProdId = model.ProductId,
                            ClientAuthenticationId = userModel.ClientId,
                            Quantity = request.Quantity,
                            IsAdded = true,
                            Amount = request.Price,
                            IsUpdated = false,
                            ProductInventoryId = productInventory.ProductInventoryId,
                            LastDateModified = DateTime.Now
                        };

                        await _context.productInventoryHistories.AddAsync(inventoryHistory);
                        await _context.SaveChangesAsync();

                        await _context.ProductItems.AddRangeAsync(proDetails);
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"{"Product "}{request.ProductName}{" was successfully saved"}", StatusCode = ResponseCodes.Success };
                    }
                    catch (Exception ex)
                    {
                        _storeLogger.LogRequest($"{"An error occured while trying to create product"}{" "}{userModel.Email}{" - "}{request.ProductName}{" - "}{ex}{" - "}{DateTime.Now}", true);

                        //_log4net.Error("Error occured" + " | " + "Create new product" + " | " + ex + " | " + request.ProductName + " - " + userModel.UserID + " | " + DateTime.Now);

                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "Error occured while creating product", StatusCode = ResponseCodes.InternalError };
                    }
                }

            }
            catch (Exception ex)
            {
                _storeLogger.LogRequest($"{"An error occured while trying to create product"}{" "}{userModel.Email}{" - "}{request.ProductName}{" - "}{ex}{" - "}{DateTime.Now}", true);

                // _log4net.Error("Error occured" + " | " + "Create new product" + " | " + ex + " | " + request.ProductName + " - "+ userModel.UserID + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "Error occured while creating new product", StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> UpdateProductAsync(ProductUpdateDto productUpdateDto, UserDetailsViewModel userModel)
        {
            try
            {
                var product = await _context.Products
                    .Include(x=>x.ProductInventory)
                    .SingleOrDefaultAsync(p => p.ProductId == productUpdateDto.ProductId);

                if (product == default)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = $"{"Product not found"}", StatusCode = ResponseCodes.RecordNotFound };

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        var color = string.Empty;
                        var size = string.Empty;

                        color = productUpdateDto.Color.Aggregate((a, b) => a + ", " + b);

                        size = string.Join(",", productUpdateDto.Size.ToArray());

                        product.Description = productUpdateDto.Description;
                        product.Color = color;
                        product.Size = size;
                        product.ProductName = productUpdateDto.ProductName;
                        product.LastDateModified = DateTime.Now;

                        _context.Update(product);
                        await _context.SaveChangesAsync();

                        var productHistory = new ProductInventoryHistory
                        {
                            ProdId = productUpdateDto.ProductId,
                            LastDateModified = DateTime.Now,
                            ClientAuthenticationId = userModel.ClientId,
                            IsUpdated = true,
                            IsAdded = false,
                            ProductInventoryId = product.ProductInventory.Select(x=>x.ProductInventoryId).FirstOrDefault(),
                        };

                        await _context.productInventoryHistories.AddAsync(productHistory);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"{"Product "} {productUpdateDto.ProductName}{" was successfully updated"}", StatusCode = ResponseCodes.Success };

                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "Error occured while updating product inventory", StatusCode = ResponseCodes.InternalError };
                    }
                }

            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "Error occured while updating product inventory", StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> UpdateProductInventoryAsync(ProductInventoryDto productInventoryDto, UserDetailsViewModel userModel)
        {
            try
            {
                var productInventory = await _context.ProductInventory.SingleOrDefaultAsync(p => p.ProductId == productInventoryDto.ProductId);

                if(productInventory == default)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = $"{"Product not found"}", StatusCode = ResponseCodes.RecordNotFound };

                using(var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {
                        productInventory.Quantity = productInventory.Quantity + productInventoryDto.Quantity;
                        productInventory.LastDateModified = DateTime.Now;

                        _context.Update(productInventory);
                        await _context.SaveChangesAsync();

                        var productHistory = new ProductInventoryHistory
                        {
                            ProdId = productInventoryDto.ProductId,
                            Quantity = productInventoryDto.Quantity,
                            LastDateModified = DateTime.Now,
                            ClientAuthenticationId = userModel.ClientId,
                            IsUpdated = true,
                            IsAdded = false,
                            ProductInventoryId = productInventory.ProductInventoryId,
                        };

                        await _context.productInventoryHistories.AddAsync(productHistory);
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();

                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = $"{"Product inventory was successfully updated"}", StatusCode = ResponseCodes.Success };

                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "Error occured while updating product inventory", StatusCode = ResponseCodes.InternalError };
                    }
                }
              
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "Error occured while updating product inventory", StatusCode = ResponseCodes.InternalError };
            }
        }
        public async Task<WebApiResponse> GetProductsByClientId(long clientId)
        {
            try
            {
                _storeLogger.LogRequest($"{"Task starts to product by client Id"}{" "}{clientId}{" - "}{DateTime.Now}", false);

                //  _log4net.Info("Task starts to get product by clientId" + " | " + clientId + " - " +  DateTime.Now);

                var productCategory = await _context.ProductCategories.Where(x => x.ClientAuthenticationId == clientId).ToListAsync();

                var query = (from c in productCategory
                             join p in _context.Products on c.ProductCategoryId equals p.ProductCategoryId
                             join s in _context.MerchantStore on p.MerchantStoreId equals s.MerchantStoreId

                             select new StoreProductsViewModel
                             {
                                 StoreName = s.StoreName,
                                 ProductName = p.ProductName,
                                 Price = p.Price,
                                 Size = p.Size,
                                 Category = c.CategoryName,
                                 Color = p.Color,
                                 Options = p.Options,
                                 StoreDescription = s.Description,
                                 ProductDescription = p.Description

                             }).ToList();

                if (query.Count > 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = query, StatusCode = ResponseCodes.Success };

                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found", StatusCode = ResponseCodes.RecordNotFound };
            }
            catch (Exception ex)
            {
                _storeLogger.LogRequest($"{"An error occured while trying to product"}{" "}{clientId}{" - "}{ex}{" - "}{DateTime.Now}", true);

                // _log4net.Error("Error occured" + " | " + "Getting product" + " - " + ex + " - " +  clientId + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured", StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetProductsByClientIdStoreId(long clientId, long storeId, string azureConnectionString, string containerName)
        {
            try
            {
                _storeLogger.LogRequest($"{"Task starts to get product by clientId and storeid"}{" "}{clientId}{" - "}{storeId}{" - "}{DateTime.Now}", false);

                //_log4net.Info("Task starts to get product by clientId and storeid" + " | " + clientId + " - " + storeId + " - "+ DateTime.Now);

                var productItems = new List<ProductItemViewModel>();

                var stores = await _context.MerchantStore
                    .SingleOrDefaultAsync(x => x.ClientAuthenticationId == clientId && x.MerchantStoreId == storeId);

                var query = (from pro in _context.Products
                             where pro.MerchantStoreId == stores.MerchantStoreId
                             join pc in _context.ProductCategories on pro.ProductCategoryId equals pc.ProductCategoryId
                             join pi in _context.ProductInventory on pro.ProductId equals pi.ProductId

                             select new ProductsDetailsViewModel
                             {
                                 StoreName = stores.StoreName,
                                 ProductName = pro.ProductName,
                                 Price = pro.Price,
                                 Size = pro.Size,
                                 Category = pc.CategoryName,
                                 Color = pro.Color,
                                 Options = pro.Options,
                                 ProductDescription = pro.Description,
                                 ProductId = pro.ProductId,
                                 Quantity = pi.Quantity
                             }).ToList();

               // query.Select(x => x.merchantStoreViewModel = new MerchantStoreViewModel { StoreName = stores.StoreName });
                var storageAccount = CloudStorageAccount.Parse(azureConnectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                foreach (var item in query)
                {
                    var getProductsItem = await (from p in _context.ProductItems
                                           .Where(x => x.ProductId == item.ProductId)
                                                 select new ProductItemViewModel
                                                 {
                                                     FileLocation = p.FileLocation
                                                 }).ToListAsync();

                    foreach (var image in getProductsItem)
                    {
                        CloudBlockBlob blob = container.GetBlockBlobReference(image.FileLocation);

                        image.Url = blob.Uri.AbsoluteUri;

                        item.ProductItemsViewModel = getProductsItem;
                    }


                }



                //var query = (from s in stores
                //             join pc in _context.ProductCategories on s.ClientAuthenticationId equals pc.ClientAuthenticationId
                //             join pr in _context.Products on pc.ProductCategoryId equals pr.ProductCategoryId
                //             join pi in _context.ProductInventory on pr.ProductId equals pi.ProductId
                //             join proItems in _context.ProductItems on pr.ProductId equals proItems.ProductId
                //             where pr.MerchantStoreId == storeId

                //             select new StoreProductsViewModel
                //             {
                //                 StoreName = s.StoreName,
                //                 ProductName = pr.ProductName,
                //                 Price = pr.Price,
                //                 Size = pr.Size,
                //                 Category = pc.CategoryName,
                //                 Color = pr.Color,
                //                 Options = pr.Options,
                //                 StoreDescription = s.Description,
                //                 ProductDescription = pr.Description,
                //                 ProductId = proItems.ProductItemsId,
                //                 //ProductId = pr.ProductId,
                //                 Quantity = pi.Quantity,
                //                 ProductItemsViewModel = new List<ProductItemViewModel>()
                //                 {
                //                     new ProductItemViewModel
                //                     {
                //                         FileLocation = proItems.FileLocation
                //                     }
                //                 }
                //             }).ToList();


                ////var storageAccount = CloudStorageAccount.Parse(azureConnectionString);
                ////var blobClient = storageAccount.CreateCloudBlobClient();

                ////CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                ////foreach (var item in query)
                ////{
                ////    var getProductsItem = await (from p in _context.ProductItems
                ////                           .Where(x => x.ProductId == item.ProductId)
                ////                                 select new ProductItemViewModel
                ////                                 {
                ////                                     FileLocation = p.FileLocation
                ////                                 }).ToListAsync();

                ////    foreach (var image in getProductsItem)
                ////    {

                ////        CloudBlockBlob blob = container.GetBlockBlobReference(image.FileLocation);

                ////        image.Url = blob.Uri.AbsoluteUri;

                ////        item.ProductItemsViewModel = getProductsItem;
                ////    }


                ////}

                if (query.Count > 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = query, StatusCode = ResponseCodes.Success };

                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found", StatusCode = ResponseCodes.RecordNotFound };
            }
            catch (Exception ex)
            {
                _storeLogger.LogRequest($"{"An error occured while trying to get product by Id"}{" "}{clientId}{" - "}{ex}{" - "}{DateTime.Now}", true);

                // _log4net.Error("Error occured" + " | " + "Get products by client id" + " | " + ex + " | " + clientId + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured", StatusCode = ResponseCodes.InternalError };
            }
        }
        public async Task<WebApiResponse> GetProductsByClientIdStoreIdOld(long clientId, long storeId, string azureConnectionString, string containerName)
        {
            try
            {
                _storeLogger.LogRequest($"{"Task starts to get product by clientId and storeid"}{" "}{clientId}{" - "}{storeId}{" - "}{DateTime.Now}", false);

                //_log4net.Info("Task starts to get product by clientId and storeid" + " | " + clientId + " - " + storeId + " - "+ DateTime.Now);

                var productItems = new List<ProductItemViewModel>();

                var stores = await _context.MerchantStore
                    .Where(x => x.ClientAuthenticationId == clientId && x.MerchantStoreId == storeId).ToListAsync();

                var query = (from s in stores
                             join pc in _context.ProductCategories on s.ClientAuthenticationId equals pc.ClientAuthenticationId
                             join pr in _context.Products on pc.ProductCategoryId equals pr.ProductCategoryId
                             join pi in _context.ProductInventory on pr.ProductId equals pi.ProductId
                             join proItems in _context.ProductItems on pr.ProductId equals proItems.ProductId
                             where pr.MerchantStoreId == storeId

                             select new StoreProductsViewModel
                             {
                                 StoreName = s.StoreName,
                                 ProductName = pr.ProductName,
                                 Price = pr.Price,
                                 Size = pr.Size,
                                 Category = pc.CategoryName,
                                 Color = pr.Color,
                                 Options = pr.Options,
                                 StoreDescription = s.Description,
                                 ProductDescription = pr.Description,
                                 ProductId = proItems.ProductItemsId,
                                 //ProductId = pr.ProductId,
                                 Quantity = pi.Quantity,
                                 ProductItemsViewModel = new List<ProductItemViewModel>()
                                 {
                                     new ProductItemViewModel
                                     {
                                         FileLocation = proItems.FileLocation
                                     }
                                 }
                             }).ToList();


                ////var storageAccount = CloudStorageAccount.Parse(azureConnectionString);
                ////var blobClient = storageAccount.CreateCloudBlobClient();

                ////CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                ////foreach (var item in query)
                ////{
                ////    var getProductsItem = await (from p in _context.ProductItems
                ////                           .Where(x => x.ProductId == item.ProductId)
                ////                                 select new ProductItemViewModel
                ////                                 {
                ////                                     FileLocation = p.FileLocation
                ////                                 }).ToListAsync();

                ////    foreach (var image in getProductsItem)
                ////    {

                ////        CloudBlockBlob blob = container.GetBlockBlobReference(image.FileLocation);

                ////        image.Url = blob.Uri.AbsoluteUri;

                ////        item.ProductItemsViewModel = getProductsItem;
                ////    }


                ////}

                if (query.Count > 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = query, StatusCode = ResponseCodes.Success };

                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found", StatusCode = ResponseCodes.RecordNotFound };
            }
            catch (Exception ex)
            {
                _storeLogger.LogRequest($"{"An error occured while trying to get product by Id"}{" "}{clientId}{" - "}{ex}{" - "}{DateTime.Now}", true);

                // _log4net.Error("Error occured" + " | " + "Get products by client id" + " | " + ex + " | " + clientId + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured", StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetProductsByClientIdProductId(long productId, string azureConnectionString, string containerName)
        {
            try
            {
                _storeLogger.LogRequest($"{"Task starts to get product by productid"}{" "}{productId}{" - "}{DateTime.Now}", false);

                // _log4net.Info("Task starts to get product by productid" + " | " + productId + " - " + DateTime.Now);

                var productItems = new List<ProductItemViewModel>();

                var query = (from pro in _context.Products
                             where pro.ProductId == productId
                             join pc in _context.ProductCategories on pro.ProductCategoryId equals pc.ProductCategoryId
                             join pi in _context.ProductInventory on pro.ProductId equals pi.ProductId

                             select new ProductsDetailsViewModel
                             {
                                 ProductName = pro.ProductName,
                                 Price = pro.Price,
                                 Size = pro.Size,
                                 Category = pc.CategoryName,
                                 Color = pro.Color,
                                 Options = pro.Options,
                                 ProductDescription = pro.Description,
                                 ProductId = pro.ProductId,
                                 Quantity = pi.Quantity
                             }).ToList();


                var storageAccount = CloudStorageAccount.Parse(azureConnectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                foreach (var item in query)
                {
                    var getProductsItem = await (from p in _context.ProductItems
                                           .Where(x => x.ProductId == item.ProductId)
                                                 select new ProductItemViewModel
                                                 {
                                                     FileLocation = p.FileLocation
                                                 }).ToListAsync();

                    foreach (var image in getProductsItem)
                    {
                        CloudBlockBlob blob = container.GetBlockBlobReference(image.FileLocation);

                        image.Url = blob.Uri.AbsoluteUri;

                        item.ProductItemsViewModel = getProductsItem;
                    }


                }

                if (query.Count > 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = query, StatusCode = ResponseCodes.Success };

                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found", StatusCode = ResponseCodes.RecordNotFound };
            }
            catch (Exception ex)
            {
                _storeLogger.LogRequest($"{"An error occured while trying to get product by Id"}{" "}{productId}{" - "}{ex}{" - "}{DateTime.Now}", true);

                // _log4net.Error("Error occured" + " | " + "Get product by Id" + " | " + ex + " | " + productId + " - " +  DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured", StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetProductsByStoreId(long storeId, string transactionReference,
            string azureConnectionString, string containerName)
        {
            try
            {
                _storeLogger.LogRequest($"{"Task starts to get product by storeId"}{" "}{storeId}{" - "}{DateTime.Now}", false);

                // _log4net.Info("Task starts to get product by storeId" + " | " + storeId + " - " + DateTime.Now);

                var storeDetail = new StoreDetailsViewModel();

                var stores = await _context.MerchantStore
                    .Where(x => x.MerchantStoreId == storeId).ToListAsync();

                storeDetail.StoreName = stores.Select(x => x.StoreName).FirstOrDefault();
                storeDetail.StoreDescription = stores.Select(x => x.Description).FirstOrDefault();
                storeDetail.TransactionReference = transactionReference;

                var query = (from s in stores
                             join pc in _context.ProductCategories on s.ClientAuthenticationId equals pc.ClientAuthenticationId
                             join pr in _context.Products on pc.ProductCategoryId equals pr.ProductCategoryId
                             join pi in _context.ProductInventory on pr.ProductId equals pi.ProductId
                             join proItem in _context.ProductItems on pr.ProductId equals proItem.ProductId
                             where pr.MerchantStoreId == storeId

                             select new StoreProductsDetailsViewModel
                             {
                                 ProductName = pr.ProductName,
                                 Price = pr.Price,
                                 Size = pr.Size,
                                 Category = pc.CategoryName,
                                 Color = pr.Color,
                                 Options = pr.Options,
                                 ProductDescription = pr.Description,
                                 ProductId = pr.ProductId,
                                 Quantity = pi.Quantity,
                                 Products = new List<ProductItemViewModel>()
                                 {
                                     new ProductItemViewModel
                                     {
                                         FileLocation = proItem.FileLocation,
                                         Url = proItem.FileLocation
                                     }
                                 }

                             }).ToList();


                ////var storageAccount = CloudStorageAccount.Parse(azureConnectionString);

                ////var blobClient = storageAccount.CreateCloudBlobClient();

                ////CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                ////foreach (var item in query)
                ////{
                ////    var getProductsItem = await (from p in _context.ProductItems
                ////                           .Where(x => x.ProductId == item.ProductId)
                ////                                 select new ProductItemViewModel
                ////                                 {
                ////                                     FileLocation = p.FileLocation
                ////                                 }).ToListAsync();

                ////    foreach (var image in getProductsItem)
                ////    {                      
                ////        CloudBlockBlob blob = container.GetBlockBlobReference(image.FileLocation);

                ////        image.Url = blob.Uri.AbsoluteUri;

                ////        item.Products = getProductsItem;
                ////    }
                ////}

                ////storeDetail.StoreDetails = query;

                ////CloudBlockBlob storeblob = container.GetBlockBlobReference(stores.Select(x=>x.FileLocation).FirstOrDefault());

                //storeDetail.StoreLogoUrl = storeblob.Uri.AbsoluteUri;
                storeDetail.StoreDetails = query;
                storeDetail.StoreLogoUrl = stores.Select(x => x.FileLocation).FirstOrDefault();

                //  if (query.Count > 0)
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = storeDetail, StatusCode = ResponseCodes.Success };

                // return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found", Data = storeDetail, StatusCode = ResponseCodes.RecordNotFound };
            }
            catch (Exception ex)
            {
                _storeLogger.LogRequest($"{"An error occured while trying to get product by store Id"}{" "}{storeId}{" - "}{ex}{" - "}{DateTime.Now}", true);

                // _log4net.Error("Error occured" + " | " + "Get product by store id" + " | " + ex + " | " + storeId + " - " +  DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured", StatusCode = ResponseCodes.InternalError };
            }
        }

        public async Task<WebApiResponse> GetProductsByStoreIdOld(long storeId, string transactionReference,
           string azureConnectionString, string containerName)
        {
            try
            {
                _log4net.Info("Task starts to get product by storeId" + " | " + storeId + " - " + DateTime.Now);

                var storeDetail = new StoreDetailsViewModel();

                var stores = await _context.MerchantStore
                    .Where(x => x.MerchantStoreId == storeId).ToListAsync();

                storeDetail.StoreName = stores.Select(x => x.StoreName).FirstOrDefault();
                storeDetail.StoreDescription = stores.Select(x => x.Description).FirstOrDefault();
                storeDetail.TransactionReference = transactionReference;

                var query = (from s in stores
                             join pc in _context.ProductCategories on s.ClientAuthenticationId equals pc.ClientAuthenticationId
                             join pr in _context.Products on pc.ProductCategoryId equals pr.ProductCategoryId
                             join pi in _context.ProductInventory on pr.ProductId equals pi.ProductId
                             where pr.MerchantStoreId == storeId

                             select new StoreProductsDetailsViewModel
                             {
                                 ProductName = pr.ProductName,
                                 Price = pr.Price,
                                 Size = pr.Size,
                                 Category = pc.CategoryName,
                                 Color = pr.Color,
                                 Options = pr.Options,
                                 ProductDescription = pr.Description,
                                 ProductId = pr.ProductId,
                                 Quantity = pi.Quantity

                             }).ToList();


                var storageAccount = CloudStorageAccount.Parse(azureConnectionString);

                var blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                foreach (var item in query)
                {
                    var getProductsItem = await (from p in _context.ProductItems
                                           .Where(x => x.ProductId == item.ProductId)
                                                 select new ProductItemViewModel
                                                 {
                                                     FileLocation = p.FileLocation
                                                 }).ToListAsync();

                    foreach (var image in getProductsItem)
                    {
                        CloudBlockBlob blob = container.GetBlockBlobReference(image.FileLocation);

                        image.Url = blob.Uri.AbsoluteUri;

                        item.Products = getProductsItem;
                    }
                }

                storeDetail.StoreDetails = query;

                CloudBlockBlob storeblob = container.GetBlockBlobReference(stores.Select(x => x.FileLocation).FirstOrDefault());

                storeDetail.StoreLogoUrl = storeblob.Uri.AbsoluteUri;

                if (query.Count > 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = storeDetail };

                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Message = "Record not found" };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "Get product by store id" + " | " + ex + " | " + storeId + " - " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured" };
            }
        }

    }
}
