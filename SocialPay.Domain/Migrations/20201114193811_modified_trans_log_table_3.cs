using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_trans_log_table_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ClientAuthenticationId",
                table: "WalletTransferRequestLog",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransferRequestLog_ClientAuthentication_ClientAuthenticationId",
                table: "WalletTransferRequestLog");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransferRequestLog_ClientAuthenticationId",
                table: "WalletTransferRequestLog");

            migrationBuilder.DropColumn(
                name: "ClientAuthenticationId",
                table: "WalletTransferRequestLog");
        }
    }
}
