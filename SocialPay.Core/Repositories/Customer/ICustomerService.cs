using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialPay.Core.Services;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SocialPay.Core.Repositories.Customer
{
    public class ICustomerService : BaseService<MerchantPaymentSetup>
    {
        private readonly SocialPayDbContext _context;
        public ICustomerService(SocialPayDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<MerchantPaymentSetup> GetTransactionReference(string refId)
        {
            return await _context.MerchantPaymentSetup.SingleOrDefaultAsync(p => p.TransactionReference
            == refId
            );
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

        public async Task<List<MerchantPaymentSetup>> GetAllPaymentLinks()
        {
            return await _context.MerchantPaymentSetup.Where(x => x.IsDeleted
            == false).ToListAsync();
        }
    }
}
