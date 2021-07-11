using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class updatedstoretables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.RenameColumn(
                name: "StoreId",
                table: "Products",
                newName: "MerchantStoreId");

            migrationBuilder.AddColumn<string>(
                name: "FileLocation",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LinkCategory",
                table: "MerchantPaymentSetup",
                type: "NVARCHAR(20)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "MerchantStoreId",
                table: "MerchantPaymentSetup",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "MerchantStore",
                columns: table => new
                {
                    MerchantStoreId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    FileLocation = table.Column<string>(type: "NVARCHAR(190)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MerchantStore", x => x.MerchantStoreId);
                    table.ForeignKey(
                        name: "FK_MerchantStore_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MerchantStore_ClientAuthenticationId",
                table: "MerchantStore",
                column: "ClientAuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MerchantStore");

            migrationBuilder.DropColumn(
                name: "FileLocation",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LinkCategory",
                table: "MerchantPaymentSetup");

            migrationBuilder.DropColumn(
                name: "MerchantStoreId",
                table: "MerchantPaymentSetup");

            migrationBuilder.RenameColumn(
                name: "MerchantStoreId",
                table: "Products",
                newName: "StoreId");

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    StoreId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileLocation = table.Column<string>(type: "NVARCHAR(190)", nullable: true),
                    Image = table.Column<string>(type: "NVARCHAR(90)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.StoreId);
                    table.ForeignKey(
                        name: "FK_Store_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Store_ClientAuthenticationId",
                table: "Store",
                column: "ClientAuthenticationId");
        }
    }
}
