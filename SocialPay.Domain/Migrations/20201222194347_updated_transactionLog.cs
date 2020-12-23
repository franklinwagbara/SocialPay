using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class updated_transactionLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "TransactionLog",
                type: "NVARCHAR(250)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(50)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerEmail",
                table: "TransactionLog",
                type: "NVARCHAR(40)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(30)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "TransactionLog",
                type: "NVARCHAR(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(250)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerEmail",
                table: "TransactionLog",
                type: "NVARCHAR(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(40)",
                oldNullable: true);
        }
    }
}
