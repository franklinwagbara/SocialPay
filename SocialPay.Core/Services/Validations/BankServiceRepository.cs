using BanksServices;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Core.Services.EventLogs;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.ViewModel;
using System;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Validations
{
    public class BankServiceRepository
    {
        private readonly AppSettings _appSettings;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(BankServiceRepository));
        private readonly EventLogService _eventLogService;
        public readonly BasicHttpBinding _basicHttpBinding;
        public readonly EndpointAddress endpointAddress;
        public BankServiceRepository(IOptions<AppSettings> appSettings, EventLogService eventLogService)
        {
            _appSettings = appSettings.Value;
            _eventLogService = eventLogService ?? throw new ArgumentNullException(nameof(eventLogService));

            endpointAddress = new EndpointAddress(_appSettings.BankServiceUrl);

            _basicHttpBinding = new BasicHttpBinding(endpointAddress.Uri.Scheme.ToLower() == "http" ?
                           BasicHttpSecurityMode.None : BasicHttpSecurityMode.Transport);

            _basicHttpBinding.OpenTimeout = TimeSpan.FromMinutes(Convert.ToInt32(_appSettings.OpenTimeout));
            _basicHttpBinding.CloseTimeout = TimeSpan.FromMinutes(Convert.ToInt32(_appSettings.CloseTimeout));
            _basicHttpBinding.ReceiveTimeout = TimeSpan.FromMinutes(Convert.ToInt32(_appSettings.ReceiveTimeout));
            _basicHttpBinding.SendTimeout = TimeSpan.FromMinutes(Convert.ToInt32(_appSettings.SendTimeout));
            _basicHttpBinding.MaxReceivedMessageSize = Convert.ToInt32(_appSettings.MaxReceivedMessageSize);
            _basicHttpBinding.MaxBufferSize = Convert.ToInt32(_appSettings.MaxBufferSize);
            _basicHttpBinding.MaxBufferPoolSize = Convert.ToInt32(_appSettings.MaxBufferPoolSize);
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

        public async Task<AccountInfoViewModel> BvnValidation(string bvn, string dateOfbirth, string firstname, 
            string lastname, string email)
        {
            DateTime DOB = DateTime.Now;
            string day = ""; string month = ""; string yr = "";
            try

            {
                _log4net.Info("Initiating BvnValidation request" + " | " + bvn + " | " + dateOfbirth + " | " + DateTime.Now);
                firstname = firstname.ToLower(); lastname = lastname.ToLower();
                var eventLog = new EventRequestDto
                {
                    ModuleAccessed = EventLogProcess.BvnValidation,
                    Description = "Bvn Validation request",
                    UserId = email
                };
                var dateValues = new[] { dateOfbirth, dateOfbirth, dateOfbirth, dateOfbirth, dateOfbirth, dateOfbirth };
                var formats = new[] { "dd/MM/yyyy", "dd-MM-yyyy", "MM/dd/yyyy", "MM-dd-yyyy", "yyyy-MM-dd", "yyyy/MM/dd" };

                foreach (var s in dateValues)
                {

                    if (DateTime.TryParseExact(s, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DOB) == true)
                    {
                        day = Convert.ToDateTime(DOB).ToString("dd");
                        month = Convert.ToDateTime(DOB).ToString("MMM");
                        yr = Convert.ToDateTime(DOB).ToString("yy");
                    }
                }

                var currentDob = day + "-" + month + "-" + yr;

                //var currentDob = DateTime.Parse(dateOfbirth).ToString("dd-MMM-yy");.
                // var banksServices = new banksSoapClient(banksSoapClient.EndpointConfiguration.banksSoap, _appSettings.BankServiceUrl);

                var banksServices = new banksSoapClient(_basicHttpBinding, endpointAddress);
                var validatebvn = await banksServices.GetBvnAsync(bvn);
                var validateNode = validatebvn.Nodes[1];
                var bvnDetails = validateNode.Descendants("Customer")

                    .Select(b => new BvnViewModel
                    {
                        Bvn = b.Element("Bvn")?.Value,
                        FirstName = b.Element("FirstName")?.Value.ToLower(),
                        LastName = b.Element("LastName")?.Value.ToLower(),
                        PhoneNumber = b.Element("PhoneNumber")?.Value,
                        DateOfBirth = b.Element("DateOfBirth")?.Value,
                        MiddleName = b.Element("MiddleName")?.Value,

                    }).FirstOrDefault();

                
                if (bvnDetails == default)
                    return new AccountInfoViewModel { ResponseCode = AppResponseCodes.InvalidBVN, Message = "Invalid BVN" };

                if (bvnDetails.Bvn.Contains("exit"))
                    return new AccountInfoViewModel { ResponseCode = AppResponseCodes.InvalidBVN, Message = "BVN does not exist" };

                if (!bvnDetails.DateOfBirth.Equals(currentDob))//
                    return new AccountInfoViewModel { ResponseCode = AppResponseCodes.InvalidBVNDateOfBirth, Message = "Invalid Date of birth" };

                var dob = Convert.ToDateTime(bvnDetails.DateOfBirth);

                int ageLimit = Convert.ToInt32(_appSettings.ageLimit);

                var currentAge = CalculateAge(dob);

                if (!firstname.Equals($"{bvnDetails.FirstName.ToLower()}") || !lastname.Equals($"{bvnDetails.LastName.ToLower()}"))
                    return new AccountInfoViewModel { ResponseCode = AppResponseCodes.InvalidBVNDetailsFirstNameOrLastName, Message = "Invalid firstname or lastname" };

                if (currentAge.Year >= ageLimit)
                    return new AccountInfoViewModel { ResponseCode = AppResponseCodes.Success, Message = "Success" };              

                return new AccountInfoViewModel { ResponseCode = AppResponseCodes.AgeNotWithinRange, Message = "Age specified not supported" };

               // return new AccountInfoViewModel { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "Bvn Validation" + " | " + bvn + " | "+ ex + " | " + DateTime.Now);

                return new AccountInfoViewModel { ResponseCode = AppResponseCodes.BvnValidationError, Message = "Error occured while validating BVN" };
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
