using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modifymerchantcolumnlength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Lastname",
                table: "MerchantWallet",
                type: "NVARCHAR(55)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Firstname",
                table: "MerchantWallet",
                type: "NVARCHAR(55)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BusinessName",
                table: "MerchantBusinessInfo",
                type: "NVARCHAR(65)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "MerchantBankInfo",
                type: "NVARCHAR(65)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(35)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "ClientAuthentication",
                type: "NVARCHAR(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(35)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ClientAuthentication",
                type: "NVARCHAR(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(30)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Lastname",
                table: "MerchantWallet",
                type: "NVARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(55)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Firstname",
                table: "MerchantWallet",
                type: "NVARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(55)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BusinessName",
                table: "MerchantBusinessInfo",
                type: "NVARCHAR(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(65)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccountName",
                table: "MerchantBankInfo",
                type: "NVARCHAR(35)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(65)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserName",
                table: "ClientAuthentication",
                type: "NVARCHAR(35)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "ClientAuthentication",
                type: "NVARCHAR(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(50)",
                oldNullable: true);
        }
    }
}
