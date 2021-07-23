using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class createdproductDetailstable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductItems",
                columns: table => new
                {
                    ProductItemsId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    FileLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductItems", x => x.ProductItemsId);
                    table.ForeignKey(
                        name: "FK_ProductItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QrPaymentRequest",
                columns: table => new
                {
                    QrPaymentRequestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    OrderNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MchNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubMchNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentRequestReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrPaymentRequest", x => x.QrPaymentRequestId);
                    table.ForeignKey(
                        name: "FK_QrPaymentRequest_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QrPaymentResponse",
                columns: table => new
                {
                    QrPaymentResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QrPaymentRequestId = table.Column<long>(type: "bigint", nullable: false),
                    OrderSn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturnMsg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SessionID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QrPaymentResponse", x => x.QrPaymentResponseId);
                    table.ForeignKey(
                        name: "FK_QrPaymentResponse_QrPaymentRequest_QrPaymentRequestId",
                        column: x => x.QrPaymentRequestId,
                        principalTable: "QrPaymentRequest",
                        principalColumn: "QrPaymentRequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductItems_ProductId",
                table: "ProductItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_QrPaymentRequest_ClientAuthenticationId",
                table: "QrPaymentRequest",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_QrPaymentResponse_QrPaymentRequestId",
                table: "QrPaymentResponse",
                column: "QrPaymentRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductItems");

            migrationBuilder.DropTable(
                name: "QrPaymentResponse");

            migrationBuilder.DropTable(
                name: "QrPaymentRequest");
        }
    }
}
