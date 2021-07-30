using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IFioranoRequestService
    {
        Task<List<FioranoRequestViewModel>> GetAllAsync();
        Task<List<FioranoRequestViewModel>> GetTransactionByClient(long clientId);
        Task<FioranoRequestViewModel> GetTransactionByreference(string reference);
        Task<int> CountTotalTransactionAsync();
        Task<bool> ExistsAsync(long clientId);
        Task<FioranoRequestViewModel> AddAsync(FioranoRequestViewModel model);
      //  Task UpdateAsync(FioranoRequestViewModel model);
        Task DeleteAsync(int id);
    }
}
