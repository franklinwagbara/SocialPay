using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IQrPaymentRequestService
    {
        Task<List<QrRequestPaymentViewModel>> GetAllAsync();
        Task<QrRequestPaymentViewModel> GetTransactionByreference(string reference);
        Task<int> CountTotalTransactionAsync();
        Task<bool> ExistsAsync(long clientId);
        Task<QrRequestPaymentViewModel> AddAsync(QrRequestPaymentViewModel model);
        Task DeleteAsync(int id);
    }
}
