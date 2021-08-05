using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IWebHookTransactionRequestService
    {
        Task<List<WebHookTransactionRequestViewModel>> GetAllAsync();
        //Task<List<WebHookTransactionRequestViewModel>> GetTransactionByClient(long clientId);
        Task<WebHookTransactionRequestViewModel> GetTransactionByreference(string reference);
        Task<int> CountTotalTransactionAsync();
        Task<bool> ExistsAsync(long clientId);
        Task<WebHookTransactionRequestViewModel> AddAsync(WebHookTransactionRequestViewModel model);
        Task DeleteAsync(int id);
    }
}
