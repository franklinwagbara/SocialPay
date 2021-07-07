using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class updatepaymentlinktable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastDateModified",
                table: "MerchantPaymentSetup",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "BindMerchant",
                columns: table => new
                {
                    BindMerchantId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantQRCodeOnboardingId = table.Column<long>(type: "bigint", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BindMerchant", x => x.BindMerchantId);
                    table.ForeignKey(
                        name: "FK_BindMerchant_MerchantQRCodeOnboarding_MerchantQRCodeOnboardingId",
                        column: x => x.MerchantQRCodeOnboardingId,
                        principalTable: "MerchantQRCodeOnboarding",
                        principalColumn: "MerchantQRCodeOnboardingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BindMerchant_MerchantQRCodeOnboardingId",
                table: "BindMerchant",
                column: "MerchantQRCodeOnboardingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BindMerchant");

            migrationBuilder.DropColumn(
                name: "LastDateModified",
                table: "MerchantPaymentSetup");
        }
    }
}
