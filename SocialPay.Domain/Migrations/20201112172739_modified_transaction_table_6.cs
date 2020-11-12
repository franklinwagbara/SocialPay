using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_transaction_table_6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsQueued",
                table: "TransactionLog",
                newName: "IsQueuedPayWithCard");

            migrationBuilder.RenameColumn(
                name: "IsCompleted",
                table: "TransactionLog",
                newName: "IsCompletedPayWithCard");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsQueuedPayWithCard",
                table: "TransactionLog",
                newName: "IsQueued");

            migrationBuilder.RenameColumn(
                name: "IsCompletedPayWithCard",
                table: "TransactionLog",
                newName: "IsCompleted");
        }
    }
}
