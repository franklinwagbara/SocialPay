using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class updatednibbstables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "MerchantQRCodeOnboarding",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "MerchantQRCodeOnboarding",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "MerchantQRCodeOnboarding");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "MerchantQRCodeOnboarding");
        }
    }
}
