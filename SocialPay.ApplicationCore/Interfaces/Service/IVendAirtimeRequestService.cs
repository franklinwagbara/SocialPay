using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IVendAirtimeRequestService
    {
        Task<List<VendAirtimeViewModel>> GetAllAsync();
        Task<List<VendAirtimeViewModel>> GetAirtimeByClient(long clientId);
        Task<VendAirtimeViewModel> GetAirtimeByReference(string reference);
        Task<int> CountTotalTransactionAsync();
        Task<bool> ExistsAsync(long clientId);
        Task<VendAirtimeViewModel> AddAsync(VendAirtimeViewModel model);
        Task DeleteAsync(int id);
    }
}
