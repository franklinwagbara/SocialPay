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

    public class VendAirtimeRequestService : IVendAirtimeRequestService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<VendAirtimeRequestLog> _vendAirtimeRequestLog;

        public VendAirtimeRequestService(IAsyncRepository<VendAirtimeRequestLog> vendAirtimeRequestLog)
        {
            _vendAirtimeRequestLog = vendAirtimeRequestLog ?? throw new ArgumentNullException(nameof(vendAirtimeRequestLog));

            var config = new MapperConfiguration(cfg => cfg.CreateMap<VendAirtimeRequestLog, VendAirtimeViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<VendAirtimeViewModel>> GetAllAsync()
        {
            var requests = await _vendAirtimeRequestLog.GetAllAsync();

            return _mapper.Map<List<VendAirtimeRequestLog>, List<VendAirtimeViewModel>>(requests);
        }

        public async Task<List<VendAirtimeViewModel>> GetAirtimeByClient(long clientId)
        {
            var requests = await _vendAirtimeRequestLog.GetAsync(x => x.ClientAuthenticationId == clientId);

            return _mapper.Map<List<VendAirtimeRequestLog>, List<VendAirtimeViewModel>>(requests);
        }

        public async Task<VendAirtimeViewModel> GetAirtimeByReference(string reference)
        {
            var request = await _vendAirtimeRequestLog.GetSingleAsync(x => x.ReferenceId == reference);

            return _mapper.Map<VendAirtimeRequestLog, VendAirtimeViewModel>(request);
        }

        public async Task<bool> ExistsAsync(long clientId)
        {
            return await _vendAirtimeRequestLog.ExistsAsync(x => x.ClientAuthenticationId == clientId);
        }

        public async Task<VendAirtimeViewModel> AddAsync(VendAirtimeViewModel model)
        {
            var entity = new VendAirtimeRequestLog
            {
                ClientAuthenticationId = model.ClientAuthenticationId,
                Amount = model.Amount,
                // Status = model.Status,
                email = model.email,
                Paymentcode = model.Paymentcode,
                Mobile = model.Mobile,
                ReferenceId = model.ReferenceId,
                RequestType = model.RequestType,
                nuban = model.nuban
            };

            await _vendAirtimeRequestLog.AddAsync(entity);

            return _mapper.Map<VendAirtimeRequestLog, VendAirtimeViewModel>(entity);
        }


        public async Task<int> CountTotalTransactionAsync()
        {
            return 1;
            // return await _clientAuthentication.CountAsync(x => x.AvailableFlag == true);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _vendAirtimeRequestLog.GetByIdAsync(id);

            await _vendAirtimeRequestLog.DeleteAsync(entity);
        }

    }

}
