﻿using BanksServices;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.SerilogService.FioranoT24;
using SocialPay.Helper.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Validations
{
    public class BankServiceRepositoryJobService
    {
        private readonly AppSettings _appSettings;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(BankServiceRepositoryJobService));
        private readonly FioranoT24Logger _fioranoT24Logger;
        public BankServiceRepositoryJobService(IOptions<AppSettings> appSettings, FioranoT24Logger fioranoT24Logger)
        {
            _appSettings = appSettings.Value;
            _fioranoT24Logger = fioranoT24Logger;
        }
        public async Task<string> LockAccountWithReasonAsync(LockAccountRequestDto model)
        {
            _fioranoT24Logger.LogRequest($"{"Job Service"}{"-"}{"LockAccountWithReasonAsync"}{" | "}{model.acct}{ " | "}{model.amt}{" | "}{model.eDate}{" | "}{ model.reasonForLocking}{" | "}{model.sDate}{" | "}{DateTime.Now}");

            try
            {

                var banks = new banksSoapClient(banksSoapClient.EndpointConfiguration.banksSoap, _appSettings.BankServiceUrl);

                var getUserInfo = await banks.LockAmountWithReasonAsync(model.acct, model.sDate, model.eDate, 
                    model.amt, model.reasonForLocking);
                // var bankService = new banksSoapClient(banksSoapClient.EndpointConfiguration.banksSoap, ServicesPoint.CoreBanking);
                var result = getUserInfo.LockAmountWithReasonResult;

                var response = result.Split("|")[1];

                _fioranoT24Logger.LogRequest($"{"Job Service"}{ "-"}{ "LockAccountWithReasonAsync response"}{ " | "}{model.acct}{" | "}{model.amt}{" | "}{ result}{ " | "}{ DateTime.Now}");

                return response;
            }
            catch (Exception ex)
            {
                _fioranoT24Logger.LogRequest($"{"Job Service"}{"-" + "Error occured"}{" | "}{"BankServiceRepositoryJobService"}{" | "}{model.acct}{" | "}{model.amt}{" | "}{ex.Message.ToString()}{" | "}{DateTime.Now}");

                return "Error";
            }

        }


        public async Task<AccountInfoViewModel> GetAccountFullInfoAsync(string nuban, decimal amount)
        {
            try
            {
                _fioranoT24Logger.LogRequest($"{"Initiating GetAccountFullInfoAsync request"}{" | "}{amount}{" | "}{nuban}{" | "}{ DateTime.Now}");

                var banks = new banksSoapClient(banksSoapClient.EndpointConfiguration.banksSoap, _appSettings.BankServiceUrl);

                var getUserInfo = await banks.getAccountFullInfoAsync(nuban);

                var validAccount = getUserInfo.Nodes[1];
                _fioranoT24Logger.LogRequest($"{"Initiating GetAccountFullInfoAsync response"}{" | " }{amount}{" | "}{nuban}{" | "}{validAccount}{" | "}{DateTime.Now}");

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
                        UsableBal = b.Element("UsableBal")?.Value,
                    }).FirstOrDefault();

                if (accountDetail == null)
                {
                    _fioranoT24Logger.LogRequest($"{"Invalid account"}{ " | "}{ amount}{" | "}{nuban}{" | "}{validAccount}{" | "}{DateTime.Now}");

                    return new AccountInfoViewModel
                    {
                        ResponseCode = AppResponseCodes.InvalidAccountNo,
                        NUBAN = nuban
                    };
                }


                decimal usableBalance = Convert.ToDecimal(accountDetail.UsableBal);

                //if (usableBalance < amount)
                //{
                //    _fioranoT24Logger.LogRequest($"{"Insufficient funds"}{" | "}{amount}{" | "}{nuban}{" | "}{validAccount}{" | "}{usableBalance}{"-"}{DateTime.Now}");

                //    return new AccountInfoViewModel { ResponseCode = AppResponseCodes.InsufficientFunds, NUBAN = nuban, UsableBal = accountDetail.UsableBal };
                //}

                accountDetail.ResponseCode = AppResponseCodes.Success;

                return accountDetail;
            }
            catch (Exception ex)
            {
                _fioranoT24Logger.LogRequest($"{"Error occured"}{" | "}{"GetAccountFullInfoAsync"}{" | "}{amount}{" | "}{ex.Message.ToString()}{" | "}{DateTime.Now}",true);

                return new AccountInfoViewModel { ResponseCode = AppResponseCodes.InternalError };
            }

        }
    }
}
