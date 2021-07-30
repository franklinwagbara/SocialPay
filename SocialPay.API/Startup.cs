using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SocialPay.ApplicationCore.Interfaces.Repositories;
using SocialPay.ApplicationCore.Interfaces.Service;
using SocialPay.ApplicationCore.Services;
using SocialPay.Core.Configurations;
using SocialPay.Core.Extensions.Common;
using SocialPay.Core.Extensions.Utilities;
using SocialPay.Core.Interface;
using SocialPay.Core.Messaging;
using SocialPay.Core.Messaging.SendGrid;
using SocialPay.Core.Repositories.Customer;
using SocialPay.Core.Repositories.Invoice;
using SocialPay.Core.Repositories.UserService;
using SocialPay.Core.Services.Account;
using SocialPay.Core.Services.Authentication;
using SocialPay.Core.Services.AzureBlob;
using SocialPay.Core.Services.Bill;
using SocialPay.Core.Services.Customer;
using SocialPay.Core.Services.Data;
using SocialPay.Core.Services.Fiorano;
using SocialPay.Core.Services.IBS;
using SocialPay.Core.Services.Merchant;
using SocialPay.Core.Services.PayU;
using SocialPay.Core.Services.Products;
using SocialPay.Core.Services.QrCode;
using SocialPay.Core.Services.Report;
using SocialPay.Core.Services.Specta;
using SocialPay.Core.Services.Store;
using SocialPay.Core.Services.Tin;
using SocialPay.Core.Services.Transaction;
using SocialPay.Core.Services.Validations;
using SocialPay.Core.Services.Wallet;
using SocialPay.Core.Store;
using SocialPay.Domain;
using SocialPay.Helper.Cryptography;
using SocialPay.Helper.Notification;
using SocialPay.Job.Repository.BasicWalletFundService;
using SocialPay.Job.Repository.Fiorano;
using SocialPay.Job.Repository.InterBankService;
using SocialPay.Job.Repository.NibbsMerchantJobService.Interface;
using SocialPay.Job.Repository.NibbsMerchantJobService.Repository;
using SocialPay.Job.Repository.NibbsMerchantJobService.Services;
using SocialPay.Job.Repository.NonEscrowBankTransactions;
using SocialPay.Job.Repository.NonEscrowCardWalletTransaction;
using SocialPay.Job.Repository.NonEscrowOtherWalletTransaction;
using SocialPay.Job.Repository.NotificationService;
using SocialPay.Job.Repository.PayWithCard;
using SocialPay.Job.Services;
using SocialPay.Job.TaskSchedules;
using SocialPay.Persistance.Repositories;

namespace SocialPay.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddControllersWithViews();
            services.AddSwaggerGen();         
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Social Pay",
                    Version = "v1",
                    Description = "Full documentation to Social Pay API"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

            });
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.SecretKey);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(x =>
             {
                 x.RequireHttpsMetadata = false;
                 x.SaveToken = true;
                 x.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = new SymmetricSecurityKey(key),
                     ValidateIssuer = false,
                     ValidateAudience = false
                 };
             });

            var con = Configuration.GetConnectionString("SocialPayDbContextString");

            services.AddDbContext<SocialPayDbContext>(p => p.UseSqlServer(con), ServiceLifetime.Scoped);
            services.AddScoped<MerchantRegistrationService>();
            services.AddScoped<AuthRepoService>();
            services.AddScoped<EmailService>();
            services.AddScoped<Utilities>();
            services.AddScoped<BankServiceRepository>();
            services.AddScoped<IBSReposervice>();
            services.AddScoped<WalletRepoService>();
            services.AddScoped<MerchantPaymentLinkService>();
            services.AddScoped<ADRepoService>();
            services.AddScoped<MerchantReportService>();
            services.AddScoped<CustomerRepoService>();
            services.AddScoped<ICustomerService>();
            services.AddScoped<EncryptDecryptAlgorithm>();
            services.AddScoped<BlobService>();
           // services.AddScoped<EncryptDecrypt>();
            services.AddScoped<TransactionReceipt>();
            services.AddScoped<InvoiceService>();
            services.AddScoped<PayWithSpectaService>();
            services.AddScoped<TransactionService>();
            services.AddScoped<CreateMerchantWalletService>();
            services.AddScoped<UserRepoService>();
            services.AddScoped<AccountResetService>();
            services.AddScoped<DisputeRepoService>();
            services.AddScoped<TinService>();
            services.AddScoped<SendGridEmailService>();
            services.AddScoped<ProductsRepository>();
            services.AddScoped<IStore, StoreRepositoryOld>();
            services.AddScoped<INibbsQrMerchantService, NibbsQrMerchantService>();
            services.AddScoped<INibbsQrSubMerchantService, NibbsQrSubMerchantService>();
            services.AddScoped<IMerchantBusinessInfoService, MerchantBusinessInfoService>();
            services.AddScoped<INibbsQrMerchantResponseService, NibbsQrMerchantResponseService>();
            services.AddScoped<IPersonalInfoService, PersonalInfoService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<IProductsService, ProductsService>();
            services.AddScoped<INotification, Notification>();
            services.AddScoped<IMerchantPaymentSetupService, MerchantPaymentSetupService>();
            services.AddScoped<IClientAuthenticationService, ClientAuthenticationService>();
            services.AddScoped<IFioranoRequestService, FioranoRequestService>();
            services.AddScoped<IFioranoResponseService, FioranoResponseService>();
            services.AddSingleton<ICreateNibbsSubMerchantService, CreateNibbsSubMerchantService>();
            services.AddScoped<MerchantBusinessInfoBaseService>();
            services.AddScoped<MerchantPersonalInfoBaseService>();
            services.AddScoped<StoreRepository>();
            services.AddScoped<StoreBaseRepository>();
            services.AddScoped<NibbsQrBaseService>();
            services.AddScoped<NibbsQrRepository>();
            services.AddScoped<BillService>();
            services.AddScoped<DstvPaymentService>();
            services.AddScoped<FioranoService>();
            services.AddScoped<FioranoAPIService>();
            services.AddSingleton<NibbsQrJobCreateMerchantRepository>();
            services.AddSingleton<NibbsQrJobCreateSubMerchantRepository>();
            services.AddSingleton<BindMerchantServiceRepository>();
            services.AddSingleton<NibbsQRCodeAPIJobService>();
            services.AddScoped<NibbsQRCodeAPIService>();
            services.AddScoped<MerchantPersonalInfoRepository>();
            services.AddSingleton<WalletRepoJobService>();
            services.AddSingleton<InterBankPendingTransferService>();
            services.AddSingleton<SqlRepository>();
            services.AddSingleton<BankServiceRepositoryJobService>();
            services.AddSingleton<IBSReposerviceJob>();
            services.AddSingleton<EncryptDecryptJob>();
            services.AddSingleton<EncryptDecrypt>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IAsyncRepository<>), typeof(Repository<>));
            //services.AddScoped<ICreditMerchantWalletService, CreditMerchantWalletService>();
            //services.AddSingleton<CreditMerchantWalletTransactions>();
            //services.AddScoped<INonEscrowCardWalletTransaction, NonEscrowCardWalletTransaction>();
            //services.AddSingleton<NonEscrowCardWalletPendingTransaction>();
            //services.AddScoped<INonEscrowOtherWalletTransaction, NonEscrowOtherWalletTransaction>();
            //services.AddSingleton<NonEscrowOtherWalletPendingTransaction>();
            //services.AddScoped<INonEscrowBankTransaction, NonEscrowBankTransaction>();
            //services.AddSingleton<NonEscrowPendingBankTransaction>();

            //services.AddScoped<INotificationServices, NotificationService>();
            //services.AddSingleton<JobEmailService>();
            //services.AddSingleton<NotificationTransactions>();


            // services.AddSingleton<DeliveryDayFioranoTransferRepository>();
            services.AddSingleton<FioranoTransferNonEscrowRepository>();
            services.AddSingleton<IBSReposerviceJob>();
            services.AddSingleton<SqlRepository>();
            services.AddSingleton<FioranoAcceptedEscrowRepository>();
            services.AddSingleton<FioranoTransferPayWithCardRepository>();
            services.AddScoped<IPayWithCardTransaction, PayWithCardTransaction>();
            ////services.AddSingleton<IHostedService, CardPaymentTask>();
            services.AddSingleton<PendingPayWithCardTransaction>();
            services.AddSingleton<CreditDebitService>();
            services.AddScoped<IProcessMerchantWalletService, ProcessMerchantWalletService>();
            services.AddSingleton<ICreateNibbsMerchantService, CreateNibbsMerchantService>();
            services.AddSingleton<IBindMerchantService, BindMerchantService>();
            services.AddSingleton<ProcessMerchantWalletTransactions>();

            var options = Configuration.GetSection(nameof(CronExpressions)).Get<CronExpressions>();

            ////////services.AddCronJob<AcceptedEscrowBankOrderTask>(c =>
            ////////{
            ////////    c.TimeZoneInfo = TimeZoneInfo.Local;
            ////////    c.CronExpression = options.AcceptedEscrowBankOrderTask;
            ////////});

            ////////services.AddCronJob<AcceptedWalletOrderTask>(c =>
            ////////{
            ////////    c.TimeZoneInfo = TimeZoneInfo.Local;
            ////////    c.CronExpression = options.AcceptedWalletOrderTask;
            ////////});

            ///Main jobs starts

            //services.AddCronJob<BindMerchantTask>(c =>
            //{
            //    c.TimeZoneInfo = TimeZoneInfo.Local;
            //    c.CronExpression = options.BindMerchantTask;
            //});

            //services.AddCronJob<CreateNibbsSubMerchantTask>(c =>
            //{
            //    c.TimeZoneInfo = TimeZoneInfo.Local;
            //    c.CronExpression = options.CreateNibbsMerchantTask;
            //});

            //services.AddCronJob<CreateNibbsMerchantTask>(c =>
            //{
            //    c.TimeZoneInfo = TimeZoneInfo.Local;
            //    c.CronExpression = options.CreateNibbsMerchantTask;
            //});

            ////services.AddCronJob<CardPaymentTask>(c =>
            ////{
            ////    c.TimeZoneInfo = TimeZoneInfo.Local;
            ////    c.CronExpression = options.CardPaymentTask;
            ////});

            ////services.AddCronJob<CreditDefaultMerchantWalletTask>(c =>
            ////{
            ////    c.TimeZoneInfo = TimeZoneInfo.Local;
            ////    c.CronExpression = options.CreditDefaultMerchantWalletTask;
            ////});

            ////services.AddCronJob<NonEscrowBankTransactionTask>(c =>
            ////{
            ////    c.TimeZoneInfo = TimeZoneInfo.Local;
            ////    c.CronExpression = options.NonEscrowBankTransactionTask;
            ////});

            ////services.AddCronJob<NonEscrowOtherWalletTransactionTask>(c =>
            ////{
            ////    c.TimeZoneInfo = TimeZoneInfo.Local;
            ////    c.CronExpression = options.NonEscrowOtherWalletTransactionTask;
            ////});

            ////services.AddCronJob<NonEscrowWalletTransactionTask>(c =>
            ////{
            ////    c.TimeZoneInfo = TimeZoneInfo.Local;
            ////    c.CronExpression = options.NonEscrowWalletTransactionTask;
            ////});

            //////services.AddCronJob<ProcessFailedMerchantWalletTask>(c =>
            //////{
            //////    c.TimeZoneInfo = TimeZoneInfo.Local;
            //////    c.CronExpression = options.ProcessFailedMerchantWalletTask;
            //////});

            ///Main jobs ends

            ////////services.AddCronJob<DeclinedEscrowWalletTask>(c =>
            ////////{
            ////////    c.TimeZoneInfo = TimeZoneInfo.Local;
            ////////    c.CronExpression = options.DeclinedEscrowWalletTask;
            ////////});

            ////////services.AddCronJob<DeliveryDayBankTask>(c =>
            ////////{
            ////////    c.TimeZoneInfo = TimeZoneInfo.Local;
            ////////    c.CronExpression = options.DeliveryDayBankTask;
            ////////});

            ////////services.AddCronJob<DeliveryDayWalletTask>(c =>
            ////////{
            ////////    c.TimeZoneInfo = TimeZoneInfo.Local;
            ////////    c.CronExpression = options.DeliveryDayWalletTask;
            ////////});

            ////////services.AddCronJob<ExpiredProductNotificationTask>(c =>
            ////////{
            ////////    c.TimeZoneInfo = TimeZoneInfo.Local;
            ////////    c.CronExpression = options.ExpiredProductNotificationTask;
            ////////});




            var redisServer = Configuration.GetSection("RedisConnectionStrings")["RedisServer"];
            var redisInstance = Configuration.GetSection("RedisConnectionStrings")["RedisInstance"];

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = redisServer;
                options.InstanceName = redisInstance;
            });

            //services.AddDistributedRedisCache(option =>
            //{
            //    option.Configuration = "172.18.4.114:6379";
            //    option.InstanceName = "master";
            //});

            services.AddScoped<INotificationServices, NotificationService>();
            //services.AddSingleton<IHostedService, ExpiredProductNotificationTask>();
            services.AddSingleton<JobEmailService>();
            services.AddSingleton<NotificationTransactions>();

            ///Wallet tranaction
            ////services.AddScoped<IWalletTransactions, WalletTransactions>();
            ////services.AddSingleton<IHostedService, FundMerchantWalletTask>();
            ////services.AddSingleton<WalletRepoJobService>();
            ////services.AddSingleton<PendingWalletRequestService>();        

            services.AddScoped<ICreditMerchantWalletService, CreditMerchantWalletService>();
            // services.AddSingleton<IHostedService, CreditDefaultMerchantWalletTask>();
            services.AddSingleton<CreditMerchantWalletTransactions>();


            //services.AddScoped<IDeclineEscrowWalletTransaction, DeclineEscrowWalletTransaction>();
            //services.AddSingleton<IHostedService, DeclinedEscrowWalletTask>();
            //services.AddSingleton<DeclineEscrowWalletPendingTransaction>();

            //Non escrow wallet transaction

            services.AddScoped<INonEscrowCardWalletTransaction, NonEscrowCardWalletTransaction>();
            // services.AddSingleton<IHostedService, NonEscrowWalletTransactionTask>();
            services.AddSingleton<NonEscrowCardWalletPendingTransaction>();


            services.AddScoped<INonEscrowOtherWalletTransaction, NonEscrowOtherWalletTransaction>();
            //services.AddSingleton<IHostedService, NonEscrowOtherWalletTransactionTask>();
            services.AddSingleton<NonEscrowOtherWalletPendingTransaction>();

            //Non escrow bank transaction

            services.AddScoped<INonEscrowBankTransaction, NonEscrowBankTransaction>();
            //services.AddSingleton<IHostedService, NonEscrowBankTransactionTask>();
            services.AddSingleton<NonEscrowPendingBankTransaction>();

            //Accepted order service

            //services.AddScoped<IAcceptedEscrowOrders, AcceptedEscrowOrders>();
            ////services.AddSingleton<IHostedService, AcceptedWalletOrderTask>();
            //services.AddSingleton<AcceptedEscrowOrderTransactions>();

            //accepted escrow bank request

            //services.AddScoped<IAcceptedEscrowRequestBankTransaction, AcceptedEscrowRequestBankTransaction>();
            //// services.AddSingleton<IHostedService, AcceptedEscrowBankOrderTask>();
            //services.AddSingleton<AcceptedEscrowRequestPendingBankTransaction>();

            //////Credit T24 account for card payments
            //services.AddScoped<IPayWithCardTransaction, PayWithCardTransaction>();
            ////services.AddSingleton<IHostedService, CardPaymentTask>();
            //services.AddSingleton<PendingPayWithCardTransaction>();
           // services.AddSingleton<CreditDebitService>();

            ////Credit Social pay wallet from merchant wallet

           // services.AddScoped<IDeliveryDayMerchantTransfer, DeliveryDayMerchantTransfer>();
            //services.AddSingleton<IHostedService, DeliveryDayWalletTask>();
            //services.AddSingleton<DeliveryDayTransferService>();

           // services.AddScoped<IDeliveryDayBankTransaction, DeliveryDayBankTransaction>();
            //services.AddSingleton<IHostedService, DeliveryDayBankTask>();
          //  services.AddSingleton<DeliveryDayBankPendingTransaction>();


            //Intrabank transactions
           // services.AddScoped<IIntraBankTransferService, IntraBankTransferService>();
           // services.AddSingleton<IHostedService, IntraBankTask>();
           // services.AddSingleton<IntraBankPendingTransactions>();

            //Interbank transactions

           // services.AddScoped<IIntraBankTransferService, IntraBankTransferService>();
           // services.AddSingleton<IHostedService, IntraBankTask>();
           // services.AddSingleton<IntraBankPendingTransactions>();
       

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("../swagger/v1/swagger.json", "Social Pay API");
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseCors(x => x
               .AllowAnyMethod()
               .AllowAnyHeader()
               .SetIsOriginAllowed(origin => true) // allow any origin
               .AllowCredentials()); // allow credentials

            ////app.UseCors(x => x
            //// .AllowAnyOrigin()
            //// .AllowAnyMethod()
            //// .AllowAnyHeader());
            //.AllowCredentials());
            app.UseRouting();
            app.UseAuthorization();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("../swagger/v1/swagger.json", "Social Pay API");
            });

        }
    }
}
