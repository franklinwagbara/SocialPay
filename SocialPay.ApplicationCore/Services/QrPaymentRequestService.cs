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

    public class QrPaymentRequestService : IQrPaymentRequestService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<QrPaymentRequest> _qrPaymentRequest;

        public QrPaymentRequestService(IAsyncRepository<QrPaymentRequest> qrPaymentRequest)
        {
            _qrPaymentRequest = qrPaymentRequest ?? throw new ArgumentNullException(nameof(qrPaymentRequest));

            var config = new MapperConfiguration(cfg => cfg.CreateMap<QrPaymentRequest, QrRequestPaymentViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<QrRequestPaymentViewModel>> GetAllAsync()
        {
            var requests = await _qrPaymentRequest.GetAllAsync();

            return _mapper.Map<List<QrPaymentRequest>, List<QrRequestPaymentViewModel>>(requests);
        }

        public async Task<QrRequestPaymentViewModel> GetTransactionByreference(string reference)
        {
            var request = await _qrPaymentRequest
                .GetSingleAsync(x => x.OrderNo == reference);

            return _mapper.Map<QrPaymentRequest, QrRequestPaymentViewModel>(request);
        }

        public async Task<bool> ExistsAsync(long clientId)
        {
            return await _qrPaymentRequest.ExistsAsync(x => x.ClientAuthenticationId == clientId);
        }

        public async Task<QrRequestPaymentViewModel> AddAsync(QrRequestPaymentViewModel model)
        {
            var entity = new QrPaymentRequest
            {
               ClientAuthenticationId = model.ClientAuthenticationId,
               LastDateModified = DateTime.Now,
               MchNo = model.MchNo,
               OrderNo = model.OrderNo,
               OrderType = model.OrderType,
               PaymentRequestReference = model.PaymentRequestReference,
               SubMchNo = model.SubMchNo
            };

            await _qrPaymentRequest.AddAsync(entity);

            return _mapper.Map<QrPaymentRequest, QrRequestPaymentViewModel>(entity);
        }


        public async Task<int> CountTotalTransactionAsync()
        {
            return 1;
            // return await _clientAuthentication.CountAsync(x => x.AvailableFlag == true);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _qrPaymentRequest.GetByIdAsync(id);

            await _qrPaymentRequest.DeleteAsync(entity);
        }

    }

}
