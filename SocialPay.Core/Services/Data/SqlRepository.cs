using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using System;
using System.Data;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Data
{
    public class SqlRepository
    {
        private readonly AppSettings _appSettings;
        public SqlRepository(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public async Task<WebApiResponse> InsertNipTransferRequest(NipFundstransferRequestDto model)
        {
			try
			{
                using (SqlConnection con = new SqlConnection(_appSettings.nipdbConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FundsTransfer_InsertNIPTransaction", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@NESessionID", SqlDbType.Char).Value = model.NESessionID;
                        cmd.Parameters.Add("@LedCodeVal", SqlDbType.VarChar).Value = model.LedCodeVal;
                        cmd.Parameters.Add("@AccountLockID", SqlDbType.VarChar).Value = model.AccountLockID;
                        cmd.Parameters.Add("@AccountName", SqlDbType.VarChar).Value = model.AccountName;
                        cmd.Parameters.Add("@AccountNumber", SqlDbType.VarChar).Value = model.AccountNumber;
                        cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = model.Amount;
                        cmd.Parameters.Add("@AppID", SqlDbType.Int).Value = model.AppID;
                        cmd.Parameters.Add("@BeneficiaryBankVerificationNumber", SqlDbType.VarChar).Value = model.BeneficiaryBankVerificationNumber;
                        cmd.Parameters.Add("@BeneficiaryKYCLevel", SqlDbType.VarChar).Value = model.BeneficiaryKYCLevel;
                        cmd.Parameters.Add("@BraCodeVal", SqlDbType.VarChar).Value = model.BraCodeVal;
                        cmd.Parameters.Add("@CurCodeVal", SqlDbType.VarChar).Value = model.CurCodeVal;
                        cmd.Parameters.Add("@CusNumVal", SqlDbType.VarChar).Value = model.CusNumVal;
                        cmd.Parameters.Add("@Fee", SqlDbType.Decimal).Value = model.Fee;
                        cmd.Parameters.Add("@Vat", SqlDbType.Decimal).Value = model.Vat;
                        cmd.Parameters.Add("@OrignatorName", SqlDbType.VarChar).Value = model.OrignatorName;
                        cmd.Parameters.Add("@OriginatorAccountNumber", SqlDbType.VarChar).Value = model.OriginatorAccountNumber;
                        cmd.Parameters.Add("@OriginatorBankVerificationNumber", SqlDbType.VarChar).Value = model.OriginatorBankVerificationNumber;
                        cmd.Parameters.Add("@PaymentRef", SqlDbType.VarChar).Value = model.PaymentRef;
                        cmd.Parameters.Add("@OriginatorKYCLevel", SqlDbType.VarChar).Value = model.OriginatorKYCLevel;
                        cmd.Parameters.Add("@ChannelCode", SqlDbType.TinyInt).Value = model.ChannelCode;
                        await con.OpenAsync();
                        await cmd.ExecuteNonQueryAsync();
                        await con.CloseAsync();
                        return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                    }
                }
            }
			catch (Exception ex)
			{
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
