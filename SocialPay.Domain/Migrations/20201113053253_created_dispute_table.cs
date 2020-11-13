using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class created_dispute_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemDispute",
                columns: table => new
                {
                    ItemDisputeId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemAcceptedOrRejectedId = table.Column<long>(type: "bigint", nullable: false),
                    DisputeComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemDispute");
        }
    }
}
