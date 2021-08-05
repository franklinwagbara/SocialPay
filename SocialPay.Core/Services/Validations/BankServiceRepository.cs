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
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(BankServiceRepository));

        public BankServiceRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        private AgeCalculatorViewModel CalculateAge(DateTime Dob)
        {
            DateTime Now = DateTime.Now;
            int Years = new DateTime(DateTime.Now.Subtract(Dob).Ticks).Year - 1;
            DateTime PastYearDate = Dob.AddYears(Years);
            int Months = 0;
            for (int i = 1; i <= 12; i++)
            {
                if (PastYearDate.AddMonths(i) == Now)
                {
                    Months = i;
                    break;
                }
                else if (PastYearDate.AddMonths(i) >= Now)
                {
                    Months = i - 1;
                    break;
                }
            }
            int Days = Now.Subtract(PastYearDate.AddMonths(Months)).Days;
            int Hours = Now.Subtract(PastYearDate).Hours;
            int Minutes = Now.Subtract(PastYearDate).Minutes;
            int Seconds = Now.Subtract(PastYearDate).Seconds;

            var response = new AgeCalculatorViewModel
            {
                Year = Years,
                Month = Months,
                Days = Days
            };

            return response;
            //return String.Format("Age: {0} Year(s) {1} Month(s) {2} Day(s) {3} Hour(s) {4} Second(s)",
            //                    Years, Months, Days, Hours, Seconds);
        }

        public async Task<AccountInfoViewModel> BvnValidation(string bvn, string dateOfbirth, string fullname = "")
        {
            try
            {
                _log4net.Info("Initiating BvnValidation request" + " | " + bvn + " | " + dateOfbirth + " | " + DateTime.Now);

                var currentDob = DateTime.Parse(dateOfbirth).ToString("dd-MMM-yy");

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

                if (!bvnDetails.DateOfBirth.Equals(currentDob))
                    return new AccountInfoViewModel { ResponseCode = AppResponseCodes.InvalidBVNDateOfBirth };

                var dob = Convert.ToDateTime(bvnDetails.DateOfBirth);

                int ageLimit = Convert.ToInt32(_appSettings.ageLimit);

                var currentAge = CalculateAge(dob);

                //if(!fullname.Equals($"{bvnDetails.FirstName}{""}{bvnDetails.LastName}"))
                //    return new AccountInfoViewModel { ResponseCode = AppResponseCodes.InvalidBVNDetails };

                if (currentAge.Year >= ageLimit)
                    return new AccountInfoViewModel { ResponseCode = AppResponseCodes.Success };              

                return new AccountInfoViewModel { ResponseCode = AppResponseCodes.AgeNotWithinRange };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "Bvn Validation" + " | " + bvn + " | "+ ex + " | " + DateTime.Now);

                return new AccountInfoViewModel { ResponseCode = AppResponseCodes.BvnValidationError };
            }
        }

        public async Task<AccountInfoViewModel> GetAccountFullInfoAsync(string nuban, string bvn)
        {
            try
            {
                _log4net.Info("Initiating GetAccountFullInfoAsync request" + " | " + bvn + " | " + nuban + " | " + DateTime.Now);

                //var validateBvn = await BvnValidation(bvn, "");
                //if (validateBvn.ResponseCode != AppResponseCodes.Success)
                //    return validateBvn;
                var banks = new banksSoapClient(banksSoapClient.EndpointConfiguration.banksSoap, _appSettings.BankServiceUrl);

                //var validatebvn = await banks.GetBvnAsync(bvn);

                var getUserInfo = await banks.getAccountFullInfoAsync(nuban);

                // var bankService = new banksSoapClient(banksSoapClient.EndpointConfiguration.banksSoap, ServicesPoint.CoreBanking);
                var validAccount = getUserInfo.Nodes[1];
                _log4net.Info("Initiating GetAccountFullInfoAsync response" + " | " + bvn + " | " + nuban + " | " + validAccount + " | "+ DateTime.Now);

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
                _log4net.Error("Error occured" + " | " + "GetAccountFullInfoAsync" + " | " + bvn + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new AccountInfoViewModel { ResponseCode = AppResponseCodes.InternalError };
            }

        }

    }
}
