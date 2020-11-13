using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
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
        public async Task<WebApiResponse> InsertNipTransferRequest()
        {
			try
			{
                using (SqlConnection con = new SqlConnection(_appSettings.nipdbConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("FundsTransfer_InsertNIPTransaction", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("@FirstName", SqlDbType.VarChar).Value = "";
                        cmd.Parameters.Add("@LastName", SqlDbType.VarChar).Value = "";

                        con.Open();
                        cmd.ExecuteNonQuery();
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
