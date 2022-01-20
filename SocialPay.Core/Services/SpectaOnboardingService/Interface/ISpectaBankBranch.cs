using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.SpectaOnboardingService.Interface
{
    public interface ISpectaBankBranch
    {
        public Task<WebApiResponse> BankBranchList(string email);
    }
}
