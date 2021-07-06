﻿using SocialPay.Domain.Entities;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Interface
{
    public interface IStore
    {
        Task<APIResponse<MerchantStoreDto>> CreateStore(MerchantStoreDto storeDto);

        Task<APIResponse<StoreCategoryDto>> CreateCategory(StoreCategoryDto categoryDto);

        Task<APIResponse<List<ProductOptionDto>>> CreateProductOption(List<ProductOptionDto> categoryDto);
    }

}
