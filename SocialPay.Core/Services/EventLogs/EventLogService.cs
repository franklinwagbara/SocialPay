using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Services.EventLogs
{
    public class EventLogService
    {
        private readonly IEventLogRequestService _eventLogRequestService;
        public EventLogService(IEventLogRequestService eventLogRequestService)
        {
            _eventLogRequestService = eventLogRequestService ?? throw new ArgumentNullException(nameof(eventLogRequestService));
        }

        private IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                return null;

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }

        public async Task<WebApiResponse> ActivityRequestLog(EventRequestDto model)
        {
            try
            {
                var request = new EventLogViewModel
                {
                    Description = model.Description,
                    ClientAuthenticationId = model.ClientAuthenticationId,
                    IpAddress = LocalIPAddress().ToString(),
                    ModuleAccessed = model.ModuleAccessed,
                    UserId = model.UserId
                };

                await _eventLogRequestService.AddAsync(request);

                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Message = "Success" };
            }
            catch (Exception ex)
            {
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Message = "Error" };
            }
        }
    }
}
