using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Core.Repositories.Invoice
{
    public class InvoiceService
    {
        private readonly SocialPayDbContext _context;
        public InvoiceService(SocialPayDbContext context)
        {
            _context = context;
        }

        public async Task<WebApiResponse> GetInvoiceByClientId(long clientId)
        {
            var invoiceView = new List<MerchantInvoiceViewModel>();

            try
            {
                var getInvoice = await _context.InvoicePaymentLink
                    .Where(x => x.ClientAuthenticationId == clientId).ToListAsync();
                if (getInvoice.Count == 0)
                    return new WebApiResponse { ResponseCode = AppResponseCodes.RecordNotFound, Data = invoiceView };
                var config = new MapperConfiguration(cfg => cfg.CreateMap<InvoicePaymentLink, MerchantInvoiceViewModel>());
                var mapper = config.CreateMapper();
                invoiceView = mapper.Map<List<MerchantInvoiceViewModel>>(getInvoice);
                return new WebApiResponse { ResponseCode = AppResponseCodes.Success, Data = invoiceView };
            }
            catch (Exception ex)
            {

                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError, Data = invoiceView };
            }
        }
    }
}
