using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Interface;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Store
{
    public class StoreRepository : IStore
    {
        private APIResponse<MerchantStoreLog> merchantresponse;
        private APIResponse<StoreCategory> categoryresponse;
        private APIResponse<List<ProductOption>> productoptionresponse;
        private readonly SocialPayDbContext _context;
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public StoreRepository(SocialPayDbContext context)
        {
            _context = context;
            merchantresponse = new APIResponse<MerchantStoreLog>();
            categoryresponse = new APIResponse<StoreCategory>();

        }
        public async Task<APIResponse<MerchantStoreLog>> CreateStore([FromBody] MerchantStoreLog storeDto)
        {
            try
            {

                //_context.MerchantStoreRequest.Add(storeDto);
                //await _context.SaveChangesAsync();
                //merchantresponse.Data = storeDto;
                //merchantresponse.Message = "Created successfully";
                //merchantresponse.StatusCode = "00";
            }
            catch (Exception ex)
            {

                merchantresponse.Message = ex.Message;
                merchantresponse.StatusCode = "99";
            }
            return merchantresponse;
        }

        public async Task<APIResponse<StoreCategory>> CreateCategory([FromBody] StoreCategory categoryDto)
        {
            try
            {

              //  _context.StoreCategoryRequest.Add(categoryDto);
                await _context.SaveChangesAsync();
                categoryresponse.Data = categoryDto;
                categoryresponse.Message = "Created successfully";
                categoryresponse.StatusCode = "00";
            }
            catch (Exception ex)
            {
                categoryresponse.Message = ex.Message;
                categoryresponse.StatusCode = "99";
            }
            return categoryresponse;
        }

        public async Task<APIResponse<List<ProductOption>>> CreateProductOption(List<ProductOption> productOption)
        {
            try
            {
                if (productOption != null)
                {
                    foreach (var item in productOption)
                    {
                       // _context.ProductOptionRequest.Add(item);

                    }
                    await _context.SaveChangesAsync();
                    productoptionresponse.Data = productOption;
                    productoptionresponse.Message = "Created successfully";
                    productoptionresponse.StatusCode = "00";
                }

            }
            catch (Exception ex)
            {

                productoptionresponse.Message = ex.Message;
                productoptionresponse.StatusCode = "99";
            }
            return productoptionresponse;
        }
    }

}
