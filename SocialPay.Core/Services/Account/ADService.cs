using EwService;
using LdapService;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.Account
{
    public class ADRepoService
    {
        private readonly SocialPayDbContext _context;
        private readonly AppSettings _appSettings;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(ADRepoService));

        public ADRepoService(SocialPayDbContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        public async Task<WebApiResponse> RegisterUser(CreateUserRequestDto createUserRequestDto)
        {
            try
            {
                _log4net.Info("RegisterUser" + " | " + createUserRequestDto.Username + " | " +  DateTime.Now);

                var aduserInfo = new ServiceSoapClient(ServiceSoapClient.EndpointConfiguration.ServiceSoap, _appSettings.EwsServiceUrl);
                var banksLdap = new ldapSoapClient(ldapSoapClient.EndpointConfiguration.ldapSoap, _appSettings.LdapServiceUrl);
                //bool validateADUser = await banksLdap.loginAsync(username, password);
                //bool validateADUser = await banksLdap.checkUserNameAsync(createUserRequestDto.Username);
                //if(!validateADUser)
                //    return new WebApiResponse { ResponseCode = AppResponseCodes.InvalidADUser };

                var userDetails = await aduserInfo.GetADDetailsAsync(createUserRequestDto.Username);
                var validAccount = userDetails.Nodes[1];
                var accountDetail = validAccount.Descendants("sr")

                    .Select(b => new ADUserViewModel
                    {
                        Username = b.Element("username")?.Value,
                        SupervisorEmail = b.Element("supervisor_email")?.Value,
                        SupervisorName = b.Element("supervisor_name")?.Value,
                        Fullname = b.Element("fullname")?.Value,
                        Email = b.Element("email")?.Value,
                        mobile = b.Element("mobile")?.Value,

                    }).FirstOrDefault();
                if (accountDetail == null)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.UserNotFoundOnAD };

                if (await _context.ClientAuthentication.AnyAsync(x => x.Email == accountDetail.Email))
                    return new WebApiResponse { ResponseCode = AppResponseCodes.DuplicateEmail };

                var model = new ClientAuthentication
                {
                    Email = accountDetail.Email,
                    StatusCode = AppResponseCodes.Success,
                    FullName = accountDetail.Fullname,
                    IsDeleted = false,
                    PhoneNumber = accountDetail.mobile,
                    RoleName = RoleDetails.SuperAdministrator,
                    LastDateModified = DateTime.Now,
                    UserName = createUserRequestDto.Username
                };
                await _context.ClientAuthentication.AddAsync(model);
                await _context.SaveChangesAsync();
                _log4net.Info("RegisterUser was successful" + " | " + createUserRequestDto.Username + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception ex)
            {
                _log4net.Error("Error occured" + " | " + "RegisterUser" + " | " + createUserRequestDto.Username + " | " + ex.Message.ToString() + " | " + DateTime.Now);

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

        public async Task<LoginAPIResponse> ValidateUserAD(string username, string password)
        {
            try
            {
                var banksLdap = new ldapSoapClient(ldapSoapClient.EndpointConfiguration.ldapSoap, _appSettings.LdapServiceUrl);
                bool validateADUser = await banksLdap.loginAsync(username, password);
                if(!validateADUser)
                    return new LoginAPIResponse { ResponseCode = AppResponseCodes.InvalidLogin };
                return new LoginAPIResponse { ResponseCode = AppResponseCodes.Success };
            }
            catch (Exception)
            {

                return new LoginAPIResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }
    }
}
