using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class createdsubmerchanttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubMerchantQRCodeOnboarding",
                columns: table => new
                {
                    SubMerchantQRCodeOnboardingId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MerchantQRCodeOnboardingId = table.Column<long>(type: "bigint", nullable: false),
                    MchNo = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    MerchantName = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    MerchantEmail = table.Column<string>(type: "NVARCHAR(80)", nullable: true),
                    MerchantPhoneNumber = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    SubFixed = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    SubAmount = table.Column<string>(type: "NVARCHAR(30)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubMerchantQRCodeOnboarding", x => x.SubMerchantQRCodeOnboardingId);
                    table.ForeignKey(
                        name: "FK_SubMerchantQRCodeOnboarding_MerchantQRCodeOnboarding_MerchantQRCodeOnboardingId",
                        column: x => x.MerchantQRCodeOnboardingId,
                        principalTable: "MerchantQRCodeOnboarding",
                        principalColumn: "MerchantQRCodeOnboardingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubMerchantQRCodeOnboardingResponse",
                columns: table => new
                {
                    SubMerchantQRCodeOnboardingResponseId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubMerchantQRCodeOnboardingId = table.Column<long>(type: "bigint", nullable: false),
                    ReturnCode = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    ReturnMsg = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    MchNo = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    MerchantName = table.Column<string>(type: "NVARCHAR(130)", nullable: true),
                    SubMchNo = table.Column<string>(type: "NVARCHAR(60)", nullable: true),
                    QrCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubMerchantQRCodeOnboardingResponse", x => x.SubMerchantQRCodeOnboardingResponseId);
                    table.ForeignKey(
                        name: "FK_SubMerchantQRCodeOnboardingResponse_SubMerchantQRCodeOnboarding_SubMerchantQRCodeOnboardingId",
                        column: x => x.SubMerchantQRCodeOnboardingId,
                        principalTable: "SubMerchantQRCodeOnboarding",
                        principalColumn: "SubMerchantQRCodeOnboardingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubMerchantQRCodeOnboarding_MerchantQRCodeOnboardingId",
                table: "SubMerchantQRCodeOnboarding",
                column: "MerchantQRCodeOnboardingId");

            migrationBuilder.CreateIndex(
                name: "IX_SubMerchantQRCodeOnboardingResponse_SubMerchantQRCodeOnboardingId",
                table: "SubMerchantQRCodeOnboardingResponse",
                column: "SubMerchantQRCodeOnboardingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubMerchantQRCodeOnboardingResponse");

            migrationBuilder.DropTable(
                name: "SubMerchantQRCodeOnboarding");
        }
    }
}
