using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface INibbsQrMerchantResponseService
    {
        Task<CreateNibsMerchantQrCodeResponse> GetMerchantInfo(long clientId);
        Task<int> CountTotalMerchantsAsync();
        Task<bool> ExistsAsync(long clientId);
        Task DeleteAsync(int id);
    }
}
