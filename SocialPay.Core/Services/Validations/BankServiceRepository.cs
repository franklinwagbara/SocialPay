using BanksServices;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Validations
{
    public class BankServiceRepository
    {
        private readonly AppSettings _appSettings;
        public BankServiceRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }


        public async Task<AccountInfoViewModel> BvnValidation(string bvn, string dateOfbirth)
        {
            try
            {
                var banksServices = new banksSoapClient(banksSoapClient.EndpointConfiguration.banksSoap, _appSettings.BankServiceUrl);
                var validatebvn = await banksServices.GetBvnAsync(bvn);
                var validateNode = validatebvn.Nodes[1];
                var bvnDetails = validateNode.Descendants("Customer")

                    .Select(b => new BvnViewModel
                    {
                        Bvn = b.Element("Bvn")?.Value,
                        FirstName = b.Element("FirstName")?.Value,
                        LastName = b.Element("LastName")?.Value,
                        PhoneNumber = b.Element("PhoneNumber")?.Value,
                        DateOfBirth = b.Element("DateOfBirth")?.Value,
                        MiddleName = b.Element("MiddleName")?.Value,

                    }).FirstOrDefault();
                if (bvnDetails == null)
                    return new AccountInfoViewModel { ResponseCode = AppResponseCodes.InvalidBVN };
                if (bvnDetails.Bvn.Contains("exit"))
                    return new AccountInfoViewModel { ResponseCode = AppResponseCodes.InvalidBVN };

                //if (!bvnDetails.DateOfBirth.Equals(dateOfbirth))
                //    return new AccountInfoViewModel { ResponseCode = AppResponseCodes.InvalidBVNDateOfBirth };
                return new AccountInfoViewModel { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {

                return new AccountInfoViewModel { ResponseCode = AppResponseCodes.InvalidBVN };
            }
        }

        public async Task<AccountInfoViewModel> GetAccountFullInfoAsync(string nuban, string bvn)
        {
            try
            {
                //var validateBvn = await BvnValidation(bvn, "");
                //if (validateBvn.ResponseCode != AppResponseCodes.Success)
                //    return validateBvn;
                var banks = new banksSoapClient(banksSoapClient.EndpointConfiguration.banksSoap, _appSettings.BankServiceUrl);

                //var validatebvn = await banks.GetBvnAsync(bvn);

                var getUserInfo = await banks.getAccountFullInfoAsync(nuban);

                // var bankService = new banksSoapClient(banksSoapClient.EndpointConfiguration.banksSoap, ServicesPoint.CoreBanking);
                var validAccount = getUserInfo.Nodes[1];
                var accountDetail = validAccount.Descendants("BankAccountFullInfo")

                    .Select(b => new AccountInfoViewModel
                    {
                        BRA_CODE = b.Element("BRA_CODE")?.Value,
                        ACCT_NO = b.Element("ACCT_NO")?.Value,
                        CUS_SHO_NAME = b.Element("CUS_SHO_NAME")?.Value,
                        WorkingBalance = b.Element("WorkingBalance")?.Value,
                        CUR_CODE = b.Element("CUR_CODE")?.Value,
                        STA_CODE = b.Element("STA_CODE")?.Value,
                        REST_FLAG = b.Element("REST_FLAG")?.Value,
                        T24_LED_CODE = b.Element("T24_LED_CODE")?.Value,
                        CUS_NUM = b.Element("CUS_NUM")?.Value,
                    }).FirstOrDefault();

                if (accountDetail == null)
                    return new AccountInfoViewModel
                    {
                        ResponseCode = AppResponseCodes.InvalidAccountNo,
                        NUBAN = nuban

                    };
                if (accountDetail.STA_CODE != "ACTIVE")
                    return new AccountInfoViewModel { ResponseCode = AppResponseCodes.InActiveAccountNumber, NUBAN = nuban };
                accountDetail.ResponseCode = AppResponseCodes.Success;
                return accountDetail;
            }
            catch (Exception ex)
            {

                return new AccountInfoViewModel { ResponseCode = AppResponseCodes.InternalError };
            }

        }

    }
}
