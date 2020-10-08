using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_t24transfer_table_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerOtherPaymentsInfo",
                columns: table => new
                {
                    CustomerOtherPaymentsInfoId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    MerchantPaymentSetupId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerOtherPaymentsInfo", x => x.CustomerOtherPaymentsInfoId);
                    table.ForeignKey(
                        name: "FK_CustomerOtherPaymentsInfo_MerchantPaymentSetup_MerchantPaymentSetupId",
                        column: x => x.MerchantPaymentSetupId,
                        principalTable: "MerchantPaymentSetup",
                        principalColumn: "MerchantPaymentSetupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOtherPaymentsInfo_MerchantPaymentSetupId",
                table: "CustomerOtherPaymentsInfo",
                column: "MerchantPaymentSetupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerOtherPaymentsInfo");
        }
    }
}
