using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_transaction_log_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompletedPayWithSpecta",
                table: "TransactionLog",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsQueuedPayWithSpecta",
                table: "TransactionLog",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FioranoT24Request",
                columns: table => new
                {
                    FioranoT24RequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
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
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FioranoT24Request", x => x.FioranoT24RequestId);
                });

            migrationBuilder.CreateTable(
                name: "FioranoT24TransactionResponse",
                columns: table => new
                {
                    FioranoT24TransactionResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FioranoT24RequestId = table.Column<long>(type: "bigint", nullable: false),
                    ReferenceID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Balance = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    COMMAMT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CHARGEAMT = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FTID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JsonResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FioranoT24TransactionResponse", x => x.FioranoT24TransactionResponseId);
                    table.ForeignKey(
                        name: "FK_FioranoT24TransactionResponse_FioranoT24Request_FioranoT24RequestId",
                        column: x => x.FioranoT24RequestId,
                        principalTable: "FioranoT24Request",
                        principalColumn: "FioranoT24RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FioranoT24TransactionResponse_FioranoT24RequestId",
                table: "FioranoT24TransactionResponse",
                column: "FioranoT24RequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FioranoT24TransactionResponse");

            migrationBuilder.DropTable(
                name: "FioranoT24Request");

            migrationBuilder.DropColumn(
                name: "IsCompletedPayWithSpecta",
                table: "TransactionLog");

            migrationBuilder.DropColumn(
                name: "IsQueuedPayWithSpecta",
                table: "TransactionLog");
        }
    }
}
