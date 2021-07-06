using SocialPay.Domain.Entities;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Interface
{
    public interface IStore
    {
        Task<APIResponse<MerchantStoreLog>> CreateStore(MerchantStoreLog storeDto);

        Task<APIResponse<StoreCategory>> CreateCategory(StoreCategory categoryDto);

        Task<APIResponse<List<ProductOption>>> CreateProductOption(List<ProductOption> categoryDto);
    }

}
