using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IEventLogRequestService
    {
        Task<List<EventLogViewModel>> GetAllAsync();
        Task<List<EventLogViewModel>> GetEventByClient(long clientId);
        Task<EventLogViewModel> GetEventByUserId(string userId);
        Task<int> CountTotalTransactionAsync();
        Task<bool> ExistsAsync(long clientId);
        Task<EventLogViewModel> AddAsync(EventLogViewModel model);
        Task DeleteAsync(int id);
    }
}
