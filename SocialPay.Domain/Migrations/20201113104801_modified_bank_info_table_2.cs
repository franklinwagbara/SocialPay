using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class modified_bank_info_table_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemDispute");

            migrationBuilder.RenameColumn(
                name: "CustomerInfo",
                table: "TransactionLog",
                newName: "MerchantInfo");

            migrationBuilder.AddColumn<string>(
                name: "ProcessedBy",
                table: "ItemAcceptedOrRejected",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DisputeRequestLog",
                columns: table => new
                {
                    DisputeRequestLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    DisputeComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerTransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisputeFile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DisputeRequestLog", x => x.DisputeRequestLogId);
                    table.ForeignKey(
                        name: "FK_DisputeRequestLog_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DisputeRequestLog_ClientAuthenticationId",
                table: "DisputeRequestLog",
                column: "ClientAuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DisputeRequestLog");

            migrationBuilder.DropColumn(
                name: "ProcessedBy",
                table: "ItemAcceptedOrRejected");

            migrationBuilder.RenameColumn(
                name: "MerchantInfo",
                table: "TransactionLog",
                newName: "CustomerInfo");

            migrationBuilder.CreateTable(
                name: "ItemDispute",
                columns: table => new
                {
                    ItemDisputeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DisputeComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemAcceptedOrRejectedId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDispute", x => x.ItemDisputeId);
                    table.ForeignKey(
                        name: "FK_ItemDispute_ItemAcceptedOrRejected_ItemAcceptedOrRejectedId",
                        column: x => x.ItemAcceptedOrRejectedId,
                        principalTable: "ItemAcceptedOrRejected",
                        principalColumn: "ItemAcceptedOrRejectedId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemDispute_ItemAcceptedOrRejectedId",
                table: "ItemDispute",
                column: "ItemAcceptedOrRejectedId");
        }
    }
}
