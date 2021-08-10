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

    public class QrPaymentResponseService : IQrPaymentResponseService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<QrPaymentResponse> _qrPaymentResponse;

        public QrPaymentResponseService(IAsyncRepository<QrPaymentResponse> qrPaymentRequest)
        {
            _qrPaymentResponse = qrPaymentRequest ?? throw new ArgumentNullException(nameof(qrPaymentRequest));

            var config = new MapperConfiguration(cfg => cfg.CreateMap<QrPaymentResponse, QrPaymentResponseViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<QrPaymentResponseViewModel>> GetAllAsync()
        {
            var requests = await _qrPaymentResponse.GetAllAsync();

            return _mapper.Map<List<QrPaymentResponse>, List<QrPaymentResponseViewModel>>(requests);
        }

        public async Task<QrPaymentResponseViewModel> GetTransactionByreference(string reference)
        {
            var request = await _qrPaymentResponse
                .GetSingleAsync(x => x.OrderSn == reference);

            return _mapper.Map<QrPaymentResponse, QrPaymentResponseViewModel>(request);
        }

        public async Task<QrPaymentResponseViewModel> AddAsync(QrPaymentResponseViewModel model)
        {
            var entity = new QrPaymentResponse
            {
               LastDateModified = DateTime.Now,
               SessionID = model.SessionID,
               Amount = model.Amount,
               CodeUrl = model.CodeUrl,
               OrderSn = model.OrderSn,
               PaymentReference = model.PaymentReference,
               QrPaymentRequestId = model.QrPaymentRequestId
            };

            await _qrPaymentResponse.AddAsync(entity);

            return _mapper.Map<QrPaymentResponse, QrPaymentResponseViewModel>(entity);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _qrPaymentResponse.GetByIdAsync(id);

            await _qrPaymentResponse.DeleteAsync(entity);
        }

    }

}
