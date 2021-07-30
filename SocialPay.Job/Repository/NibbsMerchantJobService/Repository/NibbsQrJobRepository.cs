using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SocialPay.Core.Configurations;
using SocialPay.Domain;
using SocialPay.Domain.Entities;
using SocialPay.Helper;
using SocialPay.Helper.Dto.Request;
using SocialPay.Helper.Dto.Response;
using SocialPay.Helper.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialPay.Job.Repository.NibbsMerchantJobService.Repository
{
    public class NibbsQrJobRepository
    {
        private readonly AppSettings _appSettings;
        static readonly log4net.ILog _log4net = log4net.LogManager.GetLogger(typeof(NibbsQrJobRepository));
        public NibbsQrJobRepository(IOptions<AppSettings> appSettings, IServiceProvider service)
        {
            _appSettings = appSettings.Value;
            Services = service;

        }

        public IServiceProvider Services { get; }

        public async Task<WebApiResponse> ProcessTransactions(List<NibbsQrMerchantViewModel> pendingRequest)
        {
            long transactionLogid = 0;

            try
            {
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SocialPayDbContext>();

                    foreach (var item in pendingRequest)
                    {
                        //_log4net.Info("Job Service" + "-" + "Non Escrow Pending Bank Transaction request" + " | " + item.PaymentReference + " | " + item.TransactionReference + " | " + DateTime.Now);

                        var client = await context.ClientAuthentication
                            .SingleOrDefaultAsync(x => x.ClientAuthenticationId == item.ClientAuthenticationId);

                        var merchant = new MerchantQRCodeOnboarding
                        {
                            //IsDeleted = false,
                            //Address = model.Address,
                            //ClientAuthenticationId = clientId,
                            //Contact = model.Contact,
                            //Email = model.Email,
                            //Fee = model.Fee,
                            //Name = model.Name,
                            //Phone = model.Phone,
                            //Tin = model.Tin,
                            //IsCompleted = false,
                            //Status = NibbsMerchantOnboarding.CreateAccount
                        };

                        await context.MerchantQRCodeOnboarding.AddAsync(merchant);
                        await context.SaveChangesAsync();

                        var defaultRequest = new CreateNibsMerchantRequestDto
                        {
                            //Address = model.Address,
                            //Contact = model.Contact,
                            //Email = model.Email,
                            //Fee = 00,
                            //Name = model.Name,
                            //Phone = model.Phone,
                            //Tin = model.Tin
                        };
                    }

                    //Other banks transfer
                    return new WebApiResponse { ResponseCode = AppResponseCodes.Success };
                }

            }
            catch (Exception ex)
            {
                _log4net.Error("Job Service" + "-" + "Base Error occured" + " | " + transactionLogid + " | " + ex.Message.ToString() + " | " + DateTime.Now);

           
                return new WebApiResponse { ResponseCode = AppResponseCodes.InternalError };
            }
        }

    }
}
