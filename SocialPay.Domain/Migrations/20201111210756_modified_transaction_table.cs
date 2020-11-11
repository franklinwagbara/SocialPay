using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_transaction_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsQueuedPayWithSpecta",
                table: "TransactionLog",
                newName: "TransactionCompleted");

            migrationBuilder.RenameColumn(
                name: "IsCompletedPayWithSpecta",
                table: "TransactionLog",
                newName: "IsWalletQueued");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "TransactionLog",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "TransactionLog",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsWalletCompleted",
                table: "TransactionLog",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "WalletTransferRequestLog",
                columns: table => new
                {
                    WalletTransferRequestLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    amt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    toacct = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    frmacct = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    paymentRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransferRequestLog", x => x.WalletTransferRequestLogId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletTransferRequestLog");

            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "TransactionLog");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "TransactionLog");

            migrationBuilder.DropColumn(
                name: "IsWalletCompleted",
                table: "TransactionLog");

            migrationBuilder.RenameColumn(
                name: "TransactionCompleted",
                table: "TransactionLog",
                newName: "IsQueuedPayWithSpecta");

            migrationBuilder.RenameColumn(
                name: "IsWalletQueued",
                table: "TransactionLog",
                newName: "IsCompletedPayWithSpecta");
        }
    }
}
