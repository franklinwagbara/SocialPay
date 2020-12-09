using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class created_other_wallets_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestId",
                table: "DebitMerchantWalletTransferRequestLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestId",
                table: "CreditMerchantWalletTransferRequestLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AcceptedEscrowFioranoT24Request",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionBranch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitAcctNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitAmount = table.Column<double>(type: "float", nullable: false),
                    CreditAccountNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommissionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VtellerAppID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    narrations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrxnLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JsonRequest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcceptedEscrowFioranoT24Request", x => x.PaymentReference);
                });

            migrationBuilder.CreateTable(
                name: "AcceptedEscrowWalletTransferRequestLog",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    amt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    toacct = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    frmacct = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcceptedEscrowWalletTransferRequestLog", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_AcceptedEscrowWalletTransferRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeclinedWalletTransferRequestLog",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    amt = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    toacct = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    frmacct = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelMode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeclinedWalletTransferRequestLog", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_DeclinedWalletTransferRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NonEscrowFioranoT24Request",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionBranch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitAcctNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitAmount = table.Column<double>(type: "float", nullable: false),
                    CreditAccountNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommissionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VtellerAppID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    narrations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrxnLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JsonRequest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NonEscrowFioranoT24Request", x => x.PaymentReference);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedEscrowWalletTransferRequestLog_ClientAuthenticationId",
                table: "AcceptedEscrowWalletTransferRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_DeclinedWalletTransferRequestLog_ClientAuthenticationId",
                table: "DeclinedWalletTransferRequestLog",
                column: "ClientAuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcceptedEscrowFioranoT24Request");

            migrationBuilder.DropTable(
                name: "AcceptedEscrowWalletTransferRequestLog");

            migrationBuilder.DropTable(
                name: "DeclinedWalletTransferRequestLog");

            migrationBuilder.DropTable(
                name: "NonEscrowFioranoT24Request");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "DebitMerchantWalletTransferRequestLog");

            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "CreditMerchantWalletTransferRequestLog");
        }
    }
}
