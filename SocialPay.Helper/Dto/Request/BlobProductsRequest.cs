using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SocialPay.Helper.Dto.Request
{
    public class BlobProductsRequest
    {
        public Stream ImageStream { get; set; }
        public string ProductName { get; set; }        
        public string StoreName { get; set; }
        public string RequestType { get; set; }
        public List<DefaultDocumentRequest> ImageDetail { get; set; }
        public long ClientId { get; set; }
    }

    public class DefaultDocumentRequest
    {
        public IFormFile Image { get; set; }
        public string ImageGuidId { get; set; }
        public string FileLocation { get; set; }
    }

    public class BlobStoreRequest : DefaultDocumentRequest
    {
        public Stream ImageStream { get; set; }
        public string StoreName { get; set; }
        public string RequestType { get; set; }
        public long ClientId { get; set; }
    }
}
