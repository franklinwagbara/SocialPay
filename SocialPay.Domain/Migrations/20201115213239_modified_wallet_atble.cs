using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_wallet_atble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FioranoT24TransactionResponse_FioranoT24Request_FioranoT24RequestId",
                table: "FioranoT24TransactionResponse");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransferResponse_WalletTransferRequestLog_WalletTransferRequestLogId",
                table: "WalletTransferResponse");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransferResponse_WalletTransferRequestLogId",
                table: "WalletTransferResponse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletTransferRequestLog",
                table: "WalletTransferRequestLog");

            migrationBuilder.DropIndex(
                name: "IX_FioranoT24TransactionResponse_FioranoT24RequestId",
                table: "FioranoT24TransactionResponse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FioranoT24Request",
                table: "FioranoT24Request");

            migrationBuilder.DropColumn(
                name: "WalletTransferRequestLogId",
                table: "WalletTransferResponse");

            migrationBuilder.DropColumn(
                name: "WalletTransferRequestLogId",
                table: "WalletTransferRequestLog");

            migrationBuilder.DropColumn(
                name: "FioranoT24RequestId",
                table: "FioranoT24TransactionResponse");

            migrationBuilder.DropColumn(
                name: "FioranoT24RequestId",
                table: "FioranoT24Request");

            migrationBuilder.RenameColumn(
                name: "paymentRef",
                table: "WalletTransferRequestLog",
                newName: "PaymentReference");

            migrationBuilder.AddColumn<string>(
                name: "RequestId",
                table: "WalletTransferResponse",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WalletTransferRequestLogRequestId",
                table: "WalletTransferResponse",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestId",
                table: "WalletTransferRequestLog",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestId",
                table: "FioranoT24TransactionResponse",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestId",
                table: "FioranoT24Request",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletTransferRequestLog",
                table: "WalletTransferRequestLog",
                column: "RequestId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FioranoT24Request",
                table: "FioranoT24Request",
                column: "RequestId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransferResponse_WalletTransferRequestLog_WalletTransferRequestLogRequestId",
                table: "WalletTransferResponse");

            migrationBuilder.DropIndex(
                name: "IX_WalletTransferResponse_WalletTransferRequestLogRequestId",
                table: "WalletTransferResponse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletTransferRequestLog",
                table: "WalletTransferRequestLog");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FioranoT24Request",
                table: "FioranoT24Request");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "WalletTransferResponse");

            migrationBuilder.DropColumn(
                name: "WalletTransferRequestLogRequestId",
                table: "WalletTransferResponse");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "WalletTransferRequestLog");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "FioranoT24TransactionResponse");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "FioranoT24Request");

            migrationBuilder.RenameColumn(
                name: "PaymentReference",
                table: "WalletTransferRequestLog",
                newName: "paymentRef");

            migrationBuilder.AddColumn<long>(
                name: "WalletTransferRequestLogId",
                table: "WalletTransferResponse",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "WalletTransferRequestLogId",
                table: "WalletTransferRequestLog",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "FioranoT24RequestId",
                table: "FioranoT24TransactionResponse",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "FioranoT24RequestId",
                table: "FioranoT24Request",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletTransferRequestLog",
                table: "WalletTransferRequestLog",
                column: "WalletTransferRequestLogId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FioranoT24Request",
                table: "FioranoT24Request",
                column: "FioranoT24RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransferResponse_WalletTransferRequestLogId",
                table: "WalletTransferResponse",
                column: "WalletTransferRequestLogId");

            migrationBuilder.CreateIndex(
                name: "IX_FioranoT24TransactionResponse_FioranoT24RequestId",
                table: "FioranoT24TransactionResponse",
                column: "FioranoT24RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_FioranoT24TransactionResponse_FioranoT24Request_FioranoT24RequestId",
                table: "FioranoT24TransactionResponse",
                column: "FioranoT24RequestId",
                principalTable: "FioranoT24Request",
                principalColumn: "FioranoT24RequestId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransferResponse_WalletTransferRequestLog_WalletTransferRequestLogId",
                table: "WalletTransferResponse",
                column: "WalletTransferRequestLogId",
                principalTable: "WalletTransferRequestLog",
                principalColumn: "WalletTransferRequestLogId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
