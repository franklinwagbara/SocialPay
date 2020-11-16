using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_wallet_transfer_logs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransferRequestLog_ClientAuthentication_ClientAuthenticationId",
                table: "WalletTransferRequestLog");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransferRequestLog_ClientAuthenticationId",
                table: "WalletTransferRequestLog");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_WalletTransferRequestLog_ClientAuthenticationId",
                table: "WalletTransferRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransferRequestLog_ClientAuthentication_ClientAuthenticationId",
                table: "WalletTransferRequestLog",
                column: "ClientAuthenticationId",
                principalTable: "ClientAuthentication",
                principalColumn: "ClientAuthenticationId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
