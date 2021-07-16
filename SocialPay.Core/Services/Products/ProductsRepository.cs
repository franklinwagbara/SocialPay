using Microsoft.EntityFrameworkCore;
using SocialPay.Domain;
using SocialPay.Helper;
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
        public ProductsRepository(SocialPayDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        //public async Task<WebApiResponse> GetProductsByClientId(long clientId)
        //{
        //    try
        //    {
        //        var productCategory = await _context.ProductCategories.Where(x => x.ClientAuthenticationId == clientId).ToListAsync();

        //        var query = (from c in productCategory
        //                     join p in _context.Products on c.ProductCategoryId equals p.ProductCategoryId
        //                     join s in _context.MerchantStore on p.MerchantStoreId equals s.MerchantStoreId

        //                     select new StoreProductsViewModel
        //                     {
        //                         StoreName = s.StoreName,
        //                         ProductName = p.ProductName,
        //                         Price = p.Price,
        //                         Size = p.Size,
        //                         Category = c.CategoryName,
        //                         Color = p.Color,
        //                         Options = p.Options,
        //                         StoreDescription = s.Description,
        //                         ProductDescription = p.Description

        //                     }).ToList();

        //        if (query.Count > 0)
        //            return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success", Data = query };
               
        //        return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Record not found" };
        //    }
        //    catch (Exception ex)
        //    {
        //        return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error occured" };
        //    }
        //}
    }
}
