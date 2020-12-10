using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_card_trans_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FioranoT24CreditRequest");

            migrationBuilder.CreateTable(
                name: "FioranoT24CardCreditRequest",
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
                    table.PrimaryKey("PK_FioranoT24CardCreditRequest", x => x.PaymentReference);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FioranoT24CardCreditRequest");

            migrationBuilder.CreateTable(
                name: "FioranoT24CreditRequest",
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
                    table.PrimaryKey("PK_FioranoT24CreditRequest", x => x.PaymentReference);
                });
        }
    }
}
