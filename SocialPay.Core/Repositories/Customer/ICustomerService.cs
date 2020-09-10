using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialPay.Core.Services;
using SocialPay.Core.Services.Authentication;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Core.Repositories.Customer
{
    public class ICustomerService : BaseService<MerchantPaymentSetup>
    {
        private readonly SocialPayDbContext _context;
        private readonly AuthRepoService _authRepoService;
        public ICustomerService(SocialPayDbContext context, AuthRepoService authRepoService) : base(context)
        {
            _context = context;
            _authRepoService = authRepoService;
        }

        public async Task<MerchantPaymentSetup> GetTransactionReference(string refId)
        {
            return await _context.MerchantPaymentSetup.SingleOrDefaultAsync(p => p.TransactionReference
            == refId
            );
        }

        public async Task<WebApiResponse> GetClientDetails(string email)
        {
            var getClientInfo = await _authRepoService.GetClientDetails(email);
            if (getClientInfo == null)
                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };
            return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
        }

        public async Task<WebApiResponse> CreateNewCustomer(string email, string fullname, string phoneNumber)
        {
            var createCustomer = await _authRepoService.CreateAccount(email, null, fullname,  phoneNumber);
            if (createCustomer == null)
                return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound };
            return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
        }

        public async Task<PaymentLinkViewModel> GetTransactionDetails(string refId)
        {
            var paymentview = new PaymentLinkViewModel();
            var validateReference = await GetTransactionReference(refId);
            if (validateReference == null)
                return new PaymentLinkViewModel { };
            var config = new MapperConfiguration(cfg => cfg.CreateMap<MerchantPaymentSetup, PaymentLinkViewModel>());
            var mapper = config.CreateMapper();
            paymentview = mapper.Map<PaymentLinkViewModel>(validateReference);
            return paymentview;
        }

        public async Task<List<MerchantPaymentSetup>> GetAllPaymentLinksByClientId(long clientId)
        {
            return await _context.MerchantPaymentSetup.Where(x => x.IsDeleted
            == false && x.ClientAuthenticationId == clientId).ToListAsync();
        }


        public async Task <List<PaymentLinkViewModel>> GetPaymentLinks(long clientId)
        {
            var paymentview = new List<PaymentLinkViewModel>();
            var validateReference = await GetAllPaymentLinksByClientId(clientId);
            if (validateReference == null)
                return paymentview;
            var config = new MapperConfiguration(cfg => cfg.CreateMap<MerchantPaymentSetup, PaymentLinkViewModel>());
            var mapper = config.CreateMapper();
            paymentview = mapper.Map<List<PaymentLinkViewModel>>(validateReference);
            return paymentview;
        }
    }
}
