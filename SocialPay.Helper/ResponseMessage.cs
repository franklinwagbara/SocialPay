using System;
using System.Collections.Generic;
using System.Text;

namespace SocialPay.Helper
{
    public class ResponseMessage
    {
        public const string Success = "Request was successful";
        public const string InternalError = "Error occured. Please try again later";
        public const string RecordNotFound = "Record Not Found";
        public const string OtpExpired = "Your otp has expired";
        public const string SendMailFailed = "Error occured wheile sending email. Please try again";
        public const string DuplicateRecord = "Duplicate records. Records exist.";
        public const string BusinessInfoRequired = "Request failed. Seems you have not completed your business info profile.";
        public const string BvnValidation = "BVN validation failed. Please ensure you entered valid BVN details";
        public const string InterBankNameEnquiryFailed = "Interbank Name Enquiry Failed";
        public const string TINValidationError = "Error occured validating TIN";
        public const string IncompleteMerchantProfile = "Incomplete profile. Please complete your sign up process";

    }
}
