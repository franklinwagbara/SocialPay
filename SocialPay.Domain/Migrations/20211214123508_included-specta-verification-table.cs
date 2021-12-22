using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class includedspectaverificationtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VerifyEmailConfirmationCodeRequest",
                columns: table => new
                {
                    VerifyEmailConfirmationCodeRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerifyEmailConfirmationCodeRequest", x => x.VerifyEmailConfirmationCodeRequestId);
                });

            migrationBuilder.CreateTable(
                name: "VerifyEmailConfirmationCodeResponse",
                columns: table => new
                {
                    VerifyEmailConfirmationCodeResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    success = table.Column<bool>(type: "bit", nullable: false),
                    unAuthorizedRequest = table.Column<bool>(type: "bit", nullable: false),
                    __abp = table.Column<bool>(type: "bit", nullable: false),
                    code = table.Column<int>(type: "int", nullable: false),
                    message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    validationErrors = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerifyEmailConfirmationCodeResponse", x => x.VerifyEmailConfirmationCodeResponseId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VerifyEmailConfirmationCodeRequest");

            migrationBuilder.DropTable(
                name: "VerifyEmailConfirmationCodeResponse");
        }
    }
}
