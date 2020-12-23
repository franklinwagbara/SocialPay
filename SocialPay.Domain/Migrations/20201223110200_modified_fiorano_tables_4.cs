using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_fiorano_tables_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "narrations",
                table: "FioranoT24CardCreditRequest",
                type: "NVARCHAR(530)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(130)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "FioranoT24CardCreditRequest",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(35)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JsonRequest",
                table: "FioranoT24CardCreditRequest",
                type: "NVARCHAR(950)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(250)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "narrations",
                table: "FioranoT24CardCreditRequest",
                type: "NVARCHAR(130)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(530)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "FioranoT24CardCreditRequest",
                type: "NVARCHAR(35)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JsonRequest",
                table: "FioranoT24CardCreditRequest",
                type: "NVARCHAR(250)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(950)",
                oldNullable: true);
        }
    }
}
