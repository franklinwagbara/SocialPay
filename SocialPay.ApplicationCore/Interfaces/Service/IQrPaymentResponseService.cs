using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IQrPaymentResponseService
    {
        Task<List<QrPaymentResponseViewModel>> GetAllAsync();
        Task<QrPaymentResponseViewModel> GetTransactionByreference(string reference);
        Task<QrPaymentResponseViewModel> AddAsync(QrPaymentResponseViewModel model);
        Task DeleteAsync(int id);
    }
}
