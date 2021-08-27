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

    public class UssdRequestLogService : IUssdRequestLogService
    {
        private readonly IMapper _mapper;
        private readonly IAsyncRepository<UssdServiceRequestLog> _ussdServiceRequestLog;

        public UssdRequestLogService(IAsyncRepository<UssdServiceRequestLog> ussdServiceRequestLog)
        {
            _ussdServiceRequestLog = ussdServiceRequestLog ?? throw new ArgumentNullException(nameof(ussdServiceRequestLog));

            var config = new MapperConfiguration(cfg => cfg.CreateMap<UssdServiceRequestLog, UssdRequestViewModel>());

            _mapper = config.CreateMapper();
        }

        public async Task<List<UssdRequestViewModel>> GetAllAsync()
        {
            var requests = await _ussdServiceRequestLog.GetAllAsync();

            return _mapper.Map<List<UssdServiceRequestLog>, List<UssdRequestViewModel>>(requests);
        }

        public async Task<List<UssdRequestViewModel>> GetUssdByClientId(long clientId)
        {
            var requests = await _ussdServiceRequestLog.GetAsync(x => x.ClientAuthenticationId == clientId);

            return _mapper.Map<List<UssdServiceRequestLog>, List<UssdRequestViewModel>>(requests);
        }

        public async Task<UssdRequestViewModel> GetTransactionByreference(string reference)
        {
            var request = await _ussdServiceRequestLog
                .GetSingleAsync(x => x.TransactionID == reference || x.PaymentReference == reference);

            return _mapper.Map<UssdServiceRequestLog, UssdRequestViewModel>(request);
        }

        public async Task<bool> ExistsAsync(long clientId)
        {
            return await _ussdServiceRequestLog.ExistsAsync(x => x.ClientAuthenticationId == clientId);
        }

        public async Task<UssdRequestViewModel> AddAsync(UssdRequestViewModel model)
        {
            var entity = new UssdServiceRequestLog
            {
              ShortName = model.ShortName,
              MerchantID = model.MerchantID,
              UserID = model.UserID,
              Amount = model.Amount,
              Channel = model.Channel,
              ClientAuthenticationId = model.ClientAuthenticationId,
              Customer_mobile = model.Customer_mobile,
              InstitutionCode = model.InstitutionCode,
              MerchantName = model.MerchantName,
              RetrievalReference = model.RetrievalReference,
              SubMerchantName = model.SubMerchantName,
              TerminalId = model.TerminalId,
              TransactionType = model.TransactionType,
              TransRef = model.TransRef
            };

            await _ussdServiceRequestLog.AddAsync(entity);

            return _mapper.Map<UssdServiceRequestLog, UssdRequestViewModel>(entity);
        }

        public async Task UpdateAsync(UssdRequestViewModel model)
        {
            var entity = await _ussdServiceRequestLog.GetSingleAsync(x => x.UssdServiceRequestLogId == model.UssdServiceRequestLogId);

            entity.ResponseCode = model.ResponseCode;
            entity.ResponseMessage = model.ResponseMessage;
            entity.TransactionID = model.TransactionID;
            entity.TransactionRef = model.TransactionRef;
            entity.TraceID = model.TraceID;
            entity.LastDateModified = DateTime.Now;

            await _ussdServiceRequestLog.UpdateAsync(entity);
        }

        public async Task<int> CountTotalTransactionAsync()
        {
            return 1;
            // return await _clientAuthentication.CountAsync(x => x.AvailableFlag == true);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _ussdServiceRequestLog.GetByIdAsync(id);

            await _ussdServiceRequestLog.DeleteAsync(entity);
        }

    }

}
