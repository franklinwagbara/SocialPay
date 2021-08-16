using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IUssdRequestLogService
    {
        Task<List<UssdRequestViewModel>> GetAllAsync();
        Task<List<UssdRequestViewModel>> GetUssdByClientId(long clientId);    
        Task<int> CountTotalTransactionAsync();
        Task<bool> ExistsAsync(long clientId);
        Task<UssdRequestViewModel> AddAsync(UssdRequestViewModel model);
        Task UpdateAsync(UssdRequestViewModel model);
        Task DeleteAsync(int id);
    }
}
