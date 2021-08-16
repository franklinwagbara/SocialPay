using SocialPay.Helper.ViewModel;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace SocialPay.Core.Extensions.Common
{
    public static class IdentityExtensions
    {
        public static UserDetailsViewModel GetSessionDetails(this IPrincipal principal)
        {
            try
            {
                var identity = (ClaimsIdentity)principal.Identity;

                //var clientName = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                //var role = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                //var clientId = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                var getData = new UserDetailsViewModel
                {
                    Email = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                    UserID = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                    ClientId = Convert.ToInt32(identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value),
                    Role = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,
                    //Status = Convert.ToBoolean(identity.Claims.FirstOrDefault(c => c.Type == "status")?.Value),
                    //Email = identity.Claims.FirstOrDefault(c => c.Type == "Email")?.Value,
                    //BranchCode = identity.Claims.FirstOrDefault(c => c.Type == "bankCode")?.Value,
                    //Role = identity.Claims.FirstOrDefault(c => c.Type == "appRole")?.Value,
                    //ProfileId = Convert.ToInt32(identity.Claims.FirstOrDefault(c => c.Type == "profileId")?.Value),
                    //TellerTillAccountNumber = identity.Claims.FirstOrDefault(c => c.Type == "Tillaccount")?.Value,
                    //AccountName = identity.Claims.FirstOrDefault(c => c.Type == "accountName")?.Value,
                    //BranchName = identity.Claims.FirstOrDefault(c => c.Type == "branchName")?.Value,
                    //TotalExpectedDeposit = identity.Claims.FirstOrDefault(c => c.Type == "expectedDeposit")?.Value,
                    //TotalConfirmedDeposit = identity.Claims.FirstOrDefault(c => c.Type == "confirmedDeposit")?.Value,
                    //TotalValidations = identity.Claims.FirstOrDefault(c => c.Type == "totalValidations")?.Value,
                    //TotalUsers = identity.Claims.FirstOrDefault(c => c.Type == "totalusers")?.Value,
                };

                return getData;
            //return new UserDetailsViewModel
            //{

            //    //TellerId = identity.Claims.FirstOrDefault(c => c.Type == "Teller_id")?.Value,
            //    //UserID = identity.Claims.FirstOrDefault(c => c.Type == "Username")?.Value,
            //    //Status = Convert.ToBoolean(identity.Claims.FirstOrDefault(c => c.Type == "status")?.Value),
            //    //Email = identity.Claims.FirstOrDefault(c => c.Type == "Email")?.Value,
            //    //BranchCode = identity.Claims.FirstOrDefault(c => c.Type == "bankCode")?.Value,
            //    //Role = identity.Claims.FirstOrDefault(c => c.Type == "appRole")?.Value,
            //    //ProfileId = Convert.ToInt32(identity.Claims.FirstOrDefault(c => c.Type == "profileId")?.Value),
            //    //TellerTillAccountNumber = identity.Claims.FirstOrDefault(c => c.Type == "Tillaccount")?.Value,
            //    //AccountName = identity.Claims.FirstOrDefault(c => c.Type == "accountName")?.Value,
            //    //BranchName = identity.Claims.FirstOrDefault(c => c.Type == "branchName")?.Value,
            //    //TotalExpectedDeposit = identity.Claims.FirstOrDefault(c => c.Type == "expectedDeposit")?.Value,
            //    //TotalConfirmedDeposit = identity.Claims.FirstOrDefault(c => c.Type == "confirmedDeposit")?.Value,
            //    //TotalValidations = identity.Claims.FirstOrDefault(c => c.Type == "totalValidations")?.Value,
            //    //TotalUsers = identity.Claims.FirstOrDefault(c => c.Type == "totalusers")?.Value,
            //};

        }
            catch (Exception)
            {

                return null;

            }

}

public static void ClearSessionDetails(this IPrincipal principal)
{
    var identity = (ClaimsIdentity)principal.Identity;
    foreach (var claim in identity.Claims)
    {
        identity.TryRemoveClaim(claim);
    }
}
    }
}
