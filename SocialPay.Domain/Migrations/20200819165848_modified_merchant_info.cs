using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_merchant_info : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryFees",
                table: "MerchantActivitySetup");

            migrationBuilder.AddColumn<decimal>(
                name: "OutSideLagos",
                table: "MerchantActivitySetup",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OutSideNigeria",
                table: "MerchantActivitySetup",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "WithinLagos",
                table: "MerchantActivitySetup",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutSideLagos",
                table: "MerchantActivitySetup");

            migrationBuilder.DropColumn(
                name: "OutSideNigeria",
                table: "MerchantActivitySetup");

            migrationBuilder.DropColumn(
                name: "WithinLagos",
                table: "MerchantActivitySetup");

            migrationBuilder.AddColumn<string>(
                name: "DeliveryFees",
                table: "MerchantActivitySetup",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
