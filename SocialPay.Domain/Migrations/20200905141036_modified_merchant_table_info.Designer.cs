﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SocialPay.Domain;

namespace SocialPay.Domain.Migrations
{
    [DbContext(typeof(SocialPayDbContext))]
    [Migration("20200905141036_modified_merchant_table_info")]
    partial class modified_merchant_table_info
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.0-preview.7.20365.15");

            modelBuilder.Entity("SocialPay.Domain.Entities.ClientAuthentication", b =>
                {
                    b.Property<long>("ClientAuthenticationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<byte[]>("ClientSecretHash")
                        .HasColumnType("varbinary(max)");

                    b.Property<byte[]>("ClientSecretSalt")
                        .HasColumnType("varbinary(max)");

                    b.Property<DateTime>("DateEntered")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastDateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StatusCode")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ClientAuthenticationId");

                    b.ToTable("ClientAuthentication");
                });

            modelBuilder.Entity("SocialPay.Domain.Entities.CreateWalletResponse", b =>
                {
                    b.Property<long>("CreateWalletResponseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<long>("ClientAuthenticationId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("DateEntered")
                        .HasColumnType("datetime2");

                    b.Property<string>("Message")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CreateWalletResponseId");

                    b.HasIndex("ClientAuthenticationId");

                    b.ToTable("CreateWalletResponse");
                });

            modelBuilder.Entity("SocialPay.Domain.Entities.MerchantActivitySetup", b =>
                {
                    b.Property<long>("MerchantActivitySetupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<long>("ClientAuthenticationId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("DateEntered")
                        .HasColumnType("datetime2");

                    b.Property<decimal>("OutSideLagos")
                        .HasColumnType("decimal(18,2)");

                    b.Property<decimal>("OutSideNigeria")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("PayOrchargeMe")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("ReceiveEmail")
                        .HasColumnType("bit");

                    b.Property<decimal>("WithinLagos")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("MerchantActivitySetupId");

                    b.HasIndex("ClientAuthenticationId");

                    b.ToTable("MerchantActivitySetup");
                });

            modelBuilder.Entity("SocialPay.Domain.Entities.MerchantBankInfo", b =>
                {
                    b.Property<long>("MerchantBankInfoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<string>("AccountName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BVN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BankName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("ClientAuthenticationId")
                        .HasColumnType("bigint");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Currency")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateEntered")
                        .HasColumnType("datetime2");

                    b.Property<bool>("DefaultAccount")
                        .HasColumnType("bit");

                    b.Property<string>("Nuban")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MerchantBankInfoId");

                    b.HasIndex("ClientAuthenticationId");

                    b.ToTable("MerchantBankInfo");
                });

            modelBuilder.Entity("SocialPay.Domain.Entities.MerchantBusinessInfo", b =>
                {
                    b.Property<long>("MerchantBusinessInfoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<string>("BusinessEmail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BusinessName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BusinessPhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Chargebackemail")
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("ClientAuthenticationId")
                        .HasColumnType("bigint");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateEntered")
                        .HasColumnType("datetime2");

                    b.Property<string>("FileLocation")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Logo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MerchantReferenceId")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MerchantBusinessInfoId");

                    b.HasIndex("ClientAuthenticationId");

                    b.ToTable("MerchantBusinessInfo");
                });

            modelBuilder.Entity("SocialPay.Domain.Entities.MerchantPaymentSetup", b =>
                {
                    b.Property<long>("MerchantPaymentSetupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<string>("AdditionalDetails")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<long>("ClientAuthenticationId")
                        .HasColumnType("bigint");

                    b.Property<string>("CustomUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateEntered")
                        .HasColumnType("datetime2");

                    b.Property<string>("DeliveryMethod")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DeliveryTime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("PaymentCategory")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentLinkName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PaymentMethod")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("RedirectAfterPayment")
                        .HasColumnType("bit");

                    b.Property<string>("TransactionReference")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TransactionToken")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MerchantPaymentSetupId");

                    b.HasIndex("ClientAuthenticationId");

                    b.ToTable("MerchantPaymentSetup");
                });

            modelBuilder.Entity("SocialPay.Domain.Entities.MerchantWallet", b =>
                {
                    b.Property<long>("MerchantWalletId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<long>("ClientAuthenticationId")
                        .HasColumnType("bigint");

                    b.Property<string>("CurrencyCode")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateEntered")
                        .HasColumnType("datetime2");

                    b.Property<string>("DoB")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Firstname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastDateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("Lastname")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Mobile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("MerchantWalletId");

                    b.HasIndex("ClientAuthenticationId");

                    b.ToTable("MerchantWallet");
                });

            modelBuilder.Entity("SocialPay.Domain.Entities.PinRequest", b =>
                {
                    b.Property<long>("PinRequestId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

                    b.Property<long>("ClientAuthenticationId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("DateEntered")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastDateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("Pin")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Status")
                        .HasColumnType("bit");

                    b.Property<string>("TokenSecret")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("PinRequestId");

                    b.HasIndex("ClientAuthenticationId");

                    b.ToTable("PinRequest");
                });

            modelBuilder.Entity("SocialPay.Domain.Entities.CreateWalletResponse", b =>
                {
                    b.HasOne("SocialPay.Domain.Entities.ClientAuthentication", "ClientAuthentication")
                        .WithMany("CreateWalletResponse")
                        .HasForeignKey("ClientAuthenticationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SocialPay.Domain.Entities.MerchantActivitySetup", b =>
                {
                    b.HasOne("SocialPay.Domain.Entities.ClientAuthentication", "ClientAuthentication")
                        .WithMany("MerchantActivitySetup")
                        .HasForeignKey("ClientAuthenticationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SocialPay.Domain.Entities.MerchantBankInfo", b =>
                {
                    b.HasOne("SocialPay.Domain.Entities.ClientAuthentication", "ClientAuthentication")
                        .WithMany("MerchantBankInfo")
                        .HasForeignKey("ClientAuthenticationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SocialPay.Domain.Entities.MerchantBusinessInfo", b =>
                {
                    b.HasOne("SocialPay.Domain.Entities.ClientAuthentication", "ClientAuthentication")
                        .WithMany("MerchantBusinessInfo")
                        .HasForeignKey("ClientAuthenticationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SocialPay.Domain.Entities.MerchantPaymentSetup", b =>
                {
                    b.HasOne("SocialPay.Domain.Entities.ClientAuthentication", "ClientAuthentication")
                        .WithMany("MerchantPaymentSetup")
                        .HasForeignKey("ClientAuthenticationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SocialPay.Domain.Entities.MerchantWallet", b =>
                {
                    b.HasOne("SocialPay.Domain.Entities.ClientAuthentication", "ClientAuthentication")
                        .WithMany("MerchantWallet")
                        .HasForeignKey("ClientAuthenticationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SocialPay.Domain.Entities.PinRequest", b =>
                {
                    b.HasOne("SocialPay.Domain.Entities.ClientAuthentication", "ClientAuthentication")
                        .WithMany("PinRequest")
                        .HasForeignKey("ClientAuthenticationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
