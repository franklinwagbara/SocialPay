using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Interface;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.Core.Store
{
    public class StoreRepositoryOld : IStore
    {
        private APIResponse<MerchantStoreDto> merchantresponse;
        private APIResponse<StoreCategoryDto> categoryresponse;
        private APIResponse<List<ProductOptionDto>> productoptionresponse;
        private readonly SocialPayDbContext _context;
        private static readonly log4net.ILog Logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public StoreRepositoryOld(SocialPayDbContext context)
        {
            _context = context;
            merchantresponse = new APIResponse<MerchantStoreDto>();
            categoryresponse = new APIResponse<StoreCategoryDto>();

        }
        public async Task<APIResponse<MerchantStoreDto>> CreateStore([FromBody] MerchantStoreDto storeDto)
        {
            try
            {

                var merchantlog = new MerchantStoreLog { };
                merchantlog.Name = storeDto.Name;
                merchantlog.Description = storeDto.Description;
                merchantlog.Url = storeDto.Url;
                merchantlog.Image = storeDto.Image;
                merchantlog.Price = storeDto.Price;
                merchantlog.CategoryId = storeDto.CategoryId;
                merchantlog.OptionId = storeDto.OptionId;

                await _context.MerchantStoreRequest.AddAsync(merchantlog);
                await _context.SaveChangesAsync();

                merchantresponse.Data = storeDto;
                merchantresponse.Message = "Created successfully";
                merchantresponse.StatusCode = "00";
            }
            catch (Exception ex)
            {

                merchantresponse.Message = ex.Message;
                merchantresponse.StatusCode = "99";
            }

            return merchantresponse;
        }

        public async Task<APIResponse<StoreCategoryDto>> CreateCategory([FromBody] StoreCategoryDto categoryDto)
        {
            try
            {
                var storecategory = new StoreCategory { };

                storecategory.Name = categoryDto.Name;

               // await _context.StoreCategoryRequest.AddAsync(storecategory);
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


        public async Task<APIResponse<List<ProductOptionDto>>> CreateProductOption(List<ProductOptionDto> productOption)
        {
            try
            {
                var prodoption = new ProductOption { };

                if (productOption != null)
                {
                    foreach (var item in productOption)
                    {
                        prodoption.Name = item.Name;

                       // await _context.ProductOptionRequest.AddAsync(prodoption);
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
