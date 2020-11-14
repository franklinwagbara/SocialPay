using BanksServices;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Helper.Dto.Request;
using System;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Validations
{
    public class BankServiceRepositoryJobService
    {
        private readonly AppSettings _appSettings;
        public BankServiceRepositoryJobService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public async Task<string> LockAccountWithReasonAsync(LockAccountRequestDto model)
        {
            try
            {

                var banks = new banksSoapClient(banksSoapClient.EndpointConfiguration.banksSoap, _appSettings.BankServiceUrl);

                var getUserInfo = await banks.LockAmountWithReasonAsync(model.acct, model.sDate, model.eDate, 
                    model.amt, model.reasonForLocking);
                // var bankService = new banksSoapClient(banksSoapClient.EndpointConfiguration.banksSoap, ServicesPoint.CoreBanking);
                var result = getUserInfo.LockAmountWithReasonResult;            

                return result;
            }
            catch (Exception ex)
            {

                return "Error";
            }

        }

    }
}
