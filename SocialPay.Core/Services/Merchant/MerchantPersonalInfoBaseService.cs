using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Merchant
{
    public class MerchantPersonalInfoBaseService
    {
        private readonly IPersonalInfoService _personalInfoService;

        public MerchantPersonalInfoBaseService(IPersonalInfoService personalInfoService)
        {
            _personalInfoService = personalInfoService ?? throw new ArgumentNullException(nameof(personalInfoService));
        }

        public async Task<WebApiResponse> GetMerchantPersonalInfoAsync(long clientId)
        {
           // clientId = 90;
            try
            {
                var request = await _personalInfoService.GetMerchantPersonalInfo(clientId);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = request };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "Error occured" };
            }
        }


        public async Task<WebApiResponse> UpdateMerchantPersonalInfo(long clientId, UpdateMerchantPersonalInfoRequestDto personalInfo)
        {
            clientId = 90;
            try
            {
                var getClient = await _personalInfoService.GetMerchantPersonalInfo(clientId);

                if(getClient == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Data = "Record not found" };

                var model = new PersonalInfoViewModel 
                {
                    Bvn = getClient.Bvn,
                    Email = getClient.Email,                
                    PhoneNumber = getClient.PhoneNumber
                };

                if (personalInfo.Email != getClient.Email)
                {
                    var validateEmail = await _personalInfoService.GetMerchantPersonalEmailInfo(personalInfo.Email);

                    if (validateEmail != null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateEmail, Data = "Duplicate Email" };

                    model.Email = personalInfo.Email;
                }

                if (personalInfo.PhoneNumber != getClient.PhoneNumber)
                {
                    var validatePhoneNumber = await _personalInfoService.GetMerchantPersonalPhoneNumberInfo(personalInfo.PhoneNumber);

                    if (validatePhoneNumber != null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateMerchantDetails, Data = "Duplicate Merchant Details" };
                    
                    model.PhoneNumber = personalInfo.PhoneNumber;
                }

                if (personalInfo.Bvn != getClient.Bvn)
                {
                    var validateBvn = await _personalInfoService.GetMerchantPersonalBvnInfo(personalInfo.Bvn);

                    if (validateBvn != null)
                        return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateMerchantDetails, Data = "Duplicate Merchant Details" };

                    model.Bvn = personalInfo.Bvn;
                }
                //customer.Features1 = request.customerJourney.Features1 == string.Empty ? string.Empty : request.customerJourney.Features1;

                //getClient.Bvn = model.Bvn == string.Empty ? getClient.Bvn : 

                await _personalInfoService.UpdateAsync(model);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = "Update was successful" };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = "Error occured" };
            }
        }
    }
}
