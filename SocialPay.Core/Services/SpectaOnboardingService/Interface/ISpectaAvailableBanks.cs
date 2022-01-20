using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Interface
{
    public interface ISpectaAvailableBanks
    {
        public Task<WebApiResponse> AvailableBanksList(string email);

    }
}
