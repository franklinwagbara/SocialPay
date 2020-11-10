using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class create_funds_transfer_log : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "TransactionLog",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "FundsTransferLog",
                columns: table => new
                {
                    FundsTransferLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionReference = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDebited = table.Column<bool>(type: "bit", nullable: false),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastDateModified = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundsTransferLog", x => x.FundsTransferLogId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FundsTransferLog");

            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "TransactionLog");
        }
    }
}
