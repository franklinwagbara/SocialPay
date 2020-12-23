using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_fiorano_tables_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "NonEscrowFioranoT24Request",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(35)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "NonEscrowFioranoT24Request",
                type: "NVARCHAR(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(350)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SessionId",
                table: "NonEscrowFioranoT24Request",
                type: "NVARCHAR(35)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "NonEscrowFioranoT24Request",
                type: "NVARCHAR(350)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(50)",
                oldNullable: true);
        }
    }
}
