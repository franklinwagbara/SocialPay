using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class created_interbank_transfer_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterBankTransactionRequest",
                columns: table => new
                {
                    PaymentReference = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginatorKYCLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BeneficiaryKYCLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BeneficiaryBankVerificationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginatorBankVerificationNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AppID = table.Column<int>(type: "int", nullable: false),
                    AccountLockID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginatorAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DestinationBankCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrignatorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubAcctVal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LedCodeVal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurCodeVal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CusNumVal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BraCodeVal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Vat = table.Column<double>(type: "float", nullable: false),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NESessionID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChannelCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterBankTransactionRequest", x => x.PaymentReference);
                    table.ForeignKey(
                        name: "FK_InterBankTransactionRequest_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterBankTransactionRequest_ClientAuthenticationId",
                table: "InterBankTransactionRequest",
                column: "ClientAuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterBankTransactionRequest");
        }
    }
}
