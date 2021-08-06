using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Microsoft.Extensions.Configuration;
using SocialPay.Core.Configurations;
using SocialPay.Helper.Dto.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.AzureBlob
{
    public class BlobService
    {
        public IConfiguration Configuration { get; }
        public BlobService(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public async Task UploadProducts(BlobProductsRequest request)
        {
            try
            {
                var options = Configuration.GetSection(nameof(AzureBlobConfiguration)).Get<AzureBlobConfiguration>();

                foreach (var item in request.ImageDetail)
                {
                    using var ms = new MemoryStream();
                    item.Image.CopyTo(ms);
                    ms.Position = 0;

                    string containerName = options.containerName;
                    string RequestType = request.RequestType;
                    string merchant = Convert.ToString(request.ClientId);
                    var storageaccount = options.blobConnectionstring;

                    var storageAccountString = new BlobServiceClient(storageaccount);

                    BlobContainerClient containerBlob = storageAccountString.GetBlobContainerClient(containerName);

                    await containerBlob.CreateIfNotExistsAsync(PublicAccessType.Blob);

                    BlockBlobClient blockBlob = containerBlob.GetBlockBlobClient(item.FileLocation);

                    await blockBlob.UploadAsync(ms);

                }

            }
            catch (Exception ex)
            { }
        }

        public async Task UploadStore(BlobStoreRequest request)
        {
            try
            {
                var options = Configuration.GetSection(nameof(AzureBlobConfiguration)).Get<AzureBlobConfiguration>();

                using var ms = new MemoryStream();
                request.Image.CopyTo(ms);
                ms.Position = 0;

                var storageAccountString = new BlobServiceClient(options.blobConnectionstring);

                BlobContainerClient containerBlob = storageAccountString.GetBlobContainerClient(options.containerName);

                await containerBlob.CreateIfNotExistsAsync(PublicAccessType.Blob);

                BlockBlobClient blockBlob = containerBlob.GetBlockBlobClient(request.FileLocation);

                await blockBlob.UploadAsync(ms);

            }
            catch (Exception ex)
            { }
        }

        public async Task UploadCSV(BlobOnboardingCSVRequest request)
        {
            try
            {
                var options = Configuration.GetSection(nameof(AzureBlobConfiguration)).Get<AzureBlobConfiguration>();

                using var ms = new MemoryStream();
                request.Image.CopyTo(ms);
                ms.Position = 0;

                var storageAccountString = new BlobServiceClient(options.blobConnectionstring);

                BlobContainerClient containerBlob = storageAccountString.GetBlobContainerClient(options.containerName);

                await containerBlob.CreateIfNotExistsAsync(PublicAccessType.Blob);

                BlockBlobClient blockBlob = containerBlob.GetBlockBlobClient(request.FileLocation);

                var c = await blockBlob.UploadAsync(ms);

            }
            catch (Exception ex)
            { }
        }

    }
}
