using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_fiorano_tables_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "narrations",
                table: "NonEscrowFioranoT24Request",
                type: "NVARCHAR(530)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(130)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "narrations",
                table: "NonEscrowFioranoT24Request",
                type: "NVARCHAR(130)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(530)",
                oldNullable: true);
        }
    }
}
