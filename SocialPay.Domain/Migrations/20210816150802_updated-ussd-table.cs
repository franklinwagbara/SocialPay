using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialPay.Domain.Migrations
{
    public partial class updatedussdtable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UssdServiceResponseLog");

            migrationBuilder.AddColumn<string>(
                name: "CallBackResponseCode",
                table: "UssdServiceRequestLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CallBackResponseMessage",
                table: "UssdServiceRequestLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastDateModified",
                table: "UssdServiceRequestLog",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ResponseCode",
                table: "UssdServiceRequestLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResponseMessage",
                table: "UssdServiceRequestLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TraceID",
                table: "UssdServiceRequestLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionID",
                table: "UssdServiceRequestLog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionRef",
                table: "UssdServiceRequestLog",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CallBackResponseCode",
                table: "UssdServiceRequestLog");

            migrationBuilder.DropColumn(
                name: "CallBackResponseMessage",
                table: "UssdServiceRequestLog");

            migrationBuilder.DropColumn(
                name: "LastDateModified",
                table: "UssdServiceRequestLog");

            migrationBuilder.DropColumn(
                name: "ResponseCode",
                table: "UssdServiceRequestLog");

            migrationBuilder.DropColumn(
                name: "ResponseMessage",
                table: "UssdServiceRequestLog");

            migrationBuilder.DropColumn(
                name: "TraceID",
                table: "UssdServiceRequestLog");

            migrationBuilder.DropColumn(
                name: "TransactionID",
                table: "UssdServiceRequestLog");

            migrationBuilder.DropColumn(
                name: "TransactionRef",
                table: "UssdServiceRequestLog");

            migrationBuilder.CreateTable(
                name: "UssdServiceResponseLog",
                columns: table => new
                {
                    UssdServiceResponseLogId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CallBackResponseCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CallBackResponseMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateEntered = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ResponseCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TraceID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransactionRef = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UssdServiceRequestLogId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UssdServiceResponseLog", x => x.UssdServiceResponseLogId);
                    table.ForeignKey(
                        name: "FK_UssdServiceResponseLog_UssdServiceRequestLog_UssdServiceRequestLogId",
                        column: x => x.UssdServiceRequestLogId,
                        principalTable: "UssdServiceRequestLog",
                        principalColumn: "UssdServiceRequestLogId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UssdServiceResponseLog_UssdServiceRequestLogId",
                table: "UssdServiceResponseLog",
                column: "UssdServiceRequestLogId");
        }
    }
}
