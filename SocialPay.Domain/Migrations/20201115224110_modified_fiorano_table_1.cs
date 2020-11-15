using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_fiorano_table_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransferResponse_WalletTransferRequestLog_WalletTransferRequestLogRequestId",
                table: "WalletTransferResponse");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransferResponse_WalletTransferRequestLogRequestId",
                table: "WalletTransferResponse");

            migrationBuilder.DropColumn(
                name: "WalletTransferRequestLogRequestId",
                table: "WalletTransferResponse");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "FioranoT24TransactionResponse",
                newName: "PaymentReference");

            migrationBuilder.RenameColumn(
                name: "RequestId",
                table: "FioranoT24Request",
                newName: "PaymentReference");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentReference",
                table: "FioranoT24TransactionResponse",
                newName: "RequestId");

            migrationBuilder.RenameColumn(
                name: "PaymentReference",
                table: "FioranoT24Request",
                newName: "RequestId");

            migrationBuilder.AddColumn<string>(
                name: "WalletTransferRequestLogRequestId",
                table: "WalletTransferResponse",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransferResponse_WalletTransferRequestLogRequestId",
                table: "WalletTransferResponse",
                column: "WalletTransferRequestLogRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransferResponse_WalletTransferRequestLog_WalletTransferRequestLogRequestId",
                table: "WalletTransferResponse",
                column: "WalletTransferRequestLogRequestId",
                principalTable: "WalletTransferRequestLog",
                principalColumn: "RequestId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
