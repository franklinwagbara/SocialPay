using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class created_invoice_module : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvoicePaymentLink",
                columns: table => new
                {
                    InvoicePaymentLinkId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    InvoiceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Qty = table.Column<long>(type: "bigint", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    TransactionStatus = table.Column<bool>(type: "bit", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoicePaymentLink", x => x.InvoicePaymentLinkId);
                    table.ForeignKey(
                        name: "FK_InvoicePaymentLink_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoicePaymentLink_ClientAuthenticationId",
                table: "InvoicePaymentLink",
                column: "ClientAuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoicePaymentLink");
        }
    }
}
