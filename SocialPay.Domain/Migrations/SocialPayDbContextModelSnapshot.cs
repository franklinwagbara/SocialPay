﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SocialPay.Domain;

namespace SocialPay.Domain.Migrations
{
    [DbContext(typeof(SocialPayDbContext))]
    partial class SocialPayDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

            modelBuilder.Entity("SocialPay.Domain.Entities.MerchantBankInfo", b =>
                {
                    b.Property<long>("MerchantBankInfoId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint")
                        .UseIdentityColumn();

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
