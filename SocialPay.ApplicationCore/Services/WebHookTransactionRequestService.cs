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

    public class WebHookTransactionRequestService : IWebHookTransactionRequestService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<WebHookTransactionRequestLog> _webHookTransactionRequestLog;

        public WebHookTransactionRequestService(IAsyncRepository<WebHookTransactionRequestLog> webHookTransactionRequestLog)
        {
            _webHookTransactionRequestLog = webHookTransactionRequestLog ?? throw new ArgumentNullException(nameof(webHookTransactionRequestLog));

            var config = new MapperConfiguration(cfg => cfg.CreateMap<WebHookTransactionRequestLog, WebHookTransactionRequestViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<WebHookTransactionRequestViewModel>> GetAllAsync()
        {
            var requests = await _webHookTransactionRequestLog.GetAllAsync();

            return _mapper.Map<List<WebHookTransactionRequestLog>, List<WebHookTransactionRequestViewModel>>(requests);
        }

     
        public async Task<WebHookTransactionRequestViewModel> GetTransactionByreference(string reference)
        {
            var request = await _webHookTransactionRequestLog
                .GetSingleAsync(x => x.OrderNo == reference);

            return _mapper.Map<WebHookTransactionRequestLog, WebHookTransactionRequestViewModel>(request);
        }

        public async Task<bool> ExistsAsync(long clientId)
        {
            return await _webHookTransactionRequestLog.ExistsAsync(x => x.WebHookTransactionRequestLogId == clientId);
        }

        public async Task<WebHookTransactionRequestViewModel> AddAsync(WebHookTransactionRequestViewModel model)
        {
            var entity = new WebHookTransactionRequestLog
            {
              Sign = model.Sign,
              MerchantFee = model.MerchantFee,
              MerchantName = model.MerchantName,
              MerchantNo = model.MerchantNo,
              NotificationType = model.NotificationType,
              OrderNo = model.OrderNo,
              OrderSn = model.OrderSn,
              ResidualAmount = model.ResidualAmount,
              SubMerchantName = model.SubMerchantName,
              SubMerchantNo = model.SubMerchantNo,
              TimeStamp = model.TimeStamp,
              TransactionAmount = model.TransactionAmount,
              TransactionTime = model.TransactionTime,
              TransactionType = model.TransactionType
            };

            await _webHookTransactionRequestLog.AddAsync(entity);

            return _mapper.Map<WebHookTransactionRequestLog, WebHookTransactionRequestViewModel>(entity);
        }


        public async Task<int> CountTotalTransactionAsync()
        {
            return 1;
            // return await _clientAuthentication.CountAsync(x => x.AvailableFlag == true);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _webHookTransactionRequestLog.GetByIdAsync(id);

            await _webHookTransactionRequestLog.DeleteAsync(entity);
        }

    }

}
