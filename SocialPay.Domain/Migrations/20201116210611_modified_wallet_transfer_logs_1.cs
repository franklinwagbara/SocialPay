using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_wallet_transfer_logs_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StatusJourney",
                table: "TransactionLog",
                newName: "TransactionJourney");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TransactionJourney",
                table: "TransactionLog",
                newName: "StatusJourney");
        }
    }
}
