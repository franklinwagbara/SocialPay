using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class createdstoretables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReferralCode",
                table: "ClientAuthentication",
                type: "NVARCHAR(30)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(50)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ProductOptionRequest",
                columns: table => new
                {
                    OptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOptionRequest", x => x.OptionId);
                });

            migrationBuilder.CreateTable(
                name: "StoreCategoryRequest",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreCategoryRequest", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "MerchantStoreRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    OptionId = table.Column<int>(type: "int", nullable: true),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StoreCategoryCategoryId = table.Column<int>(type: "int", nullable: true),
                    ProductOptionOptionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantStoreRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MerchantStoreRequest_ProductOptionRequest_ProductOptionOptionId",
                        column: x => x.ProductOptionOptionId,
                        principalTable: "ProductOptionRequest",
                        principalColumn: "OptionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MerchantStoreRequest_StoreCategoryRequest_StoreCategoryCategoryId",
                        column: x => x.StoreCategoryCategoryId,
                        principalTable: "StoreCategoryRequest",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MerchantStoreRequest_ProductOptionOptionId",
                table: "MerchantStoreRequest",
                column: "ProductOptionOptionId");

            migrationBuilder.CreateIndex(
                name: "IX_MerchantStoreRequest_StoreCategoryCategoryId",
                table: "MerchantStoreRequest",
                column: "StoreCategoryCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MerchantStoreRequest");

            migrationBuilder.DropTable(
                name: "ProductOptionRequest");

            migrationBuilder.DropTable(
                name: "StoreCategoryRequest");

            migrationBuilder.AlterColumn<string>(
                name: "ReferralCode",
                table: "ClientAuthentication",
                type: "NVARCHAR(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "NVARCHAR(30)",
                oldNullable: true);
        }
    }
}
