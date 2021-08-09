using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class createdairtimevendingrequesttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VendAirtimeRequestLog",
                columns: table => new
                {
                    VendAirtimeRequestLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    ReferenceId = table.Column<string>(type: "NVARCHAR(25)", nullable: true),
                    Translocation = table.Column<string>(type: "NVARCHAR(55)", nullable: true),
                    email = table.Column<string>(type: "NVARCHAR(55)", nullable: true),
                    SubscriberInfo1 = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    nuban = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    TransactionType = table.Column<string>(type: "NVARCHAR(45)", nullable: true),
                    AppId = table.Column<int>(type: "int", nullable: false),
                    RequestType = table.Column<int>(type: "int", nullable: false),
                    TerminalID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendAirtimeRequestLog", x => x.VendAirtimeRequestLogId);
                    table.ForeignKey(
                        name: "FK_VendAirtimeRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VendAirtimeRequestLog_ClientAuthenticationId",
                table: "VendAirtimeRequestLog",
                column: "ClientAuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VendAirtimeRequestLog");
        }
    }
}
