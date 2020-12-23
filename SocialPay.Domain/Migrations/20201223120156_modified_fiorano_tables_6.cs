using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_fiorano_tables_6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "FailedTransactions",
                type: "NVARCHAR(550)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(350)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "FailedTransactions",
                type: "NVARCHAR(350)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(550)",
                oldNullable: true);
        }
    }
}
