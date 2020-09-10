using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class updated_merchant_info : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionToken",
                table: "MerchantPaymentSetup",
                newName: "PaymentLinkUrl");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentLinkUrl",
                table: "MerchantPaymentSetup",
                newName: "TransactionToken");
        }
    }
}
