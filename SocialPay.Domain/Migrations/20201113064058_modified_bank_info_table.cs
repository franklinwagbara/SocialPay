using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_bank_info_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BranchCode",
                table: "MerchantBankInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CusNum",
                table: "MerchantBankInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LedCode",
                table: "MerchantBankInfo",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchCode",
                table: "MerchantBankInfo");

            migrationBuilder.DropColumn(
                name: "CusNum",
                table: "MerchantBankInfo");

            migrationBuilder.DropColumn(
                name: "LedCode",
                table: "MerchantBankInfo");
        }
    }
}
