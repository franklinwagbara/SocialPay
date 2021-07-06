using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class creatednibsqrtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MerchantQRCodeOnboarding",
                columns: table => new
                {
                    MerchantQRCodeOnboardingId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    Tin = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    Contact = table.Column<string>(type: "NVARCHAR(250)", nullable: true),
                    Phone = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    Email = table.Column<string>(type: "NVARCHAR(60)", nullable: true),
                    Address = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantQRCodeOnboarding", x => x.MerchantQRCodeOnboardingId);
                    table.ForeignKey(
                        name: "FK_MerchantQRCodeOnboarding_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MerchantQRCodeOnboardingResponse",
                columns: table => new
                {
                    MerchantQRCodeOnboardingResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantQRCodeOnboardingId = table.Column<long>(type: "bigint", nullable: false),
                    ReturnCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    ReturnMsg = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    MchNo = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    MerchantName = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    MerchantTIN = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    MerchantAddress = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    MerchantContactName = table.Column<string>(type: "NVARCHAR(150)", nullable: true),
                    MerchantPhoneNumber = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    MerchantEmail = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    JsonResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantQRCodeOnboardingResponse", x => x.MerchantQRCodeOnboardingResponseId);
                    table.ForeignKey(
                        name: "FK_MerchantQRCodeOnboardingResponse_MerchantQRCodeOnboarding_MerchantQRCodeOnboardingId",
                        column: x => x.MerchantQRCodeOnboardingId,
                        principalTable: "MerchantQRCodeOnboarding",
                        principalColumn: "MerchantQRCodeOnboardingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MerchantQRCodeOnboarding_ClientAuthenticationId",
                table: "MerchantQRCodeOnboarding",
                column: "ClientAuthenticationId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantQRCodeOnboardingResponse_MerchantQRCodeOnboardingId",
                table: "MerchantQRCodeOnboardingResponse",
                column: "MerchantQRCodeOnboardingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MerchantQRCodeOnboardingResponse");

            migrationBuilder.DropTable(
                name: "MerchantQRCodeOnboarding");
        }
    }
}
