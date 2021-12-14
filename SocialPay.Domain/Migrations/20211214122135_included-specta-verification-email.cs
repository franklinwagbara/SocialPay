using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class includedspectaverificationemail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_productInventoryHistories_Products_ProductId",
                table: "productInventoryHistories");

            migrationBuilder.DropIndex(
                name: "IX_productInventoryHistories_ProductId",
                table: "productInventoryHistories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ProductItems");

            migrationBuilder.DropColumn(
                name: "LastDateModified",
                table: "ProductItems");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "productInventoryHistories");

            migrationBuilder.CreateTable(
                name: "SendEmailVerificationCodeRequest",
                columns: table => new
                {
                    SendEmailVerificationCodeRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    clientBaseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    verificationCodeParameterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    emailParameterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendEmailVerificationCodeRequest", x => x.SendEmailVerificationCodeRequestId);
                });

            migrationBuilder.CreateTable(
                name: "SendEmailVerificationCodeResponse",
                columns: table => new
                {
                    SendEmailVerificationCodeResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    result = table.Column<bool>(type: "bit", nullable: false),
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
                    table.PrimaryKey("PK_SendEmailVerificationCodeResponse", x => x.SendEmailVerificationCodeResponseId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SendEmailVerificationCodeRequest");

            migrationBuilder.DropTable(
                name: "SendEmailVerificationCodeResponse");

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

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "productInventoryHistories",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_productInventoryHistories_ProductId",
                table: "productInventoryHistories",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_productInventoryHistories_Products_ProductId",
                table: "productInventoryHistories",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
