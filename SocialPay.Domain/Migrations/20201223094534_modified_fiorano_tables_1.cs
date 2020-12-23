using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_fiorano_tables_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "JsonRequest",
                table: "NonEscrowFioranoT24Request",
                type: "NVARCHAR(980)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(550)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "JsonRequest",
                table: "NonEscrowFioranoT24Request",
                type: "NVARCHAR(550)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(980)",
                oldNullable: true);
        }
    }
}
