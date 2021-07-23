using Microsoft.EntityFrameworkCore;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SocialPay.Core.Services.AzureBlob;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Products
{
    public class ProductsRepository
    {
        private readonly SocialPayDbContext _context;
        private readonly BlobService _blobService;
        public ProductsRepository(SocialPayDbContext context, BlobService blobService)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _blobService = blobService ?? throw new ArgumentNullException(nameof(blobService));
        }

        public async Task<WebApiResponse> CreateNewProduct(ProductRequestDto request, UserDetailsViewModel userModel)
        {
            try
            {
                userModel.ClientId = 167;
                var reference = $"{"So-"}{Guid.NewGuid().ToString("N")}";
                //if (await _context.MerchantStore.AnyAsync(x => x.StoreName == request.StoreName && x.ClientAuthenticationId == userModel.ClientId))
                //    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateStoreName, Message = "Duplicate Store Name" };

                //if (await _context.MerchantStore.AnyAsync(x => x.StoreLink == request.StoreLink && x.ClientAuthenticationId == userModel.ClientId))
                //    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateLinkName, Message = "Duplicate Link Name" };

               
              //  blobRequest.ImageDetail = productImages;

               // await _blobService.UploadProducts(blobRequest);

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    try
                    {

                        var blobRequest = new BlobProductsRequest();

                        var productImages = new List<DefaultDocumentRequest>();

                        blobRequest.ClientId = userModel.ClientId;
                        blobRequest.RequestType = "Product";
                        blobRequest.ProductName = request.ProductName;

                        var model = new Product
                        {
                            Description = request.Description,
                            // Color = color,
                            Price = request.Price,
                            ProductCategoryId = request.ProductCategoryId,
                            ProductName = request.ProductName,
                            ProductReference = reference,
                            // Size = size,
                            Options = request.Options,
                            MerchantStoreId = request.StoreId,
                            FileLocation = $"{blobRequest.RequestType}/{userModel.ClientId}/{blobRequest.ProductName}"
                        };

                        await _context.Products.AddAsync(model);
                        await _context.SaveChangesAsync();

                        var proDetails = new List<ProductItems>();

                        foreach (var item in request.Image)
                        {
                            var filePath = $"{blobRequest.ClientId}{"-"}{"PR-"}{Guid.NewGuid().ToString().Substring(18)}{".jpg"}";

                            productImages.Add(new DefaultDocumentRequest
                            {
                                Image = item,
                                ImageGuidId = filePath,
                                FileLocation = $"{blobRequest.RequestType}/{userModel.ClientId}/{blobRequest.ProductName}/{filePath}"
                            });

                            blobRequest.ImageDetail = productImages;

                            proDetails.Add(new ProductItems { FileLocation = filePath, ProductId = model.ProductId });

                           // await _context.ProductItems.AddAsync(proDetails);
                            //await _context.SaveChangesAsync();

                            await _blobService.UploadProducts(blobRequest);

                            productImages.Clear();
                            blobRequest.ImageDetail.Clear(); 
                        }

                        await _context.ProductItems.AddRangeAsync(proDetails);
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


        public async Task<WebApiResponse> GetProductsByClientId(long clientId)
        {
            try
            {
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
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = query };

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Record not found" };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured" };
            }
        }


        public async Task<WebApiResponse> GetProductsByClientIdStoreId(long clientId, long storeId)
        {
            try
            {
                var productItems = new List<ProductItemViewModel>();

                var stores = await _context.MerchantStore
                    .Where(x => x.ClientAuthenticationId == clientId && x.MerchantStoreId == storeId).ToListAsync();

                var query = (from s in stores
                             join pc in _context.ProductCategories on s.ClientAuthenticationId equals pc.ClientAuthenticationId
                             join pr in _context.Products on pc.ProductCategoryId equals pr.ProductCategoryId
                            // join pi in _context.ProductItems on pr.ProductId equals pi.ProductId                          

                             //var employeeRecord = from e in stores
                             //                     join d in _context.ProductCategories on e.ClientAuthenticationId equals d.ClientAuthenticationId into table1
                             //                     from d in table1.ToList()
                             //                     join i in _context.Products on d.ProductCategoryId equals i.ProductCategoryId into table2
                             //                     from i in table2.ToList()
                             //                     join pi in _context.ProductItems on i.ProductId equals pi.ProductId into table3
                             //                     from pi in table3.ToList()

                             //                     let data = productItems.Add(new ProductItemViewModel { FileLocation = pi.FileLocation})

                             //                     select new StoreProductsViewModel
                             //                     {
                             //                         ProductItemsViewModel = pi,
                             //                         department = d,
                             //                         incentive = i
                             //                     };
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
                                 ProductId = pr.ProductId
                                 //ProductItemsViewModel = new List<ProductItemViewModel>
                                 //{
                                 //    new ProductItemViewModel { FileLocation = pi.FileLocation },
                                 //}


                                 //} = p.ProductPhotoMaps.Select(pp => pp.FileName).ToList()
                                 // ProductItemViewModel = productItems.Add(new ProductItemViewModel { FileLocation = pi.FileLocation}),
                             }).ToList();

                //var productDetails = 

                // query.ForEach(x=>x.ProductItemsViewModel.Select(x => x.FileLocation == ).ToList()
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
                        var storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=monthlystatement;AccountKey=TiB4RbTOMBFU85N3icORuByCenohH4zhVW644VYYW4O+fCJh8jBxzIE6l9hhlCwCb9lJq0jFDHdQtGe+xl0iAg==;EndpointSuffix=core.windows.net");
                        var blobClient = storageAccount.CreateCloudBlobClient();

                        //CloudBlobContainer container = blobClient.GetContainerReference(options.containerName);
                        CloudBlobContainer container = blobClient.GetContainerReference("socialpay");
                        CloudBlockBlob blob = container.GetBlockBlobReference(image.FileLocation);

                        image.Url = blob.Uri.AbsoluteUri;

                        item.ProductItemsViewModel = getProductsItem;
                    }

                   
                }

                if (query.Count > 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = query };

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Record not found" };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured" };
            }
        }

    }
}
