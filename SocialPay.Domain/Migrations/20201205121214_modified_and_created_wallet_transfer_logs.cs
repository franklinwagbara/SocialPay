using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_and_created_wallet_transfer_logs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FioranoT24Request");

            migrationBuilder.CreateTable(
                name: "CreditMerchantWalletTransferRequestLog",
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
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CreditMerchantWalletTransferRequestLog", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_CreditMerchantWalletTransferRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DebitMerchantWalletTransferRequestLog",
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
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebitMerchantWalletTransferRequestLog", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_DebitMerchantWalletTransferRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DefaultWalletTransferRequestLog",
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
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultWalletTransferRequestLog", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_DefaultWalletTransferRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FioranoT24CreditRequest",
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
                    table.PrimaryKey("PK_FioranoT24CreditRequest", x => x.PaymentReference);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CreditMerchantWalletTransferRequestLog_ClientAuthenticationId",
                table: "CreditMerchantWalletTransferRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_DebitMerchantWalletTransferRequestLog_ClientAuthenticationId",
                table: "DebitMerchantWalletTransferRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultWalletTransferRequestLog_ClientAuthenticationId",
                table: "DefaultWalletTransferRequestLog",
                column: "ClientAuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CreditMerchantWalletTransferRequestLog");

            migrationBuilder.DropTable(
                name: "DebitMerchantWalletTransferRequestLog");

            migrationBuilder.DropTable(
                name: "DefaultWalletTransferRequestLog");

            migrationBuilder.DropTable(
                name: "FioranoT24CreditRequest");

            migrationBuilder.CreateTable(
                name: "FioranoT24Request",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommissionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditAccountNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitAcctNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitAmount = table.Column<double>(type: "float", nullable: false),
                    DebitCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JsonRequest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionBranch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrxnLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VtellerAppID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    narrations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FioranoT24Request", x => x.PaymentReference);
                });
        }
    }
}
