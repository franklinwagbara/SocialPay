using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_t24transfer_table_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "MerchantPaymentSetup",
                newName: "MerchantAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "CustomerAmount",
                table: "MerchantPaymentSetup",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "CustomerDescription",
                table: "MerchantPaymentSetup",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerAmount",
                table: "MerchantPaymentSetup");

            migrationBuilder.DropColumn(
                name: "CustomerDescription",
                table: "MerchantPaymentSetup");

            migrationBuilder.RenameColumn(
                name: "MerchantAmount",
                table: "MerchantPaymentSetup",
                newName: "Amount");
        }
    }
}
