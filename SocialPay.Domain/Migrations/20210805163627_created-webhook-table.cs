using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class createdwebhooktable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WebHookRequest",
                columns: table => new
                {
                    WebHookRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    webHookUri = table.Column<long>(type: "bigint", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    filters = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    headers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebHookRequest", x => x.WebHookRequestId);
                    table.ForeignKey(
                        name: "FK_WebHookRequest_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebHookTransactionRequestLog",
                columns: table => new
                {
                    WebHookTransactionRequestLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NotificationType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchantName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchantNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubMerchantName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubMerchantNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchantFee = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResidualAmount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderSn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sign = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebHookTransactionRequestLog", x => x.WebHookTransactionRequestLogId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WebHookRequest_ClientAuthenticationId",
                table: "WebHookRequest",
                column: "ClientAuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WebHookRequest");

            migrationBuilder.DropTable(
                name: "WebHookTransactionRequestLog");
        }
    }
}
