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
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(BankServiceRepositoryJobService));
        public BankServiceRepositoryJobService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public async Task<string> LockAccountWithReasonAsync(LockAccountRequestDto model)
        {
            _log4net.Info("Job Service" + "-" + "LockAccountWithReasonAsync" + " | " + model.acct + " | " + model.amt + " | " + model.eDate + " | " + model.reasonForLocking + " | " + model.sDate + " | " +  DateTime.Now);

            try
            {

                var banks = new banksSoapClient(banksSoapClient.EndpointConfiguration.banksSoap, _appSettings.BankServiceUrl);

                var getUserInfo = await banks.LockAmountWithReasonAsync(model.acct, model.sDate, model.eDate, 
                    model.amt, model.reasonForLocking);
                // var bankService = new banksSoapClient(banksSoapClient.EndpointConfiguration.banksSoap, ServicesPoint.CoreBanking);
                var result = getUserInfo.LockAmountWithReasonResult;
                _log4net.Info("Job Service" + "-" + "LockAccountWithReasonAsync response" + " | " + model.acct + " | " + model.amt + " | " + result + " | " + DateTime.Now);

                return result;
            }
            catch (Exception ex)
            {
                _log4net.Error("Job Service" + "-" + "Error occured" + " | " + "BankServiceRepositoryJobService" + " | " + model.acct + " | " + model.amt + " | "+ ex.Message.ToString() + " | " + DateTime.Now);

                return "Error";
            }

        }

    }
}
