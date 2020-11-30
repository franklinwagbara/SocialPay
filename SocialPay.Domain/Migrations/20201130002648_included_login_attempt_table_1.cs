using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class included_login_attempt_table_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoginAttempt");

            migrationBuilder.CreateTable(
                name: "ClientLoginStatus",
                columns: table => new
                {
                    ClientLoginStatusId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    IsSuccessful = table.Column<bool>(type: "bit", nullable: false),
                    LoginAttempt = table.Column<int>(type: "int", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientLoginStatus", x => x.ClientLoginStatusId);
                    table.ForeignKey(
                        name: "FK_ClientLoginStatus_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoginAttemptHistory",
                columns: table => new
                {
                    LoginAttemptHistoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAttemptHistory", x => x.LoginAttemptHistoryId);
                    table.ForeignKey(
                        name: "FK_LoginAttemptHistory_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientLoginStatus_ClientAuthenticationId",
                table: "ClientLoginStatus",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttemptHistory_ClientAuthenticationId",
                table: "LoginAttemptHistory",
                column: "ClientAuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientLoginStatus");

            migrationBuilder.DropTable(
                name: "LoginAttemptHistory");

            migrationBuilder.CreateTable(
                name: "LoginAttempt",
                columns: table => new
                {
                    LoginAttemptId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAttempt", x => x.LoginAttemptId);
                    table.ForeignKey(
                        name: "FK_LoginAttempt_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoginAttempt_ClientAuthenticationId",
                table: "LoginAttempt",
                column: "ClientAuthenticationId");
        }
    }
}
