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

    public class FioranoRequestService : IFioranoRequestService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<FioranoBillsRequest> _fioranoBillsRequest;

        public FioranoRequestService(IAsyncRepository<FioranoBillsRequest> fioranoBillsRequest)
        {
            _fioranoBillsRequest = fioranoBillsRequest ?? throw new ArgumentNullException(nameof(fioranoBillsRequest));

            var config = new MapperConfiguration(cfg => cfg.CreateMap<FioranoBillsRequest, FioranoRequestViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<FioranoRequestViewModel>> GetAllAsync()
        {
            var requests = await _fioranoBillsRequest.GetAllAsync();

            return _mapper.Map<List<FioranoBillsRequest>, List<FioranoRequestViewModel>>(requests);
        }

        public async Task<List<FioranoRequestViewModel>> GetTransactionByClient(long clientId)
        {
            var requests = await _fioranoBillsRequest.GetAsync(x => x.ClientAuthenticationId == clientId);

            return _mapper.Map<List<FioranoBillsRequest>, List<FioranoRequestViewModel>>(requests);
        }

        public async Task<FioranoRequestViewModel> GetTransactionByreference(string reference)
        {
            var request = await _fioranoBillsRequest
                .GetSingleAsync(x => x.TransactionReference == reference);

            return _mapper.Map<FioranoBillsRequest, FioranoRequestViewModel>(request);
        }

        public async Task<bool> ExistsAsync(long clientId)
        {
            return await _fioranoBillsRequest.ExistsAsync(x => x.ClientAuthenticationId == clientId);
        }

        public async Task<FioranoRequestViewModel> AddAsync(FioranoRequestViewModel model)
        {
            var entity = new FioranoBillsRequest
            {
               BillsType = model.BillsType,
               SessionId = model.SessionId,
               DebitAcctNo = model.DebitAcctNo,
               DebitAmount = model.DebitAmount,
               DebitCurrency = model.DebitCurrency,
               VtellerAppID = model.VtellerAppID,
               ClientAuthenticationId = model.ClientAuthenticationId,
               CommissionCode = model.CommissionCode,
               CreditAccountNo = model.CreditAccountNo,
               CreditCurrency = model.CreditCurrency,
               narrations = model.narrations,
               TransactionBranch = model.TransactionBranch,
               TransactionReference = model.TransactionReference,
               TransactionType = model.TransactionType,
               TrxnLocation = model.TrxnLocation
            };

            await _fioranoBillsRequest.AddAsync(entity);

            return _mapper.Map<FioranoBillsRequest, FioranoRequestViewModel>(entity);
        }


        public async Task<int> CountTotalTransactionAsync()
        {
            return 1;
            // return await _clientAuthentication.CountAsync(x => x.AvailableFlag == true);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _fioranoBillsRequest.GetByIdAsync(id);

            await _fioranoBillsRequest.DeleteAsync(entity);
        }

    }

}
