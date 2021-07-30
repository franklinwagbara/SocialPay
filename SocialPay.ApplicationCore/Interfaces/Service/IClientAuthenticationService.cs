using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Interfaces.Service
{
    public interface IClientAuthenticationService
    {
        Task<List<ClientAuthenticationViewModel>> GetAllAsync();
        Task<ClientAuthenticationViewModel> GetUserByClientIdInfo(long clientId);      
    }
}
