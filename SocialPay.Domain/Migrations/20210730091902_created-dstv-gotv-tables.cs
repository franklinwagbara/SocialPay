using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class createddstvgotvtables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DstvAccountLookup",
                columns: table => new
                {
                    DstvAccountLookupId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    merchantId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    merchantReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    transactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vasId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    countryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DstvAccountLookup", x => x.DstvAccountLookupId);
                    table.ForeignKey(
                        name: "FK_DstvAccountLookup_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SingleDstvPayment",
                columns: table => new
                {
                    SingleDstvPaymentId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    AccountLookupReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    amountInCents = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    merchantId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    merchantReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    transactionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vasId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    countryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    customerId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    key = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SingleDstvPayment", x => x.SingleDstvPaymentId);
                    table.ForeignKey(
                        name: "FK_SingleDstvPayment_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DstvAccountLookupResponse",
                columns: table => new
                {
                    DstvAccountLookupResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DstvAccountLookupId = table.Column<long>(type: "bigint", nullable: false),
                    merchantReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    payUVasReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    resultCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    resultMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pointOfFailure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    merchantId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DstvAccountLookupResponse", x => x.DstvAccountLookupResponseId);
                    table.ForeignKey(
                        name: "FK_DstvAccountLookupResponse_DstvAccountLookup_DstvAccountLookupId",
                        column: x => x.DstvAccountLookupId,
                        principalTable: "DstvAccountLookup",
                        principalColumn: "DstvAccountLookupId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SingleDstvPaymentResponse",
                columns: table => new
                {
                    SingleDstvPaymentResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SingleDstvPaymentId = table.Column<long>(type: "bigint", nullable: false),
                    merchantReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    payUVasReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    resultCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    resultMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    pointOfFailure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    merchantId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SingleDstvPaymentResponse", x => x.SingleDstvPaymentResponseId);
                    table.ForeignKey(
                        name: "FK_SingleDstvPaymentResponse_SingleDstvPayment_SingleDstvPaymentId",
                        column: x => x.SingleDstvPaymentId,
                        principalTable: "SingleDstvPayment",
                        principalColumn: "SingleDstvPaymentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DstvAccountLookup_ClientAuthenticationId",
                table: "DstvAccountLookup",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_DstvAccountLookupResponse_DstvAccountLookupId",
                table: "DstvAccountLookupResponse",
                column: "DstvAccountLookupId");

            migrationBuilder.CreateIndex(
                name: "IX_SingleDstvPayment_ClientAuthenticationId",
                table: "SingleDstvPayment",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_SingleDstvPaymentResponse_SingleDstvPaymentId",
                table: "SingleDstvPaymentResponse",
                column: "SingleDstvPaymentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DstvAccountLookupResponse");

            migrationBuilder.DropTable(
                name: "SingleDstvPaymentResponse");

            migrationBuilder.DropTable(
                name: "DstvAccountLookup");

            migrationBuilder.DropTable(
                name: "SingleDstvPayment");
        }
    }
}
