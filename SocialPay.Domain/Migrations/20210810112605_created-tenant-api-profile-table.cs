using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class createdtenantapiprofiletable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TransactionType",
                table: "WebHookTransactionRequestLog",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionTime",
                table: "WebHookTransactionRequestLog",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionAmount",
                table: "WebHookTransactionRequestLog",
                type: "NVARCHAR(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TimeStamp",
                table: "WebHookTransactionRequestLog",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubMerchantNo",
                table: "WebHookTransactionRequestLog",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubMerchantName",
                table: "WebHookTransactionRequestLog",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResidualAmount",
                table: "WebHookTransactionRequestLog",
                type: "NVARCHAR(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrderSn",
                table: "WebHookTransactionRequestLog",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrderNo",
                table: "WebHookTransactionRequestLog",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NotificationType",
                table: "WebHookTransactionRequestLog",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MerchantNo",
                table: "WebHookTransactionRequestLog",
                type: "NVARCHAR(40)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MerchantName",
                table: "WebHookTransactionRequestLog",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MerchantFee",
                table: "WebHookTransactionRequestLog",
                type: "NVARCHAR(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "webHookUri",
                table: "WebHookRequest",
                type: "NVARCHAR(250)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "headers",
                table: "WebHookRequest",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "filters",
                table: "WebHookRequest",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "WebHookRequest",
                type: "NVARCHAR(150)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionStatus",
                table: "StoreTransactionLogDetails",
                type: "NVARCHAR(10)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionStatus",
                table: "StoreTransactionLog",
                type: "NVARCHAR(20)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentReference",
                table: "StoreTransactionLog",
                type: "NVARCHAR(90)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TenantProfile",
                columns: table => new
                {
                    TenantProfileId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TenantName = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    Address = table.Column<string>(type: "NVARCHAR(350)", nullable: true),
                    WebSiteUrl = table.Column<string>(type: "NVARCHAR(190)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantProfile", x => x.TenantProfileId);
                    table.ForeignKey(
                        name: "FK_TenantProfile_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantProfile_ClientAuthenticationId",
                table: "TenantProfile",
                column: "ClientAuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantProfile");

            migrationBuilder.AlterColumn<string>(
                name: "TransactionType",
                table: "WebHookTransactionRequestLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionTime",
                table: "WebHookTransactionRequestLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionAmount",
                table: "WebHookTransactionRequestLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TimeStamp",
                table: "WebHookTransactionRequestLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubMerchantNo",
                table: "WebHookTransactionRequestLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubMerchantName",
                table: "WebHookTransactionRequestLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResidualAmount",
                table: "WebHookTransactionRequestLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrderSn",
                table: "WebHookTransactionRequestLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrderNo",
                table: "WebHookTransactionRequestLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NotificationType",
                table: "WebHookTransactionRequestLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MerchantNo",
                table: "WebHookTransactionRequestLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(40)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MerchantName",
                table: "WebHookTransactionRequestLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MerchantFee",
                table: "WebHookTransactionRequestLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(30)",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "webHookUri",
                table: "WebHookRequest",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(250)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "headers",
                table: "WebHookRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "filters",
                table: "WebHookRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "description",
                table: "WebHookRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(150)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionStatus",
                table: "StoreTransactionLogDetails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(10)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TransactionStatus",
                table: "StoreTransactionLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(20)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentReference",
                table: "StoreTransactionLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(90)",
                oldNullable: true);
        }
    }
}
