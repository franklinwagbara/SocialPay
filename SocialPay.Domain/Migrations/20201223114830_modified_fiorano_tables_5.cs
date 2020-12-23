using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_fiorano_tables_5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ResponseText",
                table: "FioranoT24TransactionResponse",
                type: "NVARCHAR(280)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JsonResponse",
                table: "FioranoT24TransactionResponse",
                type: "NVARCHAR(550)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(250)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ResponseText",
                table: "FioranoT24TransactionResponse",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(280)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JsonResponse",
                table: "FioranoT24TransactionResponse",
                type: "NVARCHAR(250)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(550)",
                oldNullable: true);
        }
    }
}
