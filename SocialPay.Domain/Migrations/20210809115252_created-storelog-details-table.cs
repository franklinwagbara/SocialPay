using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class createdstorelogdetailstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "StoreTransactionLog");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "StoreTransactionLog");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "StoreTransactionLog");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "StoreTransactionLog");

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "StoreTransactionLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "CustomerOtherPaymentsInfo",
                type: "NVARCHAR(20)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StoreTransactionLogDetails",
                columns: table => new
                {
                    StoreTransactionLogDetailsId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreTransactionLogId = table.Column<long>(type: "bigint", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    Size = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TransactionStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreTransactionLogDetails", x => x.StoreTransactionLogDetailsId);
                    table.ForeignKey(
                        name: "FK_StoreTransactionLogDetails_StoreTransactionLog_StoreTransactionLogId",
                        column: x => x.StoreTransactionLogId,
                        principalTable: "StoreTransactionLog",
                        principalColumn: "StoreTransactionLogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransactionLogDetails_StoreTransactionLogId",
                table: "StoreTransactionLogDetails",
                column: "StoreTransactionLogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StoreTransactionLogDetails");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "StoreTransactionLog");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "CustomerOtherPaymentsInfo");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "StoreTransactionLog",
                type: "NVARCHAR(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProductId",
                table: "StoreTransactionLog",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "StoreTransactionLog",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "StoreTransactionLog",
                type: "NVARCHAR(10)",
                nullable: true);
        }
    }
}
