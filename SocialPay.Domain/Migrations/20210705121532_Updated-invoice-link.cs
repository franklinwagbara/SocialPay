using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class Updatedinvoicelink : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "InvoicePaymentLink",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VAT",
                table: "InvoicePaymentLink",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "InvoicePaymentLinkToMulitpleEmails",
                columns: table => new
                {
                    InvoicePaymentLinkToMulitpleEmailsId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoicePaymentLinkId = table.Column<long>(type: "bigint", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePaymentLinkToMulitpleEmails", x => x.InvoicePaymentLinkToMulitpleEmailsId);
                    table.ForeignKey(
                        name: "FK_InvoicePaymentLinkToMulitpleEmails_InvoicePaymentLink_InvoicePaymentLinkId",
                        column: x => x.InvoicePaymentLinkId,
                        principalTable: "InvoicePaymentLink",
                        principalColumn: "InvoicePaymentLinkId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePaymentLinkToMulitpleEmails_InvoicePaymentLinkId",
                table: "InvoicePaymentLinkToMulitpleEmails",
                column: "InvoicePaymentLinkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoicePaymentLinkToMulitpleEmails");

            migrationBuilder.DropColumn(
                name: "Discount",
                table: "InvoicePaymentLink");

            migrationBuilder.DropColumn(
                name: "VAT",
                table: "InvoicePaymentLink");
        }
    }
}
