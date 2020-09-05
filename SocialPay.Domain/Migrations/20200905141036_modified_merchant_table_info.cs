using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_merchant_table_info : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "MerchantPaymentSetup",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TransactionReference",
                table: "MerchantPaymentSetup",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionToken",
                table: "MerchantPaymentSetup",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "MerchantPaymentSetup");

            migrationBuilder.DropColumn(
                name: "TransactionReference",
                table: "MerchantPaymentSetup");

            migrationBuilder.DropColumn(
                name: "TransactionToken",
                table: "MerchantPaymentSetup");
        }
    }
}
