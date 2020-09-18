using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class updated_payment_link_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CustomerTransaction",
                columns: table => new
                {
                    CustomerTransactionId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantPaymentSetupId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerTransaction", x => x.CustomerTransactionId);
                    table.ForeignKey(
                        name: "FK_CustomerTransaction_MerchantPaymentSetup_MerchantPaymentSetupId",
                        column: x => x.MerchantPaymentSetupId,
                        principalTable: "MerchantPaymentSetup",
                        principalColumn: "MerchantPaymentSetupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerTransaction_MerchantPaymentSetupId",
                table: "CustomerTransaction",
                column: "MerchantPaymentSetupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerTransaction");
        }
    }
}
