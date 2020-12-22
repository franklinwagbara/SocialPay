using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class updated_interbank_logs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AcceptedEscrowInterBankTransactionRequest",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    OriginatorKYCLevel = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    BeneficiaryKYCLevel = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    BeneficiaryBankVerificationNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    OriginatorBankVerificationNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    AppID = table.Column<int>(type: "int", nullable: false),
                    AccountLockID = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    OriginatorAccountNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    AccountNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    AccountName = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    DestinationBankCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    OrignatorName = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    SubAcctVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    LedCodeVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CurCodeVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CusNumVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    BraCodeVal = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    Vat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentRef = table.Column<string>(type: "NVARCHAR(120)", nullable: true),
                    NESessionID = table.Column<string>(type: "NVARCHAR(35)", nullable: true),
                    ChannelCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AcceptedEscrowInterBankTransactionRequest", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_AcceptedEscrowInterBankTransactionRequest_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryDayEscrowInterBankTransactionRequest",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    OriginatorKYCLevel = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    BeneficiaryKYCLevel = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    BeneficiaryBankVerificationNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    OriginatorBankVerificationNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    AppID = table.Column<int>(type: "int", nullable: false),
                    AccountLockID = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    OriginatorAccountNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    AccountNumber = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    AccountName = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    DestinationBankCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    OrignatorName = table.Column<string>(type: "NVARCHAR(40)", nullable: true),
                    SubAcctVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    LedCodeVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CurCodeVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CusNumVal = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    BraCodeVal = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    Vat = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentRef = table.Column<string>(type: "NVARCHAR(120)", nullable: true),
                    NESessionID = table.Column<string>(type: "NVARCHAR(35)", nullable: true),
                    ChannelCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryDayEscrowInterBankTransactionRequest", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_DeliveryDayEscrowInterBankTransactionRequest_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryDayWalletTransferRequestLog",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    RequestId = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    amt = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    toacct = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    frmacct = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    remarks = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    channelID = table.Column<int>(type: "int", nullable: false),
                    CURRENCYCODE = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    TransferType = table.Column<int>(type: "int", nullable: false),
                    ChannelMode = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryDayWalletTransferRequestLog", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_DeliveryDayWalletTransferRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FioranoT24DeliveryDayRequest",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: false),
                    TransactionReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    TransactionBranch = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    TransactionType = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitAcctNo = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitCurrency = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    CreditCurrency = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    DebitAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreditAccountNo = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    CommissionCode = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    VtellerAppID = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    narrations = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    SessionId = table.Column<string>(type: "NVARCHAR(35)", nullable: true),
                    TrxnLocation = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    JsonRequest = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    Channel = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    Message = table.Column<string>(type: "NVARCHAR(230)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FioranoT24DeliveryDayRequest", x => x.PaymentReference);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AcceptedEscrowInterBankTransactionRequest_ClientAuthenticationId",
                table: "AcceptedEscrowInterBankTransactionRequest",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDayEscrowInterBankTransactionRequest_ClientAuthenticationId",
                table: "DeliveryDayEscrowInterBankTransactionRequest",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDayWalletTransferRequestLog_ClientAuthenticationId",
                table: "DeliveryDayWalletTransferRequestLog",
                column: "ClientAuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AcceptedEscrowInterBankTransactionRequest");

            migrationBuilder.DropTable(
                name: "DeliveryDayEscrowInterBankTransactionRequest");

            migrationBuilder.DropTable(
                name: "DeliveryDayWalletTransferRequestLog");

            migrationBuilder.DropTable(
                name: "FioranoT24DeliveryDayRequest");
        }
    }
}
