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

    public class EventLogRequestService : IEventLogRequestService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<EventLog> _eventLog;

        public EventLogRequestService(IAsyncRepository<EventLog> eventLog)
        {
            _eventLog = eventLog ?? throw new ArgumentNullException(nameof(eventLog));

            var config = new MapperConfiguration(cfg => cfg.CreateMap<EventLog, EventLogViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<EventLogViewModel>> GetAllAsync()
        {
            var requests = await _eventLog.GetAllAsync();

            return _mapper.Map<List<EventLog>, List<EventLogViewModel>>(requests);
        }

        public async Task<List<EventLogViewModel>> GetEventByClient(long clientId)
        {
            var requests = await _eventLog.GetAsync(x => x.ClientAuthenticationId == clientId);

            return _mapper.Map<List<EventLog>, List<EventLogViewModel>>(requests);
        }

        public async Task<EventLogViewModel> GetEventByUserId(string userId)
        {
            var request = await _eventLog
                .GetSingleAsync(x => x.UserId == userId);

            return _mapper.Map<EventLog, EventLogViewModel>(request);
        }

        public async Task<bool> ExistsAsync(long clientId)
        {
            return await _eventLog.ExistsAsync(x => x.ClientAuthenticationId == clientId);
        }

        public async Task<EventLogViewModel> AddAsync(EventLogViewModel model)
        {
            var entity = new EventLog
            {
                ClientAuthenticationId = model.ClientAuthenticationId,
                Description = model.Description,
                ModuleAccessed = model.ModuleAccessed,
                UserId = model.UserId,
                IpAddress = model.IpAddress
            };

            await _eventLog.AddAsync(entity);

            return _mapper.Map<EventLog, EventLogViewModel>(entity);
        }


        public async Task<int> CountTotalTransactionAsync()
        {
            return 1;
            // return await _clientAuthentication.CountAsync(x => x.AvailableFlag == true);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _eventLog.GetByIdAsync(id);

            await _eventLog.DeleteAsync(entity);
        }

    }

}
