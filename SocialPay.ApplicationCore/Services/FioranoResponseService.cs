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

    public class FioranoResponseService : IFioranoResponseService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<FioranoBillsPaymentResponse> _billsResponse;

        public FioranoResponseService(IAsyncRepository<FioranoBillsPaymentResponse> billsResponse)
        {
            _billsResponse = billsResponse ?? throw new ArgumentNullException(nameof(billsResponse));

            var config = new MapperConfiguration(cfg => cfg.CreateMap<FioranoBillsPaymentResponse, FioronoBillsPaymentResponseViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<FioronoBillsPaymentResponseViewModel>> GetAllAsync()
        {
            var requests = await _billsResponse.GetAllAsync();

            return _mapper.Map<List<FioranoBillsPaymentResponse>, List<FioronoBillsPaymentResponseViewModel>>(requests);
        }


        public async Task<FioronoBillsPaymentResponseViewModel> GetTransactionByreference(string reference)
        {
            var request = await _billsResponse.GetSingleAsync(x => x.PaymentReference == reference);

            return _mapper.Map<FioranoBillsPaymentResponse, FioronoBillsPaymentResponseViewModel>(request);
        }

        public async Task<bool> ExistsAsync(long clientId)
        {
            return await _billsResponse.ExistsAsync(x => x.FioranoBillsRequestId == clientId);
        }

        public async Task<FioronoBillsPaymentResponseViewModel> AddAsync(FioronoBillsPaymentResponseViewModel model)
        {
            var entity = new FioranoBillsPaymentResponse
            {
              Balance = model.Balance,
              FTID = model.FTID,
              ReferenceID = model.ReferenceID,
              CHARGEAMT = model.CHARGEAMT,
              COMMAMT = model.COMMAMT,
              FioranoBillsRequestId = model.FioranoBillsRequestId,
              JsonResponse = model.JsonResponse,
              PaymentReference = model.PaymentReference,
              ResponseCode = model.ResponseCode,
              ResponseText = model.ResponseText
            };

            await _billsResponse.AddAsync(entity);

            return _mapper.Map<FioranoBillsPaymentResponse, FioronoBillsPaymentResponseViewModel>(entity);
        }


        public async Task<int> CountTotalTransactionAsync()
        {
            return 1;
            // return await _clientAuthentication.CountAsync(x => x.AvailableFlag == true);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _billsResponse.GetByIdAsync(id);

            await _billsResponse.DeleteAsync(entity);
        }

    }

}
