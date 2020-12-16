using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class updated_businessInfo_datatypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Logo",
                table: "MerchantBusinessInfo",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileLocation",
                table: "MerchantBusinessInfo",
                type: "NVARCHAR(190)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Chargebackemail",
                table: "MerchantBusinessInfo",
                type: "NVARCHAR(40)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BusinessEmail",
                table: "MerchantBusinessInfo",
                type: "NVARCHAR(40)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(20)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Logo",
                table: "MerchantBusinessInfo",
                type: "NVARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FileLocation",
                table: "MerchantBusinessInfo",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(190)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Chargebackemail",
                table: "MerchantBusinessInfo",
                type: "NVARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(40)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BusinessEmail",
                table: "MerchantBusinessInfo",
                type: "NVARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(40)",
                oldNullable: true);
        }
    }
}
