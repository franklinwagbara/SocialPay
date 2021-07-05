using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class UpdatedMerchantinfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OtherMerchantBankInfo",
                columns: table => new
                {
                    MerchantOtherBankInfoId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    BankName = table.Column<string>(type: "NVARCHAR(50)", nullable: true),
                    BankCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    BranchCode = table.Column<string>(type: "NVARCHAR(20)", nullable: true),
                    LedCode = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    Nuban = table.Column<string>(type: "NVARCHAR(15)", nullable: true),
                    AccountName = table.Column<string>(type: "NVARCHAR(65)", nullable: true),
                    Currency = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    BVN = table.Column<string>(type: "NVARCHAR(12)", nullable: true),
                    Country = table.Column<string>(type: "NVARCHAR(25)", nullable: true),
                    CusNum = table.Column<string>(type: "NVARCHAR(10)", nullable: true),
                    KycLevel = table.Column<string>(type: "NVARCHAR(5)", nullable: true),
                    DefaultAccount = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherMerchantBankInfo", x => x.MerchantOtherBankInfoId);
                    table.ForeignKey(
                        name: "FK_OtherMerchantBankInfo_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OtherMerchantBankInfo_ClientAuthenticationId",
                table: "OtherMerchantBankInfo",
                column: "ClientAuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OtherMerchantBankInfo");
        }
    }
}
