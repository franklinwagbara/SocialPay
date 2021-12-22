using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class includedspectatables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ProductItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDateModified",
                table: "ProductItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "SendBvnPhoneVerificationCodeResponse",
                columns: table => new
                {
                    SendBvnPhoneVerificationCodeResponseId = table.Column<long>(type: "bigint", nullable: false)
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
                    table.PrimaryKey("PK_SendBvnPhoneVerificationCodeResponse", x => x.SendBvnPhoneVerificationCodeResponseId);
                });

            migrationBuilder.CreateTable(
                name: "VerifyBvnPhoneConfirmationCodeRequest",
                columns: table => new
                {
                    VerifyBvnPhoneConfirmationCodeRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerifyBvnPhoneConfirmationCodeRequest", x => x.VerifyBvnPhoneConfirmationCodeRequestId);
                });

            migrationBuilder.CreateTable(
                name: "VerifyBvnPhoneConfirmationCodeResponse",
                columns: table => new
                {
                    VerifyBvnPhoneConfirmationCodeResponseId = table.Column<long>(type: "bigint", nullable: false)
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
                    table.PrimaryKey("PK_VerifyBvnPhoneConfirmationCodeResponse", x => x.VerifyBvnPhoneConfirmationCodeResponseId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SendBvnPhoneVerificationCodeResponse");

            migrationBuilder.DropTable(
                name: "VerifyBvnPhoneConfirmationCodeRequest");

            migrationBuilder.DropTable(
                name: "VerifyBvnPhoneConfirmationCodeResponse");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProductItems");

            migrationBuilder.DropColumn(
                name: "LastDateModified",
                table: "ProductItems");
        }
    }
}
