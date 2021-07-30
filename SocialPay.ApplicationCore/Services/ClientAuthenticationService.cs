using AutoMapper;
using SocialPay.ApplicationCore.Interfaces.Repositories;
using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.Domain.Entities;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialPay.ApplicationCore.Services
{

    public class ClientAuthenticationService : IClientAuthenticationService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<ClientAuthentication> _clientAuthentication;

        public ClientAuthenticationService(IAsyncRepository<ClientAuthentication> clientAuthentication)
        {
            _clientAuthentication = clientAuthentication ?? throw new ArgumentNullException(nameof(clientAuthentication));

            var config = new MapperConfiguration(cfg => cfg.CreateMap<ClientAuthentication, ClientAuthenticationViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<ClientAuthenticationViewModel>> GetAllAsync()
        {
            var users = await _clientAuthentication.GetAllAsync();

            return _mapper.Map<List<ClientAuthentication>, List<ClientAuthenticationViewModel>>(users);
        }

        public async Task<ClientAuthenticationViewModel> GetUserByClientIdInfo(long clientId)
        {
            var userInfo = await _clientAuthentication.GetSingleAsync(x => x.ClientAuthenticationId == clientId);

            return _mapper.Map<ClientAuthentication, ClientAuthenticationViewModel>(userInfo);
        }

    }

}
