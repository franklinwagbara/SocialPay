using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class createdussdtables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UssdServiceRequestLog",
                columns: table => new
                {
                    UssdServiceRequestLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    MerchantID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MerchantName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TerminalId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RetrievalReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InstitutionCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Customer_mobile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubMerchantName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UssdServiceRequestLog", x => x.UssdServiceRequestLogId);
                    table.ForeignKey(
                        name: "FK_UssdServiceRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UssdServiceResponseLog",
                columns: table => new
                {
                    UssdServiceResponseLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UssdServiceRequestLogId = table.Column<long>(type: "bigint", nullable: false),
                    ResponseCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallBackResponseCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallBackResponseMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TraceID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UssdServiceResponseLog", x => x.UssdServiceResponseLogId);
                    table.ForeignKey(
                        name: "FK_UssdServiceResponseLog_UssdServiceRequestLog_UssdServiceRequestLogId",
                        column: x => x.UssdServiceRequestLogId,
                        principalTable: "UssdServiceRequestLog",
                        principalColumn: "UssdServiceRequestLogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UssdServiceRequestLog_ClientAuthenticationId",
                table: "UssdServiceRequestLog",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_UssdServiceResponseLog_UssdServiceRequestLogId",
                table: "UssdServiceResponseLog",
                column: "UssdServiceRequestLogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UssdServiceResponseLog");

            migrationBuilder.DropTable(
                name: "UssdServiceRequestLog");
        }
    }
}
