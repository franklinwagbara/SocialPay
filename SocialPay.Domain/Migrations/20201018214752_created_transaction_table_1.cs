using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class created_transaction_table_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateEntered",
                table: "TransactionLog",
                newName: "TransactionDate");

            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "TransactionLog",
                type: "bit",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "TransactionLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerTransactionReference",
                table: "TransactionLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "TransactionLog",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Message",
                table: "TransactionLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderStatus",
                table: "TransactionLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LinkCategory",
                columns: table => new
                {
                    LinkCategoryId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientAuthenticationId = table.Column<long>(type: "bigint", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Channel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkCategory", x => x.LinkCategoryId);
                    table.ForeignKey(
                        name: "FK_LinkCategory_ClientAuthentication_ClientAuthenticationId",
                        column: x => x.ClientAuthenticationId,
                        principalTable: "ClientAuthentication",
                        principalColumn: "ClientAuthenticationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinkCategory_ClientAuthenticationId",
                table: "LinkCategory",
                column: "ClientAuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinkCategory");

            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "TransactionLog");

            migrationBuilder.DropColumn(
                name: "CustomerTransactionReference",
                table: "TransactionLog");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "TransactionLog");

            migrationBuilder.DropColumn(
                name: "Message",
                table: "TransactionLog");

            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "TransactionLog");

            migrationBuilder.RenameColumn(
                name: "TransactionDate",
                table: "TransactionLog",
                newName: "DateEntered");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "TransactionLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");
        }
    }
}
