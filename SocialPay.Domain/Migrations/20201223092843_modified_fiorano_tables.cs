using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_fiorano_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "NonEscrowFioranoT24Request",
                type: "NVARCHAR(350)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(230)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JsonRequest",
                table: "NonEscrowFioranoT24Request",
                type: "NVARCHAR(550)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(250)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "NonEscrowFioranoT24Request",
                type: "NVARCHAR(230)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(350)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "JsonRequest",
                table: "NonEscrowFioranoT24Request",
                type: "NVARCHAR(250)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(550)",
                oldNullable: true);
        }
    }
}
