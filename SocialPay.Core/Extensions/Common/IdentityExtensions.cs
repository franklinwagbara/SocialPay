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

                var getData = new UserDetailsViewModel
                {
                    Email = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                    UserID = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                    ClientId = Convert.ToInt32(identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value),
                    Role = identity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value,

                };

                return getData;
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
