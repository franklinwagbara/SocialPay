using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modifiedmerchantbusinessinfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpectaMerchantKey",
                table: "MerchantBusinessInfo",
                type: "NVARCHAR(90)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpectaMerchantKeyValue",
                table: "MerchantBusinessInfo",
                type: "NVARCHAR(90)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpectaMerchantKey",
                table: "MerchantBusinessInfo");

            migrationBuilder.DropColumn(
                name: "SpectaMerchantKeyValue",
                table: "MerchantBusinessInfo");
        }
    }
}
