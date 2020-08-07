﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialPay.Core.Services.Account;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;

namespace SocialPay.API.Controllers
{
    [Route("api/socialpay")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly MerchantRegistrationService _merchantRegistrationService;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(AccountsController));

        public AccountsController(MerchantRegistrationService merchantRegistrationService)
        {
            _merchantRegistrationService = merchantRegistrationService;
        }

        [HttpPost]
        [Route("merchant-signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequestDto model)
        {
            _log4net.Info("Tasks starts to create account" + " | " + model.Email + " | " + DateTime.Now);

            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _merchantRegistrationService.CreateNewMerchant(model);
                    if(result.ResponseCode != AppResponseCodes.Success)
                        return BadRequest(result);
                    return Ok(result);
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + model.Email + " | " + ex.Message.ToString() + " | " + DateTime.Now);
                response.ResponseCode = AppResponseCodes.InternalError;
              //  _logger.LogError(ex.Message.ToString() + " | " + ex.InnerException.ToString() + " | " + DateTime.Now);
                return BadRequest(response);
            }
        }


        [HttpPost]
        [Route("signup-confirmation")]
        public async Task<IActionResult> SignUpConfirmation([FromBody] SignUpConfirmationRequestDto model)
        {
            var response = new WebApiResponse { };
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _merchantRegistrationService.ConfirmSignUp(model);
                    if (result.ResponseCode != AppResponseCodes.Success)
                        return BadRequest(result);
                    return Ok(result);
                }
                var message = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                response.ResponseCode = AppResponseCodes.Failed;
                response.Data = message;
                return BadRequest(response);

            }
            catch (Exception ex)
            {
                response.ResponseCode = AppResponseCodes.InternalError;
                //  _logger.LogError(ex.Message.ToString() + " | " + ex.InnerException.ToString() + " | " + DateTime.Now);
                return BadRequest(response);
            }
        }
    }
}