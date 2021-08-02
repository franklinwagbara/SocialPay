using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface INibbsQrSubMerchantResponseService
    {
        Task<List<SubMerchantQrResponseViewModel>> GetAllAsync();
        Task<SubMerchantQrResponseViewModel> GetMerchantInfo(long clientId);
    }
}
