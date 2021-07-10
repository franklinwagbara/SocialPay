using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class updatednibbstables1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AccountName",
                table: "BindMerchant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AccountNumber",
                table: "BindMerchant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankNo",
                table: "BindMerchant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MchNo",
                table: "BindMerchant",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BindMerchantResponse",
                columns: table => new
                {
                    BindMerchantResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BindMerchantId = table.Column<long>(type: "bigint", nullable: false),
                    ReturnCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Mch_no = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JsonResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BindMerchantResponse", x => x.BindMerchantResponseId);
                    table.ForeignKey(
                        name: "FK_BindMerchantResponse_BindMerchant_BindMerchantId",
                        column: x => x.BindMerchantId,
                        principalTable: "BindMerchant",
                        principalColumn: "BindMerchantId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BindMerchantResponse_BindMerchantId",
                table: "BindMerchantResponse",
                column: "BindMerchantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BindMerchantResponse");

            migrationBuilder.DropColumn(
                name: "AccountName",
                table: "BindMerchant");

            migrationBuilder.DropColumn(
                name: "AccountNumber",
                table: "BindMerchant");

            migrationBuilder.DropColumn(
                name: "BankNo",
                table: "BindMerchant");

            migrationBuilder.DropColumn(
                name: "MchNo",
                table: "BindMerchant");
        }
    }
}
