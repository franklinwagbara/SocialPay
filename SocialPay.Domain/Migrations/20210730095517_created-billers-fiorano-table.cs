using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class createdbillersfioranotable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FioranoBillsRequest",
                columns: table => new
                {
                    FioranoBillsRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionBranch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillsType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitAcctNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebitAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreditAccountNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommissionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VtellerAppID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    narrations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrxnLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FioranoBillsRequest", x => x.FioranoBillsRequestId);
                    table.ForeignKey(
                        name: "FK_FioranoBillsRequest_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FioranoBillsPaymentResponse",
                columns: table => new
                {
                    FioranoBillsPaymentResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FioranoBillsRequestId = table.Column<long>(type: "bigint", nullable: false),
                    PaymentReference = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ReferenceID = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    ResponseCode = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    ResponseText = table.Column<string>(type: "NVARCHAR(280)", nullable: true),
                    Balance = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    COMMAMT = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    CHARGEAMT = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    FTID = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    JsonResponse = table.Column<string>(type: "NVARCHAR(550)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FioranoBillsPaymentResponse", x => x.FioranoBillsPaymentResponseId);
                    table.ForeignKey(
                        name: "FK_FioranoBillsPaymentResponse_FioranoBillsRequest_FioranoBillsRequestId",
                        column: x => x.FioranoBillsRequestId,
                        principalTable: "FioranoBillsRequest",
                        principalColumn: "FioranoBillsRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FioranoBillsPaymentResponse_FioranoBillsRequestId",
                table: "FioranoBillsPaymentResponse",
                column: "FioranoBillsRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_FioranoBillsRequest_ClientAuthenticationId",
                table: "FioranoBillsRequest",
                column: "ClientAuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FioranoBillsPaymentResponse");

            migrationBuilder.DropTable(
                name: "FioranoBillsRequest");
        }
    }
}
