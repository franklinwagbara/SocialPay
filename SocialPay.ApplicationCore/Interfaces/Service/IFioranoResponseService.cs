using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IFioranoResponseService
    {
        Task<List<FioronoBillsPaymentResponseViewModel>> GetAllAsync();
        Task<FioronoBillsPaymentResponseViewModel> GetTransactionByreference(string reference);
        Task<int> CountTotalTransactionAsync();
        Task<bool> ExistsAsync(long clientId);
        Task<FioronoBillsPaymentResponseViewModel> AddAsync(FioronoBillsPaymentResponseViewModel model);
        Task DeleteAsync(int id);
    }
}
